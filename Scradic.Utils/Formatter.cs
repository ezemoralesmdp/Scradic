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

        public static string FormatFileSize(long fileSizeInBytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double size = fileSizeInBytes;
            int unitIndex = 0;

            while (size >= 1024 && unitIndex < sizes.Length - 1)
            {
                size /= 1024;
                unitIndex++;
            }

            return $"{size:N2} {sizes[unitIndex]}";
        }
    }
}