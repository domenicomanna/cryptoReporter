import { Box, Button } from '@mui/material';
import { PageTitle } from '../../components/pageTitle';
import { useEffect, useState } from 'react';
import ImportTransactionsDialog from './importTransactionsDialog/importTransactionsDialog';
import PageLoader from '../../components/pageLoader';
import { transactionsApi } from '../../api';
import { MRT_PaginationState } from 'material-react-table';
import { TransactionDTOPaginationResult } from '../../api/generatedSdk';
import { TransactionsTable } from './transactionsTable';
import { toast } from 'react-toastify';

const Home = () => {
  const [showImportTransactionsDialog, setShowImportTransactionsDialog] = useState(false);
  const [transactionsAreLoading, setTransactionsAreLoading] = useState(false);
  const [pagination, setPagination] = useState<MRT_PaginationState>({
    pageIndex: 0,
    pageSize: 50,
  });
  const [transactionsPaginationResult, setTransactionsPaginationResult] =
    useState<TransactionDTOPaginationResult | null>(null);

  useEffect(() => {
    void handleLoadingOfTransactions(pagination);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const onTransactionsImported = async () => {
    setTransactionsAreLoading(true);
    await handleLoadingOfTransactions(pagination);
    setTransactionsAreLoading(false);
  };

  const onPaginationChange = async (pagination: MRT_PaginationState) => {
    setPagination(pagination);
    await handleLoadingOfTransactions(pagination);
  };

  const handleLoadingOfTransactions = async (pagination: MRT_PaginationState) => {
    setTransactionsAreLoading(true);
    try {
      const paginationResult = await transactionsApi.getTransactions({
        pageIndex: pagination.pageIndex,
        pageSize: pagination.pageSize,
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
            />
          )}
        </>
      )}
    </>
  );
};

export default Home;
