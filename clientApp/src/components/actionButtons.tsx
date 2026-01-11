import { Button, Grid } from '@mui/material';
import { FC } from 'react';

type Props = {
  confirmActionLabel?: string;
  denyActionLabel?: string;
  confirmButtonShouldBeDisabled?: boolean;
  denyButtonShouldBeDisabled?: boolean;
  actionIsBeingConfirmed?: boolean;
  onConfirmActionClick?: () => void;
  onDenyActionClick?: () => void;
};

const ActionButtons: FC<Props> = ({
  confirmActionLabel = 'Submit',
  denyActionLabel = 'Cancel',
  confirmButtonShouldBeDisabled,
  denyButtonShouldBeDisabled,
  actionIsBeingConfirmed,
  onConfirmActionClick,
  onDenyActionClick,
}) => {
  return (
    <Grid container spacing={1}>
      <Grid size={6}>
        <Button
          variant="outlined"
          color="info"
          disabled={denyButtonShouldBeDisabled || !onDenyActionClick}
          onClick={onDenyActionClick}
          fullWidth
        >
          {denyActionLabel}
        </Button>
      </Grid>
      <Grid size={6}>
        <Button
          variant="outlined"
          type="submit"
          disabled={confirmButtonShouldBeDisabled || !onConfirmActionClick}
          onClick={onConfirmActionClick}
          loading={actionIsBeingConfirmed}
          fullWidth
        >
          {confirmActionLabel}
        </Button>
      </Grid>
    </Grid>
  );
};

export default ActionButtons;
