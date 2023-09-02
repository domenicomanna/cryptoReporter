namespace Api.Utils;

public class CsvStringToListConverter
{
    public static List<string> ConvertToList(string csvContent)
    {
        return csvContent.Split(',').Select(value => value.Trim()).ToList();
    }
}
