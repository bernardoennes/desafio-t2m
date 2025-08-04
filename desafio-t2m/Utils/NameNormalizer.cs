using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace desafio_t2m.Utils
{
    public static class NameNormalizer
    {
        public static string Normalize(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "";

            var normalized = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            var noAccents = sb.ToString();
            var clean = Regex.Replace(noAccents, @"[^a-zA-Z0-9]", "");

            return clean.ToLowerInvariant();
        }
    }
}
