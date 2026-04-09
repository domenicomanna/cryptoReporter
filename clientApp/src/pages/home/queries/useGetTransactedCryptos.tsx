import { useQuery } from '@tanstack/react-query';
import { transactionsApi } from '../../../api';

export const useGetTransactedCryptos = () => {
  return useQuery({
    queryKey: ['transactedCryptos'],
    queryFn: () => transactionsApi.getTransactedCryptos(),
    meta: {
      errorMessage: 'Transacted cryptos could not be loaded',
    },
  });
};
