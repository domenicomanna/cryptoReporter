import { Container } from '@mui/material';
import { Outlet } from 'react-router-dom';
import Header from './header';

const AuthenticatedLayout = () => {
  return (
    <>
      <Header />
      <main>
        <Container>
          <Outlet />
        </Container>
      </main>
    </>
  );
};

export default AuthenticatedLayout;
