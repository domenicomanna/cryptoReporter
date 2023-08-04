import { configuration } from './configuration';
import { TransactionsApi, UsersApi } from './generatedSdk';

export const usersApi = new UsersApi(configuration);
export const transactionsApi = new TransactionsApi(configuration);
