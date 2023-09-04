import { Box, Button } from '@mui/material';
import { PageTitle } from '../../components/pageTitle';
import { useEffect, useState } from 'react';
import ImportTransactionsDialog from './importTransactionsDialog/importTransactionsDialog';
import PageLoader from '../../components/pageLoader';
import { transactionsApi } from '../../api';
import { MRT_ColumnFiltersState, MRT_PaginationState, MRT_SortingState } from 'material-react-table';
import { TransactionDTOPaginationResult } from '../../api/generatedSdk';
import { TransactionsTable } from './transactionsTable';
import { toast } from 'react-toastify';
import { buildSortByString } from '../../utils/builtSortByString';

const Home = () => {
  const [showImportTransactionsDialog, setShowImportTransactionsDialog] = useState(false);
  const [transactionsAreLoading, setTransactionsAreLoading] = useState(false);
  const [transactedCryptosAreLoading, setTransactedCryptosAreLoading] = useState(false);
  const [transactedCryptos, setTransactedCryptos] = useState<string[]>([]);
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
  const [transactionsPaginationResult, setTransactionsPaginationResult] =
    useState<TransactionDTOPaginationResult | null>(null);

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
    void handleLoadingOfTransactions(pagination, sorting, columnFilters);
    void loadTransactedCryptos();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const onTransactionsImported = async () => {
    setTransactionsAreLoading(true);
    await handleLoadingOfTransactions(pagination, sorting, columnFilters);
    setTransactionsAreLoading(false);
  };

  const onTableStateChange = async (
    pagination: MRT_PaginationState,
    sorting: MRT_SortingState,
    filters: MRT_ColumnFiltersState
  ) => {
    setPagination(pagination), setSorting(sorting);
    setFilters(filters);
    await handleLoadingOfTransactions(pagination, sorting, filters);
  };

  const handleLoadingOfTransactions = async (
    pagination: MRT_PaginationState,
    sorting: MRT_SortingState,
    filters: MRT_ColumnFiltersState
  ) => {
    setTransactionsAreLoading(true);
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
      {transactedCryptosAreLoading || (transactionsAreLoading && !transactionsPaginationResult) ? (
        <PageLoader />
      ) : (
        <>
          {transactionsPaginationResult && (
            <TransactionsTable
              transactionsPaginationResult={transactionsPaginationResult}
              isLoading={transactionsAreLoading}
              transactedCryptos={transactedCryptos}
              pagination={pagination}
              sorting={sorting}
              columnFilters={columnFilters}
              onTableStateChange={onTableStateChange}
            />
          )}
        </>
      )}
    </>
  );
};

export default Home;
