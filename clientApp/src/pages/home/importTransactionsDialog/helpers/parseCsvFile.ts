import Papa from 'papaparse';

export type CsvRecord = {
  [fieldName: string]: string;
};

export const parseCsvFile = (file: File): Promise<CsvRecord[]> => {
  return new Promise((resolve, reject) => {
    Papa.parse(file, {
      header: true,
      skipEmptyLines: true,
      complete: (results) => {
        resolve(results.data as CsvRecord[]);
      },
      error: (error) => {
        reject(error);
      },
    });
  });
};
