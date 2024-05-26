using Ardalis.GuardClauses;
using System.Text;

namespace Wilczura.Ocr.Domain;

public class Entry
{
    public static Entry? CreateFromNumber(string number)
    {
        var digits = number.Select(x => new Digit(Digit.DigitsMap[int.Parse(x.ToString())])).ToList();
        return new Entry(digits);
    }

    public static Entry? CreateFromEntryText(string entry)
    {
        return new Entry(ParseEntryToDigits(entry));
    }

    private List<Digit> _digits;

    private Entry(List<Digit> digits)
    {
        Guard.Against.NullOrOutOfRange(digits.Count, "Count", 9, 9);
        _digits = digits;
    }

    public override string ToString()
    {
        return base.ToString();
    }

    private bool IsAccountNumber(List<Digit> digits)
    {
        Guard.Against.NullOrEmpty(digits);
        Guard.Against.OutOfRange(digits.Count, "Count", 9, 9);
        int multiplier = 9;
        var sum = digits.Select((item, index) => (multiplier - index) * item.DigitNumber).Sum();
        return sum % 11 == 0;
    }

    private static List<Digit> ParseEntryToDigits(string entry)
    {
        Guard.Against.NullOrEmpty(entry);

        var lines = entry.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Guard.Against.OutOfRange(lines.Length, "Number of lines with text.", 3, 3);
        List<Digit> number = new(9);
        for (int i = 0; i < 9; i++)
        {
            var offset = i * 3;
            StringBuilder stringBuilder = new StringBuilder()
                .Append(lines[0].AsSpan(offset, 3))
                .Append(lines[1].AsSpan(offset, 3))
                .Append(lines[2].AsSpan(offset, 3));
            number.Add(new Digit(stringBuilder.ToString()));
        }

        return number;
    }
}
