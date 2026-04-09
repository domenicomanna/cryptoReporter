import { useMutation } from '@tanstack/react-query';
import { ResetPasswordStepOneRequest } from '../../../api/generatedSdk';
import { usersApi } from '../../../api';
import { toast } from 'react-toastify';

export const useResetPasswordStepOne = () => {
  return useMutation({
    mutationFn: async (email: string) => {
      const request: ResetPasswordStepOneRequest = {
        email,
      };
      await usersApi.resetPasswordStepOne({
        resetPasswordStepOneRequest: request,
      });
    },
    onError: () => {
      toast.error('Password could not be reset');
    },
  });
};
