import { UserInfo } from '../contexts/UserContext';

const _storageKey = 'user';

export const userStorageHelper = {
  storeUserInfo: (user: UserInfo) => {
    localStorage.setItem(_storageKey, JSON.stringify(user));
  },
  getCurrentUserInfo: (): UserInfo | null => {
    const userInfo = localStorage.getItem(_storageKey);
    if (!userInfo) return null;
    return JSON.parse(userInfo);
  },
  removeCurrentUserInfo: () => {
    localStorage.removeItem(_storageKey);
  },
};
