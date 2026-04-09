import { keepPreviousData, useQuery } from '@tanstack/react-query';
import { MRT_ColumnFiltersState, MRT_PaginationState, MRT_SortingState } from 'material-react-table';
import { transactionsApi } from '../../../api';
import { buildSortByString } from '../../../utils/builtSortByString';

export const transactionsQueryKey = 'transactions';

export const useGetTransactions = (args: {
  pagination: MRT_PaginationState;
  sorting: MRT_SortingState;
  columnFilters: MRT_ColumnFiltersState;
}) => {
  return useQuery({
    queryKey: [transactionsQueryKey, args.pagination, args.sorting, args.columnFilters],
    queryFn: async () => {
      const { pagination, sorting, columnFilters } = args;
      const transactionTypesFilter = columnFilters.find((x) => x.id === 'transactionType')?.value as
        | string[]
        | undefined;
      const cryptoTickersFilter = columnFilters.find((x) => x.id === 'cryptoTicker')?.value as string[] | undefined;
      const paginationResult = await transactionsApi.getTransactions({
        pageIndex: pagination.pageIndex,
        pageSize: pagination.pageSize,
        ...(sorting.length > 0 && { sortBy: buildSortByString(sorting) }),
        ...(transactionTypesFilter && { transactionTypes: transactionTypesFilter.join(',') }),
        ...(cryptoTickersFilter && { cryptoTickers: cryptoTickersFilter.join(',') }),
      });
      return paginationResult;
    },
    meta: {
      errorMessage: 'Transactions could not be loaded',
    },
    placeholderData: keepPreviousData,
  });
};
