import { useNavigate, useParams } from 'react-router-dom';
import { toast } from 'react-toastify';
import { routePaths } from '../../constants/routePaths';
import { useFormik } from 'formik';
import * as Yup from 'yup';
import { Box, Button, TextField } from '@mui/material';
import { PageTitle } from '../../components/pageTitle';
import { passwordSchema } from '../../validationSchemas/password';
import { useResetPasswordStepTwo } from './mutations/useResetPasswordStepTwo';

export type RouterState = {
  errorMessage?: string;
};

export type ResetPasswordStepTwoFormValues = {
  password: string;
  confirmNewPassword: string;
  token: string;
};

const ResetPasswordStepTwo = () => {
  const navigate = useNavigate();
  const { token } = useParams();
  const resetPasswordStepTwoMutation = useResetPasswordStepTwo();

  const formik = useFormik<ResetPasswordStepTwoFormValues>({
    initialValues: {
      password: '',
      confirmNewPassword: '',
      token: token ?? '',
    },
    validationSchema: Yup.object({
      password: passwordSchema,
      confirmNewPassword: Yup.string().required('Required'),
    }),
    onSubmit: async (values) => {
      await resetPasswordStepTwoMutation.mutateAsync(values, {
        onSuccess: () => {
          toast.success('Password reset!');
          void navigate(routePaths.login);
        },
      });
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
        <Button
          variant="contained"
          fullWidth
          type="submit"
          loading={resetPasswordStepTwoMutation.isPending}
          disabled={!formik.isValid || !formik.dirty}
        >
          Reset Password
        </Button>
      </Box>
    </Box>
  );
};

export default ResetPasswordStepTwo;
