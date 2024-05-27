using Ardalis.GuardClauses;
using System.Linq;
using System.Text;

namespace Wilczura.Ocr.Domain;

public class Entry
{
    public const string Illegible = "ILL";
    public const string Error = "ERR";
    public const string Ambiguous = "AMB";

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
        if (IsAccountNumber(_digits))
        {
            return $"{string.Join("", _digits)}";
        }

        var alternatives = GetAlternatives();
        if(alternatives.Count == 1)
        {
            return alternatives[0].ToString();
        }

        if(alternatives.Count > 1)
        {
            var alternativesPart = string.Join("', '", alternatives.Select(a => a.ToString()).OrderBy(a => a));
            return $"{string.Join("", _digits)} {Ambiguous} ['{alternativesPart}']";
        }

        if(IsIllegible())
        {
            return $"{string.Join("", _digits)} {Illegible}";
        }

        return $"{string.Join("", _digits)} {Error}";
    }

    internal bool IsAccountNumber()
    {
        return IsAccountNumber(_digits);
    }

    private static List<List<Digit>> GetCombinations(List<List<Digit>> candidates, List<Digit> head, List<Digit> tail)
    {
        Guard.Against.Null(candidates);

        if (tail.Count == 0)
        {
            return candidates;
        }

        var originalDigit = tail.First();
        var newDigits = originalDigit.GetNearDigits();

        var newCandidates = candidates.Select(c => c.Append(originalDigit).ToList())
            .Concat(newDigits.Select(nd => head.Append(nd).ToList()))
            .ToList();

        return GetCombinations(newCandidates, [.. head, originalDigit], tail.Skip(1).ToList());
    }

    private static bool IsIllegible(List<Digit> digits)
    {
        return digits.Any(d => !d.DigitNumber.HasValue);
    }

    private static bool IsAccountNumber(List<Digit> digits)
    {
        Guard.Against.NullOrEmpty(digits);
        Guard.Against.OutOfRange(digits.Count, "Count", 9, 9);

        if(IsIllegible(digits))
        {
            return false;
        }

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

    private List<Entry> GetCombinations()
    {
        return GetCombinations(
            _digits.First().GetNearDigits().Select(d => new List<Digit> { d }).ToList(),
            [_digits.First()],
            _digits.Skip(1).ToList())
            .Select(e => new Entry(e)).ToList();
    }

    private List<Entry> GetAlternatives()
    {
        if (IsAccountNumber(_digits) ||
            _digits.Where(d => !d.DigitNumber.HasValue).Count() > 1)
        {
            return [];
        }

        if (IsIllegible())
        {
            // we need to "fix" the illegible digit
            var illegibleDigit = _digits.Where(d => !d.DigitNumber.HasValue).Single();
            var prefix = _digits.TakeWhile(d => d.DigitNumber.HasValue);
            var sufix = _digits.SkipWhile(d => d.DigitNumber.HasValue).Skip(1);

            return illegibleDigit.GetNearDigits()
                .Select(nd => prefix.Concat(new[] { nd }).Concat(sufix))
                .Select(e => new Entry(e.ToList()))
                .Where(e => e.IsAccountNumber())
                .ToList();
        }

        // if all digit has no alternatives
        if (_digits.All(d => d.GetNearDigits().Count == 0))
        {
            return [];
        }

        return GetCombinations()
            .Where(e=> e.IsAccountNumber())
            .ToList();
    }

    private bool IsIllegible()
    {
        return IsIllegible(_digits);
    }
}
