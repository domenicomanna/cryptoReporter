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
  isLoading: boolean;
  transactedCryptos: string[];
  onTableStateChange: (
    pagination: MRT_PaginationState,
    sorting: MRT_SortingState,
    filters: MRT_ColumnFiltersState
  ) => void;
};

export const TransactionsTable: FC<Props> = ({
  transactionsPaginationResult,
  isLoading,
  transactedCryptos,
  onTableStateChange,
}) => {
  const { userInfo } = useContext(UserContext);

  const [pagination, setPagination] = useState<MRT_PaginationState>(defaultPagination);
  const [sorting, setSorting] = useState<MRT_SortingState>(defaultSorting);
  const [columnFilters, setFilters] = useState<MRT_ColumnFiltersState>([]);

  const handlePaginationChange = async (updater: Updater<MRT_PaginationState>) => {
    const updatedPagination = updater instanceof Function ? updater(pagination) : updater;
    setPagination(updatedPagination);
    onTableStateChange(updatedPagination, sorting, columnFilters);
  };

  const handleSortChange = async (updater: Updater<MRT_SortingState>) => {
    const updatedSorting = updater instanceof Function ? updater(sorting) : updater;
    const updatedPagination: MRT_PaginationState = {
      pageIndex: 0,
      pageSize: pagination.pageSize,
    };
    setPagination(updatedPagination);
    setSorting(updatedSorting);
    onTableStateChange(updatedPagination, updatedSorting, columnFilters);
  };

  const handleColumnsFiltersChange = async (updater: Updater<MRT_ColumnFiltersState>) => {
    const updatedFilters = updater instanceof Function ? updater(columnFilters) : updater;
    const updatedPagination: MRT_PaginationState = {
      pageIndex: 0,
      pageSize: pagination.pageSize,
    };
    setPagination(updatedPagination);
    setFilters(updatedFilters);
    onTableStateChange(updatedPagination, sorting, updatedFilters);
  };

  const columnHelper = createMRTColumnHelper<Transaction>(); //TS now knows the shape of your data

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
