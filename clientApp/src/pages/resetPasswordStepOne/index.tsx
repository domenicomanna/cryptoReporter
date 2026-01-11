import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { routePaths } from '../../constants/routePaths';
import { useFormik } from 'formik';
import * as Yup from 'yup';
import { ResetPasswordStepOneRequest } from '../../api/generatedSdk';
import { usersApi } from '../../api';
import { Box, Button, TextField } from '@mui/material';
import { PageTitle } from '../../components/pageTitle';
import { useMutation } from '@tanstack/react-query';

export type RouterState = {
  errorMessage?: string;
};

type FormValues = {
  email: string;
};

const ResetPasswordStepOne = () => {
  const navigate = useNavigate();

  const formik = useFormik<FormValues>({
    initialValues: {
      email: '',
    },
    validationSchema: Yup.object({
      email: Yup.string().email('Email must be a valid email').required('Required'),
    }),
    onSubmit: async (values) => {
      await resetPasswordStepOneMutation.mutateAsync(values);
    },
  });

  const resetPasswordStepOneMutation = useMutation({
    mutationFn: async (values: FormValues) => {
      const request: ResetPasswordStepOneRequest = {
        email: values.email,
      };
      await usersApi.resetPasswordStepOne({
        resetPasswordStepOneRequest: request,
      });
    },
    onSuccess: () => {
      void navigate(routePaths.resetPasswordStepOneSuccess);
    },
    onError: () => {
      toast.error('Password could not be reset');
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
        <Button
          variant="contained"
          fullWidth
          type="submit"
          loading={resetPasswordStepOneMutation.isPending}
          disabled={!formik.isValid || !formik.dirty}
        >
          Find Account
        </Button>
      </Box>
    </Box>
  );
};

export default ResetPasswordStepOne;
