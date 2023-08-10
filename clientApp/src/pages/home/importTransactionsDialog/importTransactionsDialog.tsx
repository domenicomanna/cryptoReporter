import { Box, Dialog, DialogContent, Step, StepLabel, Stepper } from '@mui/material';
import { FC, ReactElement, useState } from 'react';
import CustomDialogTitle from '../../../components/customDialogTitle';
import { toast } from 'react-toastify';
import ChooseFileStep, { ChooseFileFormValues } from './chooseFileStep';
import MapColumnsStep, { MapColumnsFormValues } from './mapColumnsStep';
import { CsvRecord, parseCsvFile } from './helpers/parseCsvFile';
import { transactionsApi } from '../../../api';
import { AddTransactionsRequest } from '../../../api/generatedSdk';
import { buildTransactions } from './helpers/buildTransactions';

enum StepName {
  ChooseFile = 'Choose File',
  MapColumns = 'Map Columns',
}

type Step = {
  stepName: StepName;
  stepNumber: number;
};

const steps: Step[] = [
  {
    stepNumber: 0,
    stepName: StepName.ChooseFile,
  },
  {
    stepNumber: 1,
    stepName: StepName.MapColumns,
  },
];

type FormValues = {
  chooseFileStep: ChooseFileFormValues;
  mapColumnsStep: MapColumnsFormValues;
};

const _mapColumnsStepStorageKey = 'mapColumnsStepFormValues';

type Props = {
  onCloseDialog: () => void;
  onTransactionsImported: () => void;
};

const ImportTransactionsDialog: FC<Props> = ({ onCloseDialog, onTransactionsImported }) => {
  const [activeStepNumber, setActiveStepNumber] = useState(0);
  const [formValues, setFormValues] = useState<FormValues>({
    chooseFileStep: {
      file: null,
    },
    mapColumnsStep: localStorage.getItem(_mapColumnsStepStorageKey)
      ? (JSON.parse(localStorage.getItem(_mapColumnsStepStorageKey)!) as MapColumnsFormValues)
      : {
          dateColumnName: null,
          cryptoTickerColumnName: null,
          quantityTransactedColumnName: null,
          priceColumnName: null,
          feeColumnName: null,
          transactionTypeColumnName: null,
          exchangeColumnName: null,
          numberOfCoinsSoldColumnName: null,
          notesColumnName: null,
          deleteExistingTransactions: false,
        },
  });
  const [csvRecords, setCsvRecords] = useState<CsvRecord[]>([]);

  const activeStep = steps.find((step) => step.stepNumber === activeStepNumber);

  const getStepContent = (): ReactElement => {
    if (activeStep?.stepName === StepName.ChooseFile) {
      return (
        <ChooseFileStep
          formValues={formValues.chooseFileStep}
          onNextStepClick={handleNextStepClickOnChooseFileStep}
          onFormValuesChange={(chooseFileFormValues) => {
            setFormValues({
              ...formValues,
              chooseFileStep: chooseFileFormValues,
            });
          }}
        />
      );
    }
    if (activeStep?.stepName === StepName.MapColumns) {
      return (
        <MapColumnsStep
          formValues={formValues.mapColumnsStep}
          onNextStepClick={handleNextStepClickOnMapColumnsStep}
          onPreviousStepClick={() => setActiveStepNumber((step) => step - 1)}
          onFormValuesChange={(mapColumnsFormValues) => {
            setFormValues({
              ...formValues,
              mapColumnsStep: mapColumnsFormValues,
            });
          }}
          csvRecords={csvRecords}
        />
      );
    }
    throw new Error(`No component found for ${activeStep?.stepName} step`);
  };

  const handleNextStepClickOnChooseFileStep = async () => {
    try {
      const csvRecords: CsvRecord[] = await parseCsvFile(formValues.chooseFileStep.file!);
      setCsvRecords(csvRecords);
      setActiveStepNumber((activeStep) => activeStep + 1);
    } catch (error) {
      toast.error('Could not parse csv file');
    }
  };

  const handleNextStepClickOnMapColumnsStep = async () => {
    localStorage.setItem(_mapColumnsStepStorageKey, JSON.stringify(formValues.mapColumnsStep));
    try {
      const addTransactionsRequest: AddTransactionsRequest = {
        deleteExistingTransactions: formValues.mapColumnsStep.deleteExistingTransactions,
        transactions: buildTransactions(csvRecords, formValues.mapColumnsStep),
      };
      await transactionsApi.addTransactions({ addTransactionsRequest });
      onTransactionsImported();
      toast.success('Transactions imported!');
      onCloseDialog();
    } catch (error) {
      console.error(error);
      toast.error('Transactions could not be imported');
    }
  };

  return (
    <Dialog
      onClose={(event, reason) => {
        if (reason === 'backdropClick') return;
        onCloseDialog();
      }}
      open
      fullWidth
      PaperProps={{
        sx: {
          maxWidth: '500px',
        },
      }}
    >
      <CustomDialogTitle onClose={onCloseDialog}>Import Transactions</CustomDialogTitle>
      <DialogContent>
        <Box sx={{ marginBottom: '1.25rem' }}>
          <Stepper activeStep={activeStepNumber}>
            {steps.map((step) => (
              <Step key={step.stepName}>
                <StepLabel>{step.stepName}</StepLabel>
              </Step>
            ))}
          </Stepper>
        </Box>
        <Box sx={{ marginBottom: '1.5rem' }}>{getStepContent()}</Box>
      </DialogContent>
    </Dialog>
  );
};

export default ImportTransactionsDialog;
