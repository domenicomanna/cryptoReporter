import { Configuration } from '../generatedSdk';
import { middleware } from './middleware';

export const baseApiUrl = (import.meta.env.VITE_BASE_API_URL ?? 'http://localhost:5180') as string;

export const configuration: Configuration = new Configuration({
  basePath: baseApiUrl,
  middleware,
  credentials: 'include',
});
