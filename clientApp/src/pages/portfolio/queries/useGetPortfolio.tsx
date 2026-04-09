import { useQuery } from '@tanstack/react-query';
import { usersApi } from '../../../api';

export const useGetPortfolio = (userId: number) => {
  return useQuery({
    queryKey: ['portfolio'],
    queryFn: () =>
      usersApi.getPortfolio({
        userId: userId,
      }),
    meta: {
      errorMessage: 'Portfolio could not be loaded',
    },
  });
};
