import { useContext, useState } from 'react';
import { Link as ReactRouterLink, Navigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { routePaths } from '../../constants/routePaths';
import { useFormik } from 'formik';
import * as Yup from 'yup';
import { CreateUserRequest, ResponseError } from '../../api/generatedSdk';
import { usersApi } from '../../api';
import { UserContext, UserInfo } from '../../contexts/UserContext';
import { Box, TextField, Link as MuiLink } from '@mui/material';
import { PageTitle } from '../../components/pageTitle';
import { LoadingButton } from '@mui/lab';
import { passwordSchema } from '../../validationSchemas/password';
import { ProblemDetails } from '../../api/types';

const SignUp = () => {
  const { setUserInfo } = useContext(UserContext);
  const [userHasBeenCreated, setUserHasBeenCreated] = useState(false);

  const formik = useFormik({
    initialValues: {
      email: '',
      password: '',
      confirmPassword: '',
    },
    validationSchema: Yup.object({
      email: Yup.string().email('Email must be a valid email').required('Required'),
      password: passwordSchema,
      confirmPassword: Yup.string()
        .required('Required')
        .oneOf([Yup.ref('password')], 'Passwords must match'),
    }),
    onSubmit: async (values) => {
      const createUserRequest: CreateUserRequest = {
        email: values.email,
        password: values.password,
        confirmedPassword: values.confirmPassword,
      };
      try {
        const result = await usersApi.createUser({ createUserRequest });
        const userInfo: UserInfo = {
          userId: result.user.id,
          token: result.accessToken,
        };
        setUserInfo(userInfo);
        setUserHasBeenCreated(true);
      } catch (error) {
        const errorDetails = error instanceof ResponseError ? ((await error.response.json()) as ProblemDetails) : null;
        const errorMessage = errorDetails?.detail ?? errorDetails?.title ?? 'Account could not be created';
        toast.error(errorMessage);
      }
    },
  });

  if (userHasBeenCreated) return <Navigate replace to={routePaths.home} />;

  return (
    <Box component="form" sx={{ margin: '1rem auto', maxWidth: '375px' }} onSubmit={formik.handleSubmit}>
      <PageTitle sx={{ textAlign: 'center', marginBottom: '1rem' }}> Sign Up </PageTitle>
      <Box
        sx={{
          display: 'flex',
          gap: '1rem',
          flexDirection: 'column',
          alignItems: 'center',
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
        <TextField
          fullWidth
          label="Password"
          size="small"
          type="password"
          {...formik.getFieldProps('password')}
          helperText={formik.touched.password && formik.errors.password}
          error={formik.touched.password && Boolean(formik.errors.password)}
        />
        <TextField
          fullWidth
          label="Confirm Password"
          size="small"
          type="password"
          {...formik.getFieldProps('confirmPassword')}
          helperText={formik.touched.confirmPassword && formik.errors.confirmPassword}
          error={formik.touched.confirmPassword && Boolean(formik.errors.confirmPassword)}
        />
        <LoadingButton
          variant="contained"
          fullWidth
          type="submit"
          loading={formik.isSubmitting}
          disabled={!formik.isValid || !formik.dirty}
        >
          Create Account
        </LoadingButton>
        <MuiLink component={ReactRouterLink} to={routePaths.login}>
          Already have an account? Log in
        </MuiLink>
      </Box>
    </Box>
  );
};

export default SignUp;
