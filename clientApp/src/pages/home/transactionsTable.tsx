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
import { Dispatch, FC, SetStateAction, useContext, useRef } from 'react';
import { DateTime } from 'luxon';
import { formatAsCurrency } from '../../utils/formatAsCurrency';
import { UserContext } from '../../contexts/UserContext';

type Props = {
  records: Transaction[];
  totalRowCount: number;
  transactedCryptos: string[];
  isLoading: boolean;
  isRefetching: boolean;
  pagination: MRT_PaginationState;
  sorting: MRT_SortingState;
  columnFilters: MRT_ColumnFiltersState;
  onPaginationChange: Dispatch<SetStateAction<MRT_PaginationState>>;
  onSortingChange: Dispatch<SetStateAction<MRT_SortingState>>;
  onColumnFiltersChange: Dispatch<SetStateAction<MRT_ColumnFiltersState>>;
};

export const TransactionsTable: FC<Props> = ({
  records,
  totalRowCount,
  transactedCryptos,
  isLoading,
  isRefetching,
  pagination,
  sorting,
  columnFilters,
  onPaginationChange,
  onSortingChange,
  onColumnFiltersChange,
}) => {
  const { userInfo } = useContext(UserContext);
  const tableContainerRef = useRef<HTMLDivElement>(null);

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
    columnHelper.accessor('amountTransacted', {
      header: 'Amount Transacted',
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
    data: records,
    enableStickyHeader: true,
    enableGlobalFilter: false,
    enableColumnActions: false,
    manualPagination: true,
    manualSorting: true,
    manualFiltering: true,
    rowCount: totalRowCount,
    onPaginationChange: onPaginationChange,
    onSortingChange: onSortingChange,
    onColumnFiltersChange: onColumnFiltersChange,
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
      isLoading,
      showProgressBars: isRefetching,
    },
  });

  return <MaterialReactTable table={table} />;
};
