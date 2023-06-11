import AppBar from '@mui/material/AppBar';
import Toolbar from '@mui/material/Toolbar';
import { Container, Link as MuiLink } from '@mui/material';
import { Link as ReactRouterLink } from 'react-router-dom';
import { routePaths } from '../../constants/routePaths';

const Header = () => {
  return (
    <AppBar position="static" sx={{ marginBottom: '1rem' }}>
      <Container disableGutters>
        <Toolbar sx={{ justifyContent: 'flex-end' }} variant="dense">
          <MuiLink component={ReactRouterLink} to={routePaths.home} sx={{ color: '#fff' }}>
            Home
          </MuiLink>
        </Toolbar>
      </Container>
    </AppBar>
  );
};

export default Header;
