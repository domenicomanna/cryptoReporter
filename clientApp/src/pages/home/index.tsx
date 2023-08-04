import { Box, Button } from '@mui/material';
import { PageTitle } from '../../components/pageTitle';
import { useState } from 'react';
import ImportTransactionsDialog from './importTransactionsDialog/importTransactionsDialog';
import PageLoader from '../../components/pageLoader';

const Home = () => {
  const [showImportTransactionsDialog, setShowImportTransactionsDialog] = useState(false);
  const [transactionsAreLoading, setTransactionsAreLoading] = useState(false);

  const onTransactionsImported = async () => {
    setTransactionsAreLoading(true);
    await new Promise((r) => setTimeout(r, 2000));
    setTransactionsAreLoading(false);
  };

  return (
    <>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
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
      {transactionsAreLoading ? <PageLoader /> : <></>}
    </>
  );
};

export default Home;
