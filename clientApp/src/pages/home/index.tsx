import { Box, Button } from '@mui/material';
import { PageTitle } from '../../components/pageTitle';
import { useState } from 'react';
import ImportTransactionsDialog from './importTransactionsDialog/importTransactionsDialog';
import { transactionsApi } from '../../api';
import { TransactionsTable, transactionsQueryKey } from './transactionsTable';
import { useQuery, useQueryClient } from '@tanstack/react-query';

const Home = () => {
  const [showImportTransactionsDialog, setShowImportTransactionsDialog] = useState(false);
  const queryClient = useQueryClient();
  const transactedCryptosQuery = useQuery({
    queryKey: ['transactedCryptos'],
    queryFn: () => transactionsApi.getTransactedCryptos(),
    meta: {
      errorMessage: 'Transacted cryptos could not be loaded',
    },
  });

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
      <TransactionsTable transactedCryptos={transactedCryptosQuery.data ?? []} />
    </>
  );
};

export default Home;
