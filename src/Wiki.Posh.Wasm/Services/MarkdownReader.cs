using System.Text.RegularExpressions;

namespace Wiki.Posh.Wasm.Services;

public static partial class MarkdownHeaderReader
{
    [GeneratedRegex(@"-{3}\s*\r?\n?", RegexOptions.Compiled)]
    private static partial Regex HeaderIdentifierRegex();

    public static Dictionary<string, string> ReadFromStream(Stream stream)
    {
        using var reader = new StreamReader(stream);

        string? firstLine = reader.ReadLine();
        if (firstLine is null)
        {
            return [];
        }

        if (!HeaderIdentifierRegex().IsMatch(firstLine))
        {
            return [];
        }

        Dictionary<string, string> ret = [];
        while (true)
        {
            string? newLine = reader.ReadLine();
            if (newLine is null)
            {
                return [];
            }

            if (HeaderIdentifierRegex().IsMatch(newLine))
            {
                break;
            }

            string[] split = newLine.Split(':');
            ret.Add(split[0], split.Length == 1 ? string.Empty : string.Join(':', split.Skip(1).ToArray()).TrimStart(' '));
        }

        return ret;
    }
}