using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Extension for csv
/// </summary>
public static class CsvExtension
{
    /// <summary>
    /// Convert collection string to csv line
    /// </summary>
    /// <param name="fields"></param>
    /// <returns></returns>
    public static string ToCsvLine(this IEnumerable<string> fields)
    {
        if (fields == null || fields.Any() == false)
            return string.Empty;

        var builder = new StringBuilder();

        foreach (var field in fields)
        {
            var convertedField = field;

            if (convertedField.Contains("\""))
                convertedField = convertedField.Replace("\"", "\"\"");

            if (convertedField.Contains(",") || convertedField.Contains("\\n"))
                convertedField = string.Format("\"{0}\"", convertedField);

            builder.Append($"{convertedField},");
        }

        return builder.ToString();
    }
}