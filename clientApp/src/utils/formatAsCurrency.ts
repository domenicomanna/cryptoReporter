export const formatAsCurrency = (value: number, currencyType = 'USD'): string => {
  const formatter = new Intl.NumberFormat(undefined, {
    style: 'currency',
    currency: currencyType,
  });

  return formatter.format(value);
};
