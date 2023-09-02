import { createContext, useState, useEffect, FC, ReactElement } from 'react';
import { userStorageHelper } from '../utils/userStorageHelper';
import { router } from '..';
import { routePaths } from '../constants/routePaths';
import { RouterState as LoginPageRouterState } from '../pages/login';
import { usersApi } from '../api';
import PageLoader from '../components/pageLoader';

export type UserInfo = {
  userId: number;
  token: string;
  fiatCurrency: string;
};

type UserContextType = {
  userInfo: UserInfo | null;
  setUserInfo: (userInfo: UserInfo) => void;
  clearUserInfo: () => void;
};

export const UserContext = createContext({} as UserContextType);

type Props = {
  children: ReactElement;
};

const UserContextProvider: FC<Props> = ({ children }) => {
  const [userInfo, setUserInfo] = useState<UserInfo | null>(userStorageHelper.getCurrentUserInfo());
  const [userDetailsAreBeingFetched, setUserDetailsAreBeingFetched] = useState(true);

  useEffect(() => {
    const fetchUser = async (userId: number) => {
      try {
        await usersApi.getUser({ userId });
      } catch (error) {
        const loginPageRouterState: LoginPageRouterState = {
          errorMessage: 'Session expired. Please sign in again',
        };
        await router.navigate(routePaths.login, {
          state: loginPageRouterState,
        });
        userStorageHelper.removeCurrentUserInfo();
      } finally {
        setUserDetailsAreBeingFetched(false);
      }
    };
    if (userInfo?.userId) void fetchUser(userInfo.userId);
    else setUserDetailsAreBeingFetched(false);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const handleSetUserInfo = (userInfo: UserInfo) => {
    setUserInfo(userInfo);
    userStorageHelper.storeUserInfo(userInfo);
  };

  const clearUserInfo = () => {
    setUserInfo(null);
    userStorageHelper.removeCurrentUserInfo();
  };

  return (
    <UserContext.Provider
      value={{
        userInfo,
        setUserInfo: handleSetUserInfo,
        clearUserInfo,
      }}
    >
      {userDetailsAreBeingFetched ? <PageLoader /> : children}
    </UserContext.Provider>
  );
};

export default UserContextProvider;
