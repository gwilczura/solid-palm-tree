namespace Wilczura.Ocr;

public static class Extensions
{
    public static string FormatNumber(this List<int> number, string? suffix = null)
    {
        return $"{string.Join("", number.Select(d => d.ToString()))} {suffix ?? string.Empty}".Trim();
    }

    public static string FormatNumber(this List<int?> number, string? suffix = null)
    {
        return $"{string.Join("", number.Select(d => d.HasValue ? d.ToString() : "?"))} {suffix ?? string.Empty}".Trim();
    }
}
