import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { routePaths } from '../../constants/routePaths';
import { useFormik } from 'formik';
import * as Yup from 'yup';
import { ResetPasswordStepOneRequest } from '../../api/generatedSdk';
import { usersApi } from '../../api';
import { Box, TextField } from '@mui/material';
import { PageTitle } from '../../components/pageTitle';
import { LoadingButton } from '@mui/lab';

export type RouterState = {
  errorMessage?: string;
};

const ResetPasswordStepOne = () => {
  const navigate = useNavigate();

  const formik = useFormik({
    initialValues: {
      email: '',
    },
    validationSchema: Yup.object({
      email: Yup.string().email('Email must be a valid email').required('Required'),
    }),
    onSubmit: async (values) => {
      try {
        const request: ResetPasswordStepOneRequest = {
          email: values.email,
        };
        await usersApi.resetPasswordStepOne({
          resetPasswordStepOneRequest: request,
        });
        navigate(routePaths.resetPasswordStepOneSuccess);
      } catch (error) {
        toast.error('Password could not be reset');
      }
    },
  });

  return (
    <Box component="form" sx={{ margin: '1rem auto', maxWidth: '375px' }} onSubmit={formik.handleSubmit}>
      <PageTitle sx={{ textAlign: 'center', marginBottom: '1rem' }}> Find Your Account </PageTitle>
      <Box
        sx={{
          display: 'flex',
          gap: '1rem',
          flexDirection: 'column',
        }}
      >
        <TextField
          fullWidth
          label="Email"
          size="small"
          {...formik.getFieldProps('email')}
          helperText={formik.touched.email && formik.errors.email}
          error={formik.touched.email && Boolean(formik.errors.email)}
        />
        <LoadingButton
          variant="contained"
          fullWidth
          type="submit"
          loading={formik.isSubmitting}
          disabled={!formik.isValid || !formik.dirty}
        >
          Find Account
        </LoadingButton>
      </Box>
    </Box>
  );
};

export default ResetPasswordStepOne;
