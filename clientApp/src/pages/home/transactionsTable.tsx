import MaterialReactTable, { MRT_ColumnDef, MRT_PaginationState, MRT_SortingState } from 'material-react-table';
import { TransactionDTO, TransactionDTOPaginationResult } from '../../api/generatedSdk';
import { FC, useContext, useMemo } from 'react';
import { Updater } from '@tanstack/react-table';
import { DateTime } from 'luxon';
import { formatAsCurrency } from '../../utils/formatAsCurrency';
import { UserContext } from '../../contexts/UserContext';

type Props = {
  transactionsPaginationResult: TransactionDTOPaginationResult;
  isLoading: boolean;
  pagination: MRT_PaginationState;
  onPaginationChange: (pagination: MRT_PaginationState) => void;
  sorting: MRT_SortingState;
  onSortingChange: (sorting: MRT_SortingState) => void;
};

export const TransactionsTable: FC<Props> = ({
  transactionsPaginationResult,
  isLoading,
  pagination,
  onPaginationChange,
  sorting,
  onSortingChange,
}) => {
  const { userInfo } = useContext(UserContext);

  const handlePaginationChange = async (updater: Updater<MRT_PaginationState>) => {
    const updatedPagination = updater instanceof Function ? updater(pagination) : updater;
    onPaginationChange(updatedPagination);
  };

  const handleSortChange = async (updater: Updater<MRT_SortingState>) => {
    const updatedSorting = updater instanceof Function ? updater(sorting) : updater;
    onSortingChange(updatedSorting);
  };

  const columns = useMemo<MRT_ColumnDef<TransactionDTO>[]>(
    () => [
      {
        accessorKey: 'date',
        header: 'Date',
        Cell: ({ cell }) => {
          return <span>{DateTime.fromISO(cell.getValue<string>()).toLocaleString(DateTime.DATE_SHORT)}</span>;
        },
      },
      {
        accessorKey: 'cryptoTicker',
        header: 'Coin',
      },
      {
        accessorKey: 'quantityTransacted',
        header: 'Quantity Transacted',
        Cell: ({ cell }) => {
          return <span>{formatAsCurrency(cell.getValue<number>(), userInfo?.fiatCurrency)}</span>;
        },
      },
      {
        accessorKey: 'price',
        header: 'Price',
        Cell: ({ cell }) => {
          return <span>{formatAsCurrency(cell.getValue<number>(), userInfo?.fiatCurrency)}</span>;
        },
      },
      {
        accessorKey: 'fee',
        header: 'Fee',
        Cell: ({ cell }) => {
          return <span>{formatAsCurrency(cell.getValue<number>(), userInfo?.fiatCurrency)}</span>;
        },
      },
      {
        accessorKey: 'coinsTransacted',
        header: 'Coins Transacted',
        Cell: ({ cell }) => {
          return <span>{parseFloat(cell.getValue<number>().toFixed(4))}</span>;
        },
        enableSorting: false,
      },
      {
        accessorKey: 'transactionType',
        header: 'Transaction Type',
      },
      {
        accessorKey: 'exchange',
        header: 'Exchange',
      },
      {
        accessorKey: 'numberOfCoinsSold',
        header: 'Number of Coins Sold',
      },
      {
        accessorKey: 'notes',
        header: 'Notes',
      },
    ],
    [userInfo]
  );

  return (
    <MaterialReactTable
      columns={columns}
      data={transactionsPaginationResult.records}
      enableStickyHeader
      enableTopToolbar={false}
      enableColumnActions={false}
      manualPagination
      manualSorting
      rowCount={transactionsPaginationResult.totalRecordCount}
      onPaginationChange={handlePaginationChange}
      onSortingChange={handleSortChange}
      muiTableContainerProps={{ sx: { maxHeight: '75vh' } }}
      state={{
        density: 'compact',
        pagination,
        sorting,
        isLoading,
      }}
    />
  );
};
