import { useMutation } from '@tanstack/react-query';
import { ResetPasswordStepTwoRequest } from '../../../api/generatedSdk';
import { usersApi } from '../../../api';
import { toast } from 'react-toastify';
import { ResetPasswordStepTwoFormValues } from '..';

export const useResetPasswordStepTwo = () => {
  return useMutation({
    mutationFn: async (formValues: ResetPasswordStepTwoFormValues) => {
      const request: ResetPasswordStepTwoRequest = {
        resetPasswordToken: formValues.token ?? '',
        newPassword: formValues.password,
        confirmedNewPassword: formValues.confirmNewPassword,
      };
      await usersApi.resetPasswordStepTwo({
        resetPasswordStepTwoRequest: request,
      });
    },
    onError: () => {
      toast.error('Password could not be reset');
    },
  });
};
