export type ProblemDetails = {
  detail?: string;
  status?: number;
  title?: string;
};

export enum TransactionType {
  Purchase = 'Purchase',
  Reward = 'Reward',
  Sale = 'Sale',
}
