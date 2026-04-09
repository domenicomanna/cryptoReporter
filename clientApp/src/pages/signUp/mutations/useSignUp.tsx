import { useMutation } from '@tanstack/react-query';
import { CreateUserRequest, ResponseError } from '../../../api/generatedSdk';
import { usersApi } from '../../../api';
import { ProblemDetails } from '../../../api/types';
import { toast } from 'react-toastify';
import { SignUpFormValues } from '..';

export const useSignUp = () => {
  return useMutation({
    mutationFn: async (formValues: SignUpFormValues) => {
      const createUserRequest: CreateUserRequest = {
        email: formValues.email,
        password: formValues.password,
        confirmedPassword: formValues.confirmPassword,
        fiatCurrencyType: formValues.fiatCurrency,
      };
      const result = await usersApi.createUser({ createUserRequest });
      return result;
    },
    onError: async (error) => {
      const errorDetails = error instanceof ResponseError ? ((await error.response.json()) as ProblemDetails) : null;
      const errorMessage = errorDetails?.detail ?? errorDetails?.title ?? 'Account could not be created';
      toast.error(errorMessage);
    },
  });
};
