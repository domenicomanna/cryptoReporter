import { FC, ReactElement, useContext } from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { routePaths } from '../constants/routePaths';
import { UserContext } from '../contexts/UserContext';

type Props = {
  children?: ReactElement;
};

const ProtectedRoute: FC<Props> = ({ children }) => {
  const { userInfo } = useContext(UserContext);
  if (!userInfo) {
    return <Navigate to={routePaths.login} replace />;
  }

  return children ? children : <Outlet />;
};

export default ProtectedRoute;
