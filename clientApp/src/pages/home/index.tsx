import { Box, Button } from '@mui/material';
import { PageTitle } from '../../components/pageTitle';
import { useEffect, useState } from 'react';
import ImportTransactionsDialog from './importTransactionsDialog/importTransactionsDialog';
import PageLoader from '../../components/pageLoader';
import { transactionsApi } from '../../api';
import { MRT_PaginationState, MRT_SortingState } from 'material-react-table';
import { TransactionDTOPaginationResult } from '../../api/generatedSdk';
import { TransactionsTable } from './transactionsTable';
import { toast } from 'react-toastify';
import { buildSortByString } from '../../utils/builtSortByString';

const Home = () => {
  const [showImportTransactionsDialog, setShowImportTransactionsDialog] = useState(false);
  const [transactionsAreLoading, setTransactionsAreLoading] = useState(false);
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
  const [transactionsPaginationResult, setTransactionsPaginationResult] =
    useState<TransactionDTOPaginationResult | null>(null);

  useEffect(() => {
    void handleLoadingOfTransactions(pagination, sorting);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const onTransactionsImported = async () => {
    setTransactionsAreLoading(true);
    await handleLoadingOfTransactions(pagination, sorting);
    setTransactionsAreLoading(false);
  };

  const onPaginationChange = async (pagination: MRT_PaginationState) => {
    setPagination(pagination);
    await handleLoadingOfTransactions(pagination, sorting);
  };

  const onSortingChange = async (sorting: MRT_SortingState) => {
    setSorting(sorting);
    await handleLoadingOfTransactions(pagination, sorting);
  };

  const handleLoadingOfTransactions = async (pagination: MRT_PaginationState, sorting: MRT_SortingState) => {
    setTransactionsAreLoading(true);
    try {
      const paginationResult = await transactionsApi.getTransactions({
        pageIndex: pagination.pageIndex,
        pageSize: pagination.pageSize,
        ...(sorting.length > 0 && { sortBy: buildSortByString(sorting) }),
      });
      setTransactionsPaginationResult(paginationResult);
    } catch (error) {
      toast.error('Transactions could not be loaded');
    } finally {
      setTransactionsAreLoading(false);
    }
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
      {transactionsAreLoading && !transactionsPaginationResult ? (
        <PageLoader />
      ) : (
        <>
          {transactionsPaginationResult && (
            <TransactionsTable
              transactionsPaginationResult={transactionsPaginationResult}
              isLoading={transactionsAreLoading}
              pagination={pagination}
              onPaginationChange={onPaginationChange}
              sorting={sorting}
              onSortingChange={onSortingChange}
            />
          )}
        </>
      )}
    </>
  );
};

export default Home;
