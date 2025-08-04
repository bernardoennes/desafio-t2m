using System.Globalization;
using System.Text;

namespace desafio_t2m.Utils
{
    public static class NameNormalizer
    {
        public static string Normalize(string input)
        {
            var normalized = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString()
                     .Replace(" ", "")
                     .ToLowerInvariant();
        }
    }
}