import { useContext, useState } from 'react';
import { Link as ReactRouterLink, Navigate } from 'react-router-dom';
import { routePaths } from '../../constants/routePaths';
import { useFormik } from 'formik';
import * as Yup from 'yup';
import { UserContext, UserInfo } from '../../contexts/UserContext';
import { Box, TextField, Link as MuiLink, Autocomplete, CircularProgress, Button } from '@mui/material';
import { PageTitle } from '../../components/pageTitle';
import { passwordSchema } from '../../validationSchemas/password';
import { ArrowDropDown } from '@mui/icons-material';
import { useSignUp } from './mutations/useSignUp';
import { useGetFiatCurrencies } from './queries/useGetFiatCurrencies';

export type SignUpFormValues = {
  email: string;
  password: string;
  confirmPassword: string;
  fiatCurrency: string;
};

const SignUp = () => {
  const { setUserInfo } = useContext(UserContext);
  const signUpMutation = useSignUp();
  const [userHasBeenCreated, setUserHasBeenCreated] = useState(false);
  const fiatCurrenciesQuery = useGetFiatCurrencies();

  const formik = useFormik<SignUpFormValues>({
    initialValues: {
      email: '',
      password: '',
      confirmPassword: '',
      fiatCurrency: 'USD',
    },

    validationSchema: Yup.object({
      email: Yup.string().email('Email must be a valid email').required('Required'),
      password: passwordSchema,
      confirmPassword: Yup.string()
        .required('Required')
        .oneOf([Yup.ref('password')], 'Passwords must match'),
      fiatCurrency: Yup.string().required('Required'),
    }),

    onSubmit: async (values) => {
      await signUpMutation.mutateAsync(values, {
        onSuccess: (createUserResult) => {
          const userInfo: UserInfo = {
            userId: createUserResult.user.id,
            fiatCurrency: createUserResult.user.fiatCurrencyTypeName,
            token: createUserResult.accessToken,
          };
          setUserInfo(userInfo);
          setUserHasBeenCreated(true);
        },
      });
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
          required
          {...formik.getFieldProps('email')}
          helperText={formik.touched.email && formik.errors.email}
          error={formik.touched.email && Boolean(formik.errors.email)}
        />
        <TextField
          fullWidth
          label="Password"
          size="small"
          type="password"
          required
          {...formik.getFieldProps('password')}
          helperText={formik.touched.password && formik.errors.password}
          error={formik.touched.password && Boolean(formik.errors.password)}
        />
        <TextField
          fullWidth
          label="Confirm Password"
          size="small"
          type="password"
          required
          {...formik.getFieldProps('confirmPassword')}
          helperText={formik.touched.confirmPassword && formik.errors.confirmPassword}
          error={formik.touched.confirmPassword && Boolean(formik.errors.confirmPassword)}
        />
        <Autocomplete
          options={(fiatCurrenciesQuery.data ?? []).map((x) => x.name)}
          fullWidth
          disabled={fiatCurrenciesQuery.isPending}
          renderInput={(params) => (
            <TextField
              {...params}
              label="Fiat Currency"
              name="fiatCurrency"
              required
              helperText={formik.touched.fiatCurrency && formik.errors.fiatCurrency}
              error={formik.touched.fiatCurrency && Boolean(formik.errors.fiatCurrency)}
            />
          )}
          size="small"
          value={formik.values.fiatCurrency}
          onChange={(event, value) => formik.setFieldValue('fiatCurrency', value)}
          onBlur={formik.handleBlur}
          popupIcon={fiatCurrenciesQuery.isPending ? <CircularProgress size={16} /> : <ArrowDropDown />}
        />
        <Button
          variant="contained"
          fullWidth
          type="submit"
          loading={signUpMutation.isPending}
          disabled={!formik.isValid || !formik.dirty}
        >
          Create Account
        </Button>
        <MuiLink component={ReactRouterLink} to={routePaths.login}>
          Already have an account? Log in
        </MuiLink>
      </Box>
    </Box>
  );
};

export default SignUp;
