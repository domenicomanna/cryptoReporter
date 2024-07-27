import { usersApi } from '../../api';
import { useContext } from 'react';
import { UserContext } from '../../contexts/UserContext';
import PortfolioHoldingsTable from './portfolioHoldingsTable';
import { useQuery } from '@tanstack/react-query';
import PageLoader from '../../components/pageLoader';
import { PageTitle } from '../../components/pageTitle';

const Portfolio = () => {
  const { userInfo } = useContext(UserContext);

  const portfolioQuery = useQuery({
    queryKey: ['portfolio'],
    queryFn: () =>
      usersApi.getPortfolio({
        userId: userInfo!.userId,
      }),
    meta: {
      errorMessage: 'Portfolio could not be loaded',
    },
  });

  return (
    <>
      <PageTitle>Portfolio</PageTitle>
      {portfolioQuery.isLoading && <PageLoader />}
      {portfolioQuery.data && <PortfolioHoldingsTable portfolio={portfolioQuery.data} />}
    </>
  );
};

export default Portfolio;
