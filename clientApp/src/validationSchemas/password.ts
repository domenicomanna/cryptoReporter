import { string } from 'yup';

export const passwordSchema = string().required('Required').min(6, 'Password must be at least 6 characters');
