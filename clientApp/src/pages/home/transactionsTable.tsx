import MaterialReactTable, {
  MRT_ColumnDef,
  MRT_ColumnFiltersState,
  MRT_PaginationState,
  MRT_SortingState,
} from 'material-react-table';
import { TransactionDTO, TransactionDTOPaginationResult } from '../../api/generatedSdk';
import { FC, useContext, useMemo } from 'react';
import { Updater } from '@tanstack/react-table';
import { DateTime } from 'luxon';
import { formatAsCurrency } from '../../utils/formatAsCurrency';
import { UserContext } from '../../contexts/UserContext';

type Props = {
  transactionsPaginationResult: TransactionDTOPaginationResult;
  isLoading: boolean;
  transactedCryptos: string[];
  pagination: MRT_PaginationState;
  sorting: MRT_SortingState;
  columnFilters: MRT_ColumnFiltersState;
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
  pagination,
  sorting,
  columnFilters,
  onTableStateChange,
}) => {
  const { userInfo } = useContext(UserContext);

  const handlePaginationChange = async (updater: Updater<MRT_PaginationState>) => {
    const updatedPagination = updater instanceof Function ? updater(pagination) : updater;
    onTableStateChange(updatedPagination, sorting, columnFilters);
  };

  const handleSortChange = async (updater: Updater<MRT_SortingState>) => {
    const updatedSorting = updater instanceof Function ? updater(sorting) : updater;
    const updatedPagination: MRT_PaginationState = {
      pageIndex: 0,
      pageSize: pagination.pageSize,
    };
    onTableStateChange(updatedPagination, updatedSorting, columnFilters);
  };

  const handleColumnsFiltersChange = async (updater: Updater<MRT_ColumnFiltersState>) => {
    const updatedFilters = updater instanceof Function ? updater(columnFilters) : updater;
    const updatedPagination: MRT_PaginationState = {
      pageIndex: 0,
      pageSize: pagination.pageSize,
    };
    onTableStateChange(updatedPagination, sorting, updatedFilters);
  };

  const columns = useMemo<MRT_ColumnDef<TransactionDTO>[]>(
    () => [
      {
        accessorKey: 'date',
        header: 'Date',
        Cell: ({ cell }) => {
          return <span>{DateTime.fromISO(cell.getValue<string>()).toLocaleString(DateTime.DATE_SHORT)}</span>;
        },
        enableColumnFilter: false,
      },
      {
        accessorKey: 'cryptoTicker',
        header: 'Coin',
        filterVariant: 'multi-select',
        filterSelectOptions: transactedCryptos,
      },
      {
        accessorKey: 'quantityTransacted',
        header: 'Quantity Transacted',
        Cell: ({ cell }) => {
          return <span>{formatAsCurrency(cell.getValue<number>(), userInfo?.fiatCurrency)}</span>;
        },
        enableColumnFilter: false,
      },
      {
        accessorKey: 'price',
        header: 'Price',
        Cell: ({ cell }) => {
          return <span>{formatAsCurrency(cell.getValue<number>(), userInfo?.fiatCurrency)}</span>;
        },
        enableColumnFilter: false,
      },
      {
        accessorKey: 'fee',
        header: 'Fee',
        Cell: ({ cell }) => {
          return <span>{formatAsCurrency(cell.getValue<number>(), userInfo?.fiatCurrency)}</span>;
        },
        enableColumnFilter: false,
      },
      {
        accessorKey: 'coinsTransacted',
        header: 'Coins Transacted',
        Cell: ({ cell }) => {
          return <span>{parseFloat(cell.getValue<number>().toFixed(4))}</span>;
        },
        enableSorting: false,
        enableColumnFilter: false,
      },
      {
        accessorKey: 'transactionType',
        header: 'Transaction Type',
        filterVariant: 'multi-select',
        filterSelectOptions: ['Purchase', 'Reward', 'Sale'],
      },
      {
        accessorKey: 'exchange',
        header: 'Exchange',
        enableColumnFilter: false,
      },
      {
        accessorKey: 'numberOfCoinsSold',
        header: 'Number of Coins Sold',
        enableColumnFilter: false,
      },
      {
        accessorKey: 'notes',
        header: 'Notes',
        enableColumnFilter: false,
      },
    ],
    [userInfo, transactedCryptos]
  );

  return (
    <MaterialReactTable
      columns={columns}
      data={transactionsPaginationResult.records}
      enableStickyHeader
      enableGlobalFilter={false}
      enableColumnActions={false}
      manualPagination
      manualSorting
      manualFiltering
      rowCount={transactionsPaginationResult.totalRecordCount}
      onPaginationChange={handlePaginationChange}
      onSortingChange={handleSortChange}
      onColumnFiltersChange={handleColumnsFiltersChange}
      muiTableContainerProps={{ sx: { maxHeight: '75vh' } }}
      initialState={{
        density: 'compact',
        showColumnFilters: true,
      }}
      state={{
        pagination,
        sorting,
        columnFilters,
        isLoading,
      }}
    />
  );
};
