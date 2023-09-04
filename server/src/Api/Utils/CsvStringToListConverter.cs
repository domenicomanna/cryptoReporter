namespace Api.Utils;

public enum LetterCase
{
    LeaveAsIs,
    Lower,
    Upper,
}

public class CsvStringToListConverter
{
    public static List<string> ConvertToList(string csvContent, LetterCase letterCase = LetterCase.LeaveAsIs)
    {
        return csvContent
            .Split(',')
            .Select(value =>
            {
                value = value.Trim();
                if (letterCase == LetterCase.LeaveAsIs)
                    return value;
                if (letterCase == LetterCase.Lower)
                    return value.ToLower();
                return value.ToUpper();
            })
            .ToList();
    }
}
