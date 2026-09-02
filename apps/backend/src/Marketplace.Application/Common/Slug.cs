using System.Globalization;
using System.Text;

namespace Marketplace.Application.Common;

/// <summary>
/// Generates URL-safe listing slugs from a title. Vietnamese diacritics are stripped and a short
/// random suffix is appended so slugs are unique without a database round-trip (the DB enforces a
/// unique index as a backstop).
/// </summary>
public static class Slug
{
    public static string Generate(string title)
    {
        string normalized = StripDiacritics(title ?? string.Empty).ToLowerInvariant();

        var builder = new StringBuilder(normalized.Length);
        bool lastWasHyphen = false;
        foreach (char c in normalized)
        {
            if (char.IsLetterOrDigit(c))
            {
                builder.Append(c);
                lastWasHyphen = false;
            }
            else if (!lastWasHyphen && builder.Length > 0)
            {
                builder.Append('-');
                lastWasHyphen = true;
            }
        }

        string body = builder.ToString().Trim('-');
        if (body.Length == 0)
        {
            body = "listing";
        }

        // 6 hex chars from a fresh Guid — enough entropy to avoid collisions in practice.
        string suffix = Guid.NewGuid().ToString("N")[..6];
        return $"{body}-{suffix}";
    }

    private static string StripDiacritics(string text)
    {
        // Đ/đ have no combining form, so map them explicitly before Unicode decomposition.
        text = text.Replace('Đ', 'D').Replace('đ', 'd');

        string decomposed = text.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(decomposed.Length);
        foreach (char c in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(c);
            }
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }
}
