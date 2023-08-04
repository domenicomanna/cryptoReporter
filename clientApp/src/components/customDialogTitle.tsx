import { DialogTitle, IconButton } from '@mui/material';
import { FC } from 'react';
import CloseIcon from '@mui/icons-material/Close';

type Props = {
  onClose?: () => void;
  children: string;
};

const CustomDialogTitle: FC<Props> = ({ onClose, children }) => {
  return (
    <DialogTitle
      sx={{
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'space-between',
        paddingBottom: 0,
        marginBottom: '.75rem',
      }}
    >
      {children}
      {onClose && (
        <IconButton onClick={onClose}>
          <CloseIcon />
        </IconButton>
      )}
    </DialogTitle>
  );
};

export default CustomDialogTitle;
