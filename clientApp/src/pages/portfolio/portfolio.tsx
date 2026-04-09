import { useContext } from 'react';
import { UserContext } from '../../contexts/UserContext';
import PortfolioHoldingsTable from './portfolioHoldingsTable';
import PageLoader from '../../components/pageLoader';
import { PageTitle } from '../../components/pageTitle';
import { useGetPortfolio } from './queries/useGetPortfolio';

const Portfolio = () => {
  const { userInfo } = useContext(UserContext);
  const portfolioQuery = useGetPortfolio(userInfo!.userId);
  return (
    <>
      <PageTitle>Portfolio</PageTitle>
      {portfolioQuery.isLoading && <PageLoader />}
      {portfolioQuery.data && <PortfolioHoldingsTable portfolio={portfolioQuery.data} />}
    </>
  );
};

export default Portfolio;
