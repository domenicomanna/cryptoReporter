import { useNavigate, useParams } from 'react-router-dom';
import { toast } from 'react-toastify';
import { routePaths } from '../../constants/routePaths';
import { useFormik } from 'formik';
import * as Yup from 'yup';
import { Box, TextField } from '@mui/material';
import { PageTitle } from '../../components/pageTitle';
import { LoadingButton } from '@mui/lab';
import { ResetPasswordStepTwoRequest } from '../../api/generatedSdk';
import { passwordSchema } from '../../validationSchemas/password';
import { usersApi } from '../../api';

export type RouterState = {
  errorMessage?: string;
};

const ResetPasswordStepTwo = () => {
  const navigate = useNavigate();
  const { token } = useParams();

  const formik = useFormik({
    initialValues: {
      password: '',
      confirmNewPassword: '',
    },
    validationSchema: Yup.object({
      password: passwordSchema,
      confirmNewPassword: Yup.string().required('Required'),
    }),
    onSubmit: async (values) => {
      try {
        const request: ResetPasswordStepTwoRequest = {
          resetPasswordToken: token ?? '',
          newPassword: values.password,
          confirmedNewPassword: values.confirmNewPassword,
        };
        await usersApi.resetPasswordStepTwo({
          resetPasswordStepTwoRequest: request,
        });
        toast.success('Password reset!');
        navigate(routePaths.login);
      } catch (error) {
        toast.error('Password could not be reset');
      }
    },
  });

  return (
    <Box component="form" sx={{ margin: '1rem auto', maxWidth: '375px' }} onSubmit={formik.handleSubmit}>
      <PageTitle sx={{ textAlign: 'center', marginBottom: '1rem' }}> Reset Your Password </PageTitle>
      <Box
        sx={{
          display: 'flex',
          gap: '1rem',
          flexDirection: 'column',
        }}
      >
        <TextField
          fullWidth
          label="New Password"
          type="password"
          size="small"
          {...formik.getFieldProps('password')}
          helperText={formik.touched.password && formik.errors.password}
          error={formik.touched.password && Boolean(formik.errors.password)}
        />
        <TextField
          fullWidth
          label="Confirm Password"
          type="password"
          size="small"
          {...formik.getFieldProps('confirmNewPassword')}
          helperText={formik.touched.confirmNewPassword && formik.errors.confirmNewPassword}
          error={formik.touched.confirmNewPassword && Boolean(formik.errors.confirmNewPassword)}
        />
        <LoadingButton
          variant="contained"
          fullWidth
          type="submit"
          loading={formik.isSubmitting}
          disabled={!formik.isValid || !formik.dirty}
        >
          Reset Password
        </LoadingButton>
      </Box>
    </Box>
  );
};

export default ResetPasswordStepTwo;
