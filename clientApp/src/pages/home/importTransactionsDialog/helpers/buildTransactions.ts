import { SingleTransaction } from '../../../../api/generatedSdk';
import { TransactionType } from '../../../../api/types';
import { MapColumnsFormValues } from '../mapColumnsStep';
import { CsvRecord } from './parseCsvFile';
import { DateTime } from 'luxon';

export const buildTransactions = (
  csvRecords: CsvRecord[],
  mappingConfig: MapColumnsFormValues
): SingleTransaction[] => {
  return csvRecords.map((record) => ({
    date: formatDateString(record[mappingConfig.dateColumnName ?? ''] ?? ''),
    quantityTransacted: convertToNumber(record[mappingConfig.quantityTransactedColumnName ?? '']),
    price: parseFloat(record[mappingConfig.priceColumnName ?? ''] ?? '0'),
    // TODO: remove hardcoding of currency and store it as a field on the user's table instead of on each transaction
    priceCurrency: 'USD',
    fee: convertToNumber(record[mappingConfig.priceColumnName ?? '']),
    transactionType: getTransactionType(record[mappingConfig.transactionTypeColumnName ?? ''] ?? ''),
    numberOfCoinsSold: convertToNumber(record[mappingConfig.numberOfCoinsSoldColumnName ?? '']),
    ...(mappingConfig.exchangeColumnName && {
      exchange: record[mappingConfig.exchangeColumnName ?? ''] ?? '',
    }),
    ...(mappingConfig.notesColumnName && {
      notes: record[mappingConfig.notesColumnName ?? ''] ?? '',
    }),
  }));
};

/** Returns the date in YYYY-MM-DD format . If the given date is not valid then an error will be thrown */
const formatDateString = (date: string): string => {
  const dateFromISOFormat = DateTime.fromISO(date);
  if (dateFromISOFormat.isValid) return dateFromISOFormat.toISODate()!;

  const dateFromMonthDayYearFormat = DateTime.fromFormat(date, 'MM/dd/yyyy');
  if (dateFromMonthDayYearFormat.isValid) return dateFromMonthDayYearFormat.toISODate()!;

  throw new Error(`Date ${date} could not be parsed`);
};

const convertToNumber = (value?: string): number => {
  if (value) return parseFloat(value);
  return 0;
};

const getTransactionType = (transactionType: string): TransactionType => {
  transactionType = transactionType.toLowerCase().trim();
  if (['purchase', 'buy'].includes(transactionType)) return TransactionType.Purchase;
  if (['sale', 'sell'].includes(transactionType)) return TransactionType.Sale;
  if (['reward', 'staking reward', 'gift'].includes(transactionType)) return TransactionType.Reward;

  throw new Error(`Transaction type ${transactionType} is invalid`);
};
