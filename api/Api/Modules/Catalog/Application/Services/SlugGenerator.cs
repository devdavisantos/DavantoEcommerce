using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Api.Modules.Catalog.Application.Services
{
    public static partial class SlugGenerator
    {
        public static string Generate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var normalized = value
                .Trim()
                .ToLowerInvariant()
                .Normalize(NormalizationForm.FormD);

            var builder = new StringBuilder();

            foreach (var character in normalized)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(character);

                if (category != UnicodeCategory.NonSpacingMark)
                    builder.Append(character);
            }

            var slug = builder
                .ToString()
                .Normalize(NormalizationForm.FormC);

            slug = InvalidCharactersRegex().Replace(slug, "-");
            slug = MultipleHyphensRegex().Replace(slug, "-");

            return slug.Trim('-');
        }

        public static string GenerateUnique(
        string baseSlug,
        IReadOnlyCollection<string> existingSlugs)
        {
            if (!existingSlugs.Contains(
                baseSlug,
                StringComparer.OrdinalIgnoreCase))
            {
                return baseSlug;
            }

            var prefix = $"{baseSlug}-";

            var usedSuffixes = existingSlugs
                .Where(x => x.StartsWith(
                    prefix,
                    StringComparison.OrdinalIgnoreCase))
                .Select(x => x[prefix.Length..])
                .Select(x => int.TryParse(x, out var suffix)
                    ? suffix
                    : 0)
                .Where(x => x >= 2)
                .ToHashSet();

            var nextSuffix = 2;

            while (usedSuffixes.Contains(nextSuffix))
                nextSuffix++;

            return $"{baseSlug}-{nextSuffix}";
        }

        [GeneratedRegex(@"[^a-z0-9]+")]
        private static partial Regex InvalidCharactersRegex();

        [GeneratedRegex(@"-+")]
        private static partial Regex MultipleHyphensRegex();
    }
}