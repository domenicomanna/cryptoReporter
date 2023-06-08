import { Box, CircularProgress } from '@mui/material';

const PageLoader = () => {
  return (
    <Box sx={{ display: 'flex', height: '55vh', justifyContent: 'center', alignItems: 'center' }}>
      <CircularProgress size={'56px'} />
    </Box>
  );
};

export default PageLoader;
