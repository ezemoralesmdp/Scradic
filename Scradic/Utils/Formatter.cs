using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Scradic.Utils
{
    public static class Formatter
    {
        public static string WithoutAccentsAndLowerCase(this string input) =>
            new string(
                input.Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray()
            )
        .Normalize(NormalizationForm.FormC).ToLower();

        public static string? SanitizeDataInput(string? input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                Regex regex = new Regex("\\s|'");
                input = regex.Replace(input, "-");
                input = input.WithoutAccentsAndLowerCase();
            }
            return input;
        }
    }
}