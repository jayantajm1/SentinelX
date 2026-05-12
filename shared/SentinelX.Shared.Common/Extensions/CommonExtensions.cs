namespace SentinelX.Shared.Common.Extensions;

public static class StringExtensions
{
    public static string ToSnakeCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var startUnderscores = new string(input.TakeWhile(c => c == '_').ToArray());
        return startUnderscores + System.Text.RegularExpressions.Regex.Replace(
            input[startUnderscores.Length..],
            "(?<!^)(?<=[a-z0-9])(?=[A-Z])|(?<!^)(?<=[A-Z])(?=[A-Z][a-z])",
            "_")
            .ToLower();
    }

    public static string ToPascalCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var parts = input.Split('_', '-');
        return string.Concat(parts.Select(p => char.ToUpper(p[0]) + p[1..]));
    }
}

public static class GuidExtensions
{
    public static string ToShortId(this Guid guid)
    {
        return Convert.ToBase64String(guid.ToByteArray())
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }
}
