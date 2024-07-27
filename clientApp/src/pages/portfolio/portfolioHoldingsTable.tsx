import { MRT_ColumnDef, MaterialReactTable, createMRTColumnHelper, useMaterialReactTable } from 'material-react-table';
import { AggregatedAsset, Portfolio as PortfolioDetails } from '../../api/generatedSdk';
import { FC, useContext } from 'react';
import { formatAsCurrency } from '../../utils/formatAsCurrency';
import { UserContext } from '../../contexts/UserContext';
import { Box, LinearProgress } from '@mui/material';
import { formatAsPercent } from '../../utils/formatAsPercent';

type Props = {
  portfolio: PortfolioDetails;
};

const PortfolioHoldingsTable: FC<Props> = ({ portfolio }) => {
  const { userInfo } = useContext(UserContext);

  const columnHelper = createMRTColumnHelper<AggregatedAsset>();
  const columns: MRT_ColumnDef<AggregatedAsset, any>[] = [
    columnHelper.accessor('cryptoTicker', {
      header: 'Asset',
      size: 100,
    }),
    columnHelper.accessor('portfolioPercentage', {
      header: 'Allocation',
      Cell: ({ cell }) => {
        const portfolioPercentage = parseFloat(cell.getValue<number>().toFixed(4));
        return (
          <Box sx={{ width: '80px' }}>
            <span>{formatAsPercent(portfolioPercentage)}</span>
            <LinearProgress variant="determinate" value={portfolioPercentage * 100} />
          </Box>
        );
      },
    }),
    columnHelper.accessor('totalCoinsOwned', {
      header: 'Amount',
      Cell: ({ cell, row }) => (
        <span>
          {cell.getValue<number>().toFixed(5)} {row.original.cryptoTicker}
        </span>
      ),
    }),
    columnHelper.accessor('totalInvestedIncludingFees', {
      header: 'Investment',
      Cell: ({ cell }) => <span>{formatAsCurrency(cell.getValue<number>(), userInfo?.fiatCurrency)}</span>,
      Footer: () => <span>{formatAsCurrency(portfolio.totalInvested, userInfo?.fiatCurrency)}</span>,
    }),

    columnHelper.accessor('currentValue', {
      header: 'Value',
      Cell: ({ cell }) => <span>{formatAsCurrency(cell.getValue<number>(), userInfo?.fiatCurrency)}</span>,
      Footer: () => <span>{formatAsCurrency(portfolio.currentValue, userInfo?.fiatCurrency)}</span>,
    }),

    columnHelper.accessor('currentPrice', {
      header: 'Price',
      Cell: ({ cell }) => <span>{formatAsCurrency(cell.getValue<number>(), userInfo?.fiatCurrency)}</span>,
    }),

    columnHelper.accessor('costBasis', {
      header: 'Cost Basis',
      Cell: ({ cell }) => <span>{formatAsCurrency(cell.getValue<number>(), userInfo?.fiatCurrency)}</span>,
    }),
  ];

  const table = useMaterialReactTable({
    columns,
    data: portfolio.holdings,
    enableStickyHeader: true,
    enableGlobalFilter: false,
    enableColumnActions: false,
    enablePagination: false,
    enableRowVirtualization: true,
    enableTopToolbar: false,
    enableBottomToolbar: false,
    defaultColumn: {
      size: 120,
    },
    muiTableContainerProps: {
      sx: {
        maxHeight: '80vh',
      },
    },
    initialState: {
      density: 'compact',
    },
  });

  return <MaterialReactTable table={table} />;
};

export default PortfolioHoldingsTable;
