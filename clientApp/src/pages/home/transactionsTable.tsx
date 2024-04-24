import {
  createMRTColumnHelper,
  MaterialReactTable,
  MRT_ColumnDef,
  MRT_ColumnFiltersState,
  MRT_PaginationState,
  MRT_SortingState,
  useMaterialReactTable,
} from 'material-react-table';
import { Transaction, TransactionPaginationResult } from '../../api/generatedSdk';
import { FC, useContext, useState } from 'react';
import { Updater } from '@tanstack/react-table';
import { DateTime } from 'luxon';
import { formatAsCurrency } from '../../utils/formatAsCurrency';
import { UserContext } from '../../contexts/UserContext';
import { transactionsApi } from '../../api';
import { buildSortByString } from '../../utils/builtSortByString';
import { toast } from 'react-toastify';

export const defaultPagination: MRT_PaginationState = {
  pageIndex: 0,
  pageSize: 50,
};

export const defaultSorting: MRT_SortingState = [
  {
    id: 'date',
    desc: false,
  },
];

type Props = {
  transactionsPaginationResult: TransactionPaginationResult;
  transactedCryptos: string[];
};

export const TransactionsTable: FC<Props> = ({
  transactionsPaginationResult: initialTransactionsPaginationResult,
  transactedCryptos,
}) => {
  const { userInfo } = useContext(UserContext);
  const [isLoading, setIsLoading] = useState(false);
  const [transactionsPaginationResult, setTransactionsPaginationResult] = useState<TransactionPaginationResult>(
    initialTransactionsPaginationResult
  );
  const [pagination, setPagination] = useState<MRT_PaginationState>(defaultPagination);
  const [sorting, setSorting] = useState<MRT_SortingState>(defaultSorting);
  const [columnFilters, setFilters] = useState<MRT_ColumnFiltersState>([]);

  const handlePaginationChange = async (updater: Updater<MRT_PaginationState>) => {
    const updatedPagination = updater instanceof Function ? updater(pagination) : updater;
    setPagination(updatedPagination);
    await onTableStateChange(updatedPagination, sorting, columnFilters);
  };

  const handleSortChange = async (updater: Updater<MRT_SortingState>) => {
    const updatedSorting = updater instanceof Function ? updater(sorting) : updater;
    const updatedPagination: MRT_PaginationState = {
      pageIndex: 0,
      pageSize: pagination.pageSize,
    };
    setPagination(updatedPagination);
    setSorting(updatedSorting);
    await onTableStateChange(updatedPagination, updatedSorting, columnFilters);
  };

  const handleColumnsFiltersChange = async (updater: Updater<MRT_ColumnFiltersState>) => {
    const updatedFilters = updater instanceof Function ? updater(columnFilters) : updater;
    const updatedPagination: MRT_PaginationState = {
      pageIndex: 0,
      pageSize: pagination.pageSize,
    };
    setPagination(updatedPagination);
    setFilters(updatedFilters);
    await onTableStateChange(updatedPagination, sorting, updatedFilters);
  };

  const onTableStateChange = async (
    pagination: MRT_PaginationState,
    sorting: MRT_SortingState,
    filters: MRT_ColumnFiltersState
  ) => {
    setIsLoading(true);
    try {
      const transactionTypesFilter = filters.find((x) => x.id === 'transactionType')?.value as string[] | undefined;
      const cryptoTickersFilter = filters.find((x) => x.id === 'cryptoTicker')?.value as string[] | undefined;
      const paginationResult = await transactionsApi.getTransactions({
        pageIndex: pagination.pageIndex,
        pageSize: pagination.pageSize,
        ...(sorting.length > 0 && { sortBy: buildSortByString(sorting) }),
        ...(transactionTypesFilter && { transactionTypes: transactionTypesFilter.join(',') }),
        ...(cryptoTickersFilter && { cryptoTickers: cryptoTickersFilter.join(',') }),
      });
      setTransactionsPaginationResult(paginationResult);
    } catch (error) {
      toast.error('Transactions could not be loaded');
    } finally {
      setIsLoading(false);
    }
  };

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
    data: transactionsPaginationResult.records,
    enableStickyHeader: true,
    enableGlobalFilter: false,
    enableColumnActions: false,
    manualPagination: true,
    manualSorting: true,
    manualFiltering: true,
    rowCount: transactionsPaginationResult.totalRecordCount,
    onPaginationChange: handlePaginationChange,
    onSortingChange: handleSortChange,
    onColumnFiltersChange: handleColumnsFiltersChange,
    muiTableContainerProps: {
      sx: {
        maxHeight: '70vh',
      },
    },
    initialState: {
      density: 'compact',
      showColumnFilters: true,
    },
    state: {
      pagination,
      sorting,
      columnFilters,
      isLoading,
    },
  });

  return <MaterialReactTable table={table} />;
};
