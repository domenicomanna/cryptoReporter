import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
// import App from './App';
import '@fontsource/roboto/300.css';
import '@fontsource/roboto/400.css';
import '@fontsource/roboto/500.css';
import '@fontsource/roboto/700.css';
import 'react-toastify/dist/ReactToastify.css';
import { StyledEngineProvider, ThemeProvider } from '@mui/material';
import { RouterProvider } from 'react-router-dom';
import { ToastContainer, toast } from 'react-toastify';
import UserContextProvider from './contexts/UserContext';
import { theme } from './constants/theme';
import { router } from './router';
import { QueryCache, QueryClient, QueryClientProvider } from '@tanstack/react-query';

const root = ReactDOM.createRoot(document.getElementById('root') as HTMLElement);

const queryClient = new QueryClient({
  queryCache: new QueryCache({
    onError: (error, query) => {
      if (query.meta?.errorMessage) {
        toast.error(query.meta.errorMessage as string);
      }
    },
  }),
});

root.render(
  <React.StrictMode>
    <ThemeProvider theme={theme}>
      <StyledEngineProvider injectFirst>
        <QueryClientProvider client={queryClient}>
          <UserContextProvider>
            <RouterProvider router={router} />
          </UserContextProvider>
        </QueryClientProvider>
        <ToastContainer />
      </StyledEngineProvider>
    </ThemeProvider>
  </React.StrictMode>
);
