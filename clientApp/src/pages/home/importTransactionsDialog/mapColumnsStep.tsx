import { Autocomplete, Box, Checkbox, Divider, FormControlLabel, Grid, TextField, Typography } from '@mui/material';
import { FC, Fragment } from 'react';
import { CsvRecord } from './helpers/parseCsvFile';
import { useFormik } from 'formik';
import * as Yup from 'yup';
import ActionButtons from '../../../components/actionButtons';

export type MapColumnsFormValues = {
  dateColumnName: string | null;
  quantityTransactedColumnName: string | null;
  priceColumnName: string | null;
  feeColumnName: string | null;
  transactionTypeColumnName: string | null;
  exchangeColumnName: string | null;
  numberOfCoinsSoldColumnName: string | null;
  notesColumnName: string | null;
  deleteExistingTransactions: boolean;
};

type SourceColumnFormField = {
  sourceFieldName: keyof MapColumnsFormValues;
  targetColumnName: string;
  valueIsRequired: boolean;
};

const formFields: SourceColumnFormField[] = [
  {
    sourceFieldName: 'dateColumnName',
    targetColumnName: 'Date',
    valueIsRequired: true,
  },
  {
    sourceFieldName: 'quantityTransactedColumnName',
    targetColumnName: 'Quantity Transacted',
    valueIsRequired: true,
  },
  {
    sourceFieldName: 'priceColumnName',
    targetColumnName: 'Price',
    valueIsRequired: true,
  },
  {
    sourceFieldName: 'feeColumnName',
    targetColumnName: 'Fee',
    valueIsRequired: true,
  },
  {
    sourceFieldName: 'transactionTypeColumnName',
    targetColumnName: 'Transaction Type',
    valueIsRequired: true,
  },
  {
    sourceFieldName: 'exchangeColumnName',
    targetColumnName: 'Exchange',
    valueIsRequired: false,
  },
  {
    sourceFieldName: 'numberOfCoinsSoldColumnName',
    targetColumnName: 'Number of Coins Sold',
    valueIsRequired: true,
  },
  {
    sourceFieldName: 'notesColumnName',
    targetColumnName: 'Notes',
    valueIsRequired: false,
  },
];

type Props = {
  formValues: MapColumnsFormValues;
  onFormValuesChange: (mapColumnsFormValues: MapColumnsFormValues) => void;
  onNextStepClick: () => Promise<void>;
  onPreviousStepClick: () => void;
  csvRecords: CsvRecord[];
};

const MapColumnsStep: FC<Props> = ({
  formValues,
  onFormValuesChange,
  onNextStepClick,
  onPreviousStepClick,
  csvRecords,
}) => {
  const csvFileColumnNames = Object.keys(csvRecords[0] ?? {}).sort((a, b) =>
    a.toLowerCase().localeCompare(b.toLowerCase())
  );

  const formik = useFormik<MapColumnsFormValues>({
    initialValues: formValues,
    validationSchema: Yup.object({
      dateColumnName: Yup.string().required('Required'),
      quantityTransactedColumnName: Yup.string().required('Required'),
      priceColumnName: Yup.string().required('Required'),
      feeColumnName: Yup.string().required('Required'),
      transactionTypeColumnName: Yup.string().required('Required'),
      exchangeColumnName: Yup.string().optional(),
      numberOfCoinsSoldColumnName: Yup.string().required('Required'),
      notesColumnName: Yup.string().optional(),
      deleteExistingTransactions: Yup.boolean().optional(),
    }),
    onSubmit: async () => {
      await onNextStepClick();
    },
    validateOnMount: true,
  });

  const onFieldValueChange = (fieldName: keyof MapColumnsFormValues, fieldValue: string | null | boolean) => {
    void formik.setFieldValue(fieldName, fieldValue);
    onFormValuesChange({
      ...formValues,
      [fieldName]: fieldValue,
    });
  };

  return (
    <Box component="form" onSubmit={formik.handleSubmit}>
      <Grid container rowSpacing={1.5} columnSpacing={3} alignItems={'center'} sx={{ marginBottom: '1rem' }}>
        <Grid item xs={6}>
          <Typography fontWeight={500}>Source Column</Typography>
        </Grid>
        <Grid item xs={6}>
          <Typography fontWeight={500}>Target Column</Typography>
        </Grid>

        {formFields.map((field) => (
          <Fragment key={field.sourceFieldName}>
            <Grid item xs={6}>
              <Autocomplete
                options={csvFileColumnNames}
                fullWidth
                renderInput={(params) => (
                  <TextField
                    {...params}
                    name={field.sourceFieldName}
                    variant="standard"
                    required={field.valueIsRequired}
                    helperText={formik.touched[field.sourceFieldName] && formik.errors[field.sourceFieldName]}
                    error={formik.touched[field.sourceFieldName] && Boolean(formik.errors[field.sourceFieldName])}
                  />
                )}
                size="small"
                value={formik.values[field.sourceFieldName] as string}
                onChange={(event, value) => onFieldValueChange(field.sourceFieldName, value)}
                onBlur={formik.handleBlur}
              />
            </Grid>
            <Grid item xs={6}>
              <Typography>{field.targetColumnName}</Typography>
            </Grid>
          </Fragment>
        ))}

        <Grid item xs={12}>
          <Divider sx={{ marginTop: '1rem' }} />
        </Grid>

        <Grid item xs={12}>
          <FormControlLabel
            control={
              <Checkbox
                {...formik.getFieldProps('deleteExistingTransactions')}
                checked={formik.values.deleteExistingTransactions}
                onChange={(event, value) => onFieldValueChange('deleteExistingTransactions', value)}
              />
            }
            label="Delete Existing Transactions"
          />
        </Grid>
      </Grid>

      <ActionButtons
        confirmActionLabel="Submit"
        denyActionLabel="Back"
        onConfirmActionClick={formik.handleSubmit}
        onDenyActionClick={onPreviousStepClick}
        denyButtonShouldBeDisabled={false}
        confirmButtonShouldBeDisabled={!formik.isValid}
        actionIsBeingConfirmed={formik.isSubmitting}
      />
    </Box>
  );
};

export default MapColumnsStep;
