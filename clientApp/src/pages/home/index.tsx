import { Box, Button } from '@mui/material';
import { PageTitle } from '../../components/pageTitle';
import { useState } from 'react';
import ImportTransactionsDialog from './importTransactionsDialog/importTransactionsDialog';
import { TransactionsTable } from './transactionsTable';
import { useQueryClient } from '@tanstack/react-query';
import { MRT_ColumnFiltersState, MRT_PaginationState, MRT_SortingState } from 'material-react-table';
import { transactionsQueryKey, useGetTransactions } from './queries/useGetTransactions';
import { useGetTransactedCryptos } from './queries/useGetTransactedCryptos';

const Home = () => {
  const [showImportTransactionsDialog, setShowImportTransactionsDialog] = useState(false);
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
  const [columnFilters, setColumnFilters] = useState<MRT_ColumnFiltersState>([]);
  const transactionsQuery = useGetTransactions({ pagination, sorting, columnFilters });

  const queryClient = useQueryClient();
  const transactedCryptosQuery = useGetTransactedCryptos();

  const onTransactionsImported = async () => {
    await queryClient.invalidateQueries({ queryKey: [transactionsQueryKey] });
  };

  return (
    <>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 1 }}>
        <PageTitle>Transactions</PageTitle>
        <Button onClick={() => setShowImportTransactionsDialog(true)} variant="outlined">
          Import Transactions
        </Button>
        {showImportTransactionsDialog && (
          <ImportTransactionsDialog
            onTransactionsImported={onTransactionsImported}
            onCloseDialog={() => setShowImportTransactionsDialog(false)}
          />
        )}
      </Box>
      <TransactionsTable
        transactedCryptos={transactedCryptosQuery.data ?? []}
        records={transactionsQuery.data?.records ?? []}
        totalRowCount={transactionsQuery.data?.totalRecordCount ?? 0}
        isLoading={transactionsQuery.isLoading}
        isRefetching={transactionsQuery.isRefetching}
        pagination={pagination}
        sorting={sorting}
        columnFilters={columnFilters}
        onPaginationChange={setPagination}
        onSortingChange={setSorting}
        onColumnFiltersChange={setColumnFilters}
      />
    </>
  );
};

export default Home;
