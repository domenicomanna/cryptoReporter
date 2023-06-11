import AppBar from '@mui/material/AppBar';
import Toolbar from '@mui/material/Toolbar';
import { Container, Link as MuiLink } from '@mui/material';
import { Link as ReactRouterLink } from 'react-router-dom';
import { routePaths } from '../../constants/routePaths';
import { useContext } from 'react';
import { UserContext } from '../../contexts/UserContext';
import { usersApi } from '../../api';

const Header = () => {
  const { clearUserInfo } = useContext(UserContext);

  const handleLogout = async () => {
    clearUserInfo();
    await usersApi.logout();
  };

  return (
    <AppBar position="static" sx={{ marginBottom: '1rem' }}>
      <Container disableGutters>
        <Toolbar sx={{ justifyContent: 'flex-end', gap: '1rem' }} variant="dense">
          <MuiLink component={ReactRouterLink} to={routePaths.home} sx={{ color: '#fff' }}>
            Home
          </MuiLink>
          <MuiLink component={ReactRouterLink} to={routePaths.login} sx={{ color: '#fff' }} onClick={handleLogout}>
            Logout
          </MuiLink>
        </Toolbar>
      </Container>
    </AppBar>
  );
};

export default Header;
