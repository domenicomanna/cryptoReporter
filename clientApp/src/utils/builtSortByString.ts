import { MRT_SortingState } from 'material-react-table';

export const buildSortByString = (sortState: MRT_SortingState): string => {
  return sortState.map((column) => `${column.id} ${column.desc ? 'desc' : 'asc'}`).join(', ');
};
