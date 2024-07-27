export const formatAsPercent = (value: number): string => {
  const formatter = new Intl.NumberFormat(undefined, {
    style: 'percent',
    maximumFractionDigits: 2,
  });

  return formatter.format(value);
};
