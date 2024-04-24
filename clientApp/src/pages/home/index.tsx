import { Box, Button } from '@mui/material';
import { PageTitle } from '../../components/pageTitle';
import { useEffect, useState } from 'react';
import ImportTransactionsDialog from './importTransactionsDialog/importTransactionsDialog';
import PageLoader from '../../components/pageLoader';
import { transactionsApi } from '../../api';
import { TransactionPaginationResult } from '../../api/generatedSdk';
import { TransactionsTable, defaultPagination, defaultSorting } from './transactionsTable';
import { toast } from 'react-toastify';
import { buildSortByString } from '../../utils/builtSortByString';

const Home = () => {
  const [showImportTransactionsDialog, setShowImportTransactionsDialog] = useState(false);
  const [transactionsAreLoading, setTransactionsAreLoading] = useState(false);
  const [transactedCryptosAreLoading, setTransactedCryptosAreLoading] = useState(false);
  const [transactedCryptos, setTransactedCryptos] = useState<string[]>([]);
  const [transactionsPaginationResult, setTransactionsPaginationResult] = useState<TransactionPaginationResult | null>(
    null
  );

  useEffect(() => {
    const loadTransactedCryptos = async () => {
      try {
        setTransactedCryptosAreLoading(true);
        const transactedCryptos = await transactionsApi.getTransactedCryptos();
        setTransactedCryptos(transactedCryptos);
      } catch (error) {
        toast.error('Transacted cryptos could not be loaded');
      } finally {
        setTransactedCryptosAreLoading(false);
      }
    };
    void loadTransactedCryptos();
  }, []);

  const onTransactionsImported = async () => {
    setTransactionsPaginationResult(null);
    await loadTransactions();
  };

  useEffect(() => {
    void loadTransactions();
  }, []);

  const loadTransactions = async () => {
    try {
      setTransactionsAreLoading(true);
      const paginationResult = await transactionsApi.getTransactions({
        pageIndex: defaultPagination.pageIndex,
        pageSize: defaultPagination.pageSize,
        sortBy: buildSortByString(defaultSorting),
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
      {transactedCryptosAreLoading || transactionsAreLoading ? (
        <PageLoader />
      ) : (
        <>
          {transactionsPaginationResult && (
            <TransactionsTable
              transactionsPaginationResult={transactionsPaginationResult}
              transactedCryptos={transactedCryptos}
            />
          )}
        </>
      )}
    </>
  );
};

export default Home;
