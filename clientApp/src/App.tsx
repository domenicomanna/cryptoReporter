import { Route, Routes } from 'react-router-dom';
import Layout from './components/layout/layout';
import ProtectedRoute from './components/protectedRoute';
import { routePaths } from './constants/routePaths';
import Home from './pages/home';
import Login from './pages/login';
import SignUp from './pages/signUp';

const App = () => {
  return (
    <Routes>
      <Route element={<ProtectedRoute />}>
        <Route element={<Layout />}>
          <Route path={routePaths.home} element={<Home />} />
        </Route>
      </Route>
      <Route path={routePaths.login} element={<Login />} />
      <Route path={routePaths.signUp} element={<SignUp />} />
    </Routes>
  );
};

export default App;
