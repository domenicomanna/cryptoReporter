import { baseApiUrl } from '.';
import { ReauthenticateWithRefreshTokenResult } from '../generatedSdk';

export const refreshToken = async (): Promise<ReauthenticateWithRefreshTokenResult> => {
  const response = await fetch(`${baseApiUrl}/api/users/refresh-token`, {
    method: 'POST',
    credentials: 'include',
  });
  if (!response.ok) throw new Error('Could not refresh token');
  const result = (await response.json()) as ReauthenticateWithRefreshTokenResult;
  return result;
};
