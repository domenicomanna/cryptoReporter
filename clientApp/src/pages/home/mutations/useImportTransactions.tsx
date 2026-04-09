import { useMutation } from '@tanstack/react-query';
import { AddTransactionsRequest, SingleTransaction } from '../../../api/generatedSdk';
import { toast } from 'react-toastify';
import { transactionsApi } from '../../../api';

export const useImportTransactions = () => {
  return useMutation({
    mutationFn: async (args: { transactions: SingleTransaction[]; deleteExistingTransactions: boolean }) => {
      const addTransactionsRequest: AddTransactionsRequest = {
        transactions: args.transactions,
        deleteExistingTransactions: args.deleteExistingTransactions,
      };
      await transactionsApi.addTransactions({ addTransactionsRequest });
    },
    onError: () => {
      toast.error('Transactions could not be imported');
    },
  });
};
