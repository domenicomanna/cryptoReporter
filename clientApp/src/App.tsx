import { Route, Routes } from 'react-router-dom';
import AuthenticatedLayout from './components/authenticatedLayout/authenticatedLayout';
import ProtectedRoute from './components/protectedRoute';
import { routePaths } from './constants/routePaths';
import Home from './pages/home';
import Login from './pages/login';
import SignUp from './pages/signUp';
import ResetPasswordStepOne from './pages/resetPasswordStepOne';
import ResetPasswordStepTwo from './pages/resetPasswordStepTwo';
import ResetPasswordStepOneSuccess from './pages/resetPasswordStepOneSuccess';
import UnauthenticatedLayout from './components/unauthenticatedLayout/unauthenticatedLayout';

const App = () => {
  return (
    <Routes>
      <Route element={<ProtectedRoute />}>
        <Route element={<AuthenticatedLayout />}>
          <Route path={routePaths.home} element={<Home />} />
        </Route>
      </Route>
      <Route element={<UnauthenticatedLayout />}>
        <Route path={routePaths.login} element={<Login />} />
        <Route path={routePaths.signUp} element={<SignUp />} />
        <Route path={routePaths.resetPasswordStepOne} element={<ResetPasswordStepOne />} />
        <Route path={routePaths.resetPasswordStepTwo} element={<ResetPasswordStepTwo />} />
        <Route path={routePaths.resetPasswordStepOneSuccess} element={<ResetPasswordStepOneSuccess />} />
      </Route>
    </Routes>
  );
};

export default App;
