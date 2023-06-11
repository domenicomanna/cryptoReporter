import { Box, Typography } from '@mui/material';
import { PageTitle } from '../../components/pageTitle';

const ResetPasswordStepOneSuccess = () => {
  return (
    <Box sx={{ margin: '0 auto', maxWidth: '400px' }}>
      <PageTitle sx={{ textAlign: 'center', marginBottom: '.5rem' }}>Check Your Email</PageTitle>
      <Typography>
        An email has been sent to the email address provided. Click the link in the email to reset your password.
      </Typography>
    </Box>
  );
};

export default ResetPasswordStepOneSuccess;
