import { SxProps, Typography } from '@mui/material';
import { FC } from 'react';

type Props = {
  children: string;
  sx?: SxProps;
};

export const PageTitle: FC<Props> = ({ children, sx }) => {
  return (
    <Typography variant="h4" sx={sx}>
      {children}
    </Typography>
  );
};
