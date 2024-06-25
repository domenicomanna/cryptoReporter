import {
  createMRTColumnHelper,
  MaterialReactTable,
  MRT_ColumnDef,
  MRT_ColumnFiltersState,
  MRT_PaginationState,
  MRT_SortingState,
  useMaterialReactTable,
} from 'material-react-table';
import { Transaction } from '../../api/generatedSdk';
import { FC, useContext, useRef, useState } from 'react';
import { DateTime } from 'luxon';
import { formatAsCurrency } from '../../utils/formatAsCurrency';
import { UserContext } from '../../contexts/UserContext';
import { transactionsApi } from '../../api';
import { buildSortByString } from '../../utils/builtSortByString';
import { keepPreviousData, useQuery } from '@tanstack/react-query';

type Props = {
  transactedCryptos: string[];
};

export const transactionsQueryKey = 'transactions-table-data';

export const TransactionsTable: FC<Props> = ({ transactedCryptos }) => {
  const { userInfo } = useContext(UserContext);

  const [pagination, setPagination] = useState<MRT_PaginationState>({
    pageIndex: 0,
    pageSize: 50,
  });

  const [sorting, setSorting] = useState<MRT_SortingState>([
    {
      id: 'date',
      desc: false,
    },
  ]);
  const [columnFilters, setFilters] = useState<MRT_ColumnFiltersState>([]);
  const tableContainerRef = useRef<HTMLDivElement>(null);

  const transactionsQuery = useQuery({
    queryKey: [transactionsQueryKey, pagination, sorting, columnFilters],
    queryFn: async () => {
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

  const columnHelper = createMRTColumnHelper<Transaction>();

  const columns: MRT_ColumnDef<Transaction, any>[] = [
    columnHelper.accessor('date', {
      header: 'Date',
      Cell: ({ cell }) => {
        return <span>{DateTime.fromISO(cell.getValue<string>()).toLocaleString(DateTime.DATE_SHORT)}</span>;
      },
      enableColumnFilter: false,
    }),
    columnHelper.accessor('cryptoTicker', {
      header: 'Coin',
      filterVariant: 'multi-select',
      filterSelectOptions: transactedCryptos,
    }),
    columnHelper.accessor('quantityTransacted', {
      header: 'Quantity Transacted',
      Cell: ({ cell }) => {
        return <span>{formatAsCurrency(cell.getValue<number>(), userInfo?.fiatCurrency)}</span>;
      },
      enableColumnFilter: false,
    }),
    columnHelper.accessor('price', {
      header: 'Price',
      Cell: ({ cell }) => {
        return <span>{formatAsCurrency(cell.getValue<number>(), userInfo?.fiatCurrency)}</span>;
      },
      enableColumnFilter: false,
    }),
    columnHelper.accessor('fee', {
      header: 'Fee',
      Cell: ({ cell }) => {
        return <span>{formatAsCurrency(cell.getValue<number>(), userInfo?.fiatCurrency)}</span>;
      },
      enableColumnFilter: false,
    }),
    columnHelper.accessor('coinsTransacted', {
      header: 'Coins Transacted',
      Cell: ({ cell }) => {
        return <span>{parseFloat(cell.getValue<number>().toFixed(4))}</span>;
      },
      enableSorting: false,
      enableColumnFilter: false,
    }),
    columnHelper.accessor('transactionType', {
      header: 'Transaction Type',
      filterVariant: 'multi-select',
      filterSelectOptions: ['Purchase', 'Reward', 'Sale'],
    }),
    columnHelper.accessor('exchange', {
      header: 'Exchange',
      enableColumnFilter: false,
    }),
    columnHelper.accessor('numberOfCoinsSold', {
      header: 'Number of Coins Sold',
      enableColumnFilter: false,
    }),
    columnHelper.accessor('notes', {
      header: 'Notes',
      enableColumnFilter: false,
    }),
  ];

  const table = useMaterialReactTable({
    columns,
    data: transactionsQuery.data?.records ?? [],
    enableStickyHeader: true,
    enableGlobalFilter: false,
    enableColumnActions: false,
    manualPagination: true,
    manualSorting: true,
    manualFiltering: true,
    rowCount: transactionsQuery.data?.totalRecordCount ?? 0,
    onPaginationChange: setPagination,
    onSortingChange: setSorting,
    onColumnFiltersChange: setFilters,
    muiTableContainerProps: {
      sx: {
        maxHeight: '70vh',
      },
      ref: tableContainerRef,
    },
    initialState: {
      density: 'compact',
      showColumnFilters: true,
    },
    state: {
      pagination,
      sorting,
      columnFilters,
      isLoading: transactionsQuery.isLoading,
      showProgressBars: transactionsQuery.isRefetching,
    },
  });

  return <MaterialReactTable table={table} />;
};
