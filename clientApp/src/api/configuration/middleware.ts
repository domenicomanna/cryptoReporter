import { routePaths } from '../../constants/routePaths';
import { UserInfo } from '../../contexts/UserContext';
import { RouterState as LoginPageRouterState } from '../../pages/login';
import { router } from '../../router';
import { userStorageHelper } from '../../utils/userStorageHelper';
import { Middleware, ReauthenticateWithRefreshTokenResult, RequestContext, ResponseContext } from '../generatedSdk';
import { refreshToken } from './refreshToken';

let _refreshTokenPromise: Promise<ReauthenticateWithRefreshTokenResult> | null = null;

export const middleware: Middleware[] = [
  {
    // eslint-disable-next-line @typescript-eslint/require-await
    pre: async (context: RequestContext) => {
      const userInfo = userStorageHelper.getCurrentUserInfo();
      if (!userInfo) return;
      context.init.headers = new Headers({
        ...context.init.headers,
        Authorization: `Bearer ${userInfo.token}`,
      });
    },
    post: async (context: ResponseContext): Promise<Response> => {
      if (context.url.toLowerCase().includes('login')) return context.response;
      if (context.response.status !== 401) return context.response;
      try {
        if (!_refreshTokenPromise) _refreshTokenPromise = refreshToken();
        const authenticationResult = await _refreshTokenPromise;
        if (!authenticationResult) {
          await handleLogout();
          return context.response;
        }
        const currentUserInfo = userStorageHelper.getCurrentUserInfo();
        const userInfo: UserInfo = {
          userId: authenticationResult.userId,
          token: authenticationResult.accessToken,
          fiatCurrency: currentUserInfo?.fiatCurrency ?? '',
        };
        userStorageHelper.storeUserInfo(userInfo);
        return await fetch(context.url, {
          ...context.init,
          headers: new Headers({
            ...context.init.headers,
            Authorization: `Bearer ${authenticationResult.accessToken}`,
          }),
        });
      } catch {
        await handleLogout();
        return context.response;
      } finally {
        _refreshTokenPromise = null;
      }
    },
  },
];

const handleLogout = async () => {
  const loginPageRouterState: LoginPageRouterState = {
    errorMessage: 'Session expired. Please sign in again',
  };
  await router.navigate(routePaths.login, {
    state: loginPageRouterState,
  });
  userStorageHelper.removeCurrentUserInfo();
};
