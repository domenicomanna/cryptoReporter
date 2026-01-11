import { useContext, useState } from 'react';
import { Link as ReactRouterLink, Navigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { routePaths } from '../../constants/routePaths';
import { useFormik } from 'formik';
import * as Yup from 'yup';
import { CreateUserRequest, ResponseError } from '../../api/generatedSdk';
import { fiatCurrenciesApi, usersApi } from '../../api';
import { UserContext, UserInfo } from '../../contexts/UserContext';
import { Box, TextField, Link as MuiLink, Autocomplete, CircularProgress, Button } from '@mui/material';
import { PageTitle } from '../../components/pageTitle';
import { passwordSchema } from '../../validationSchemas/password';
import { ProblemDetails } from '../../api/types';
import { ArrowDropDown } from '@mui/icons-material';
import { useQuery, useMutation } from '@tanstack/react-query';

type FormValues = {
  email: string;
  password: string;
  confirmPassword: string;
  fiatCurrency: string;
};

const SignUp = () => {
  const { setUserInfo } = useContext(UserContext);
  const [userHasBeenCreated, setUserHasBeenCreated] = useState(false);

  const fiatCurrenciesQuery = useQuery({
    queryKey: ['fiatCurrencies'],
    queryFn: () => fiatCurrenciesApi.getFiatCurrencies(),
    meta: {
      errorMessage: 'Currencies could not be loaded',
    },
  });

  const formik = useFormik<FormValues>({
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
      await signUpMutation.mutateAsync(values);
    },
  });

  const signUpMutation = useMutation({
    mutationFn: async (values: FormValues) => {
      const createUserRequest: CreateUserRequest = {
        email: values.email,
        password: values.password,
        confirmedPassword: values.confirmPassword,
        fiatCurrencyType: values.fiatCurrency,
      };
      const result = await usersApi.createUser({ createUserRequest });
      const userInfo: UserInfo = {
        userId: result.user.id,
        fiatCurrency: result.user.fiatCurrencyTypeName,
        token: result.accessToken,
      };
      setUserInfo(userInfo);
      setUserHasBeenCreated(true);
    },
    onError: async (error) => {
      const errorDetails = error instanceof ResponseError ? ((await error.response.json()) as ProblemDetails) : null;
      const errorMessage = errorDetails?.detail ?? errorDetails?.title ?? 'Account could not be created';
      toast.error(errorMessage);
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
