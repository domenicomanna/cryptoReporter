import { configuration } from './configuration';
import { FiatCurrenciesApi, TransactionsApi, UsersApi } from './generatedSdk';

export const usersApi = new UsersApi(configuration);
export const transactionsApi = new TransactionsApi(configuration);
export const fiatCurrenciesApi = new FiatCurrenciesApi(configuration);
