import { MuiFileInput } from 'mui-file-input';
import { FC } from 'react';
import ActionButtons from '../../../components/actionButtons';
import { Box } from '@mui/material';
import * as Yup from 'yup';
import { useFormik } from 'formik';

export type ChooseFileFormValues = {
  file: File | null;
};

type Props = {
  formValues: ChooseFileFormValues;
  onFormValuesChange: (chooseFileFormValues: ChooseFileFormValues) => void;
  onNextStepClick: () => void;
};

const ChooseFileStep: FC<Props> = ({ formValues, onFormValuesChange, onNextStepClick }) => {
  const formik = useFormik({
    initialValues: formValues,
    validationSchema: Yup.object({
      file: Yup.mixed().required('Required'),
    }),
    onSubmit: onNextStepClick,
    validateOnMount: true,
  });

  const onFieldValueChange = (fieldName: keyof ChooseFileFormValues, fieldValue: any) => {
    void formik.setFieldValue(fieldName, fieldValue);
    onFormValuesChange({
      ...formValues,
      [fieldName]: fieldValue,
    });
  };

  return (
    <Box component={'form'} onSubmit={formik.handleSubmit}>
      <Box sx={{ marginBottom: '1.5rem' }}>
        <MuiFileInput
          placeholder="Select a file"
          label="File"
          size="small"
          fullWidth
          hideSizeText
          name="file"
          required
          value={formik.values.file}
          onChange={(file) => onFieldValueChange('file', file)}
          onBlur={formik.handleBlur}
          InputProps={{
            inputProps: {
              accept: 'text/csv',
            },
          }}
          helperText={formik.touched.file && formik.errors.file}
          error={formik.touched.file && Boolean(formik.errors.file)}
        />
      </Box>
      <ActionButtons
        confirmActionLabel="Next"
        denyActionLabel="Back"
        onConfirmActionClick={formik.handleSubmit}
        confirmButtonShouldBeDisabled={!formik.isValid}
      />
    </Box>
  );
};

export default ChooseFileStep;
