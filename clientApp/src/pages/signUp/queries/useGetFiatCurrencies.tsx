import { useQuery } from '@tanstack/react-query';
import { fiatCurrenciesApi } from '../../../api';

export const useGetFiatCurrencies = () => {
  return useQuery({
    queryKey: ['fiatCurrencies'],
    queryFn: () => fiatCurrenciesApi.getFiatCurrencies(),
    meta: {
      errorMessage: 'Currencies could not be loaded',
    },
  });
};
