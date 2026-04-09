import { useMutation } from '@tanstack/react-query';
import { LoginFormValues } from '..';
import { LoginRequest } from '../../../api/generatedSdk';
import { usersApi } from '../../../api';
import { toast } from 'react-toastify';

export const useLogin = () => {
  return useMutation({
    mutationFn: async (formValues: LoginFormValues) => {
      const loginRequest: LoginRequest = {
        email: formValues.email,
        password: formValues.password,
      };
      const loginResult = await usersApi.login({ loginRequest });
      return loginResult;
    },
    onError: () => {
      toast.error('Invalid email or password', {
        toastId: 'invalidCredentials',
      });
    },
  });
};
