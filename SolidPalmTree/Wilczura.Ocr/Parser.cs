using Ardalis.GuardClauses;
using System.Text;

namespace Wilczura.Ocr;

public static class Parser
{
    /// <summary>
    /// Use case 1 - simple parsing
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    public static List<int> GetNumber(string entry)
    {
        var digits = ParseEntryToDigits(entry);
        return digits.Select(d =>
        {
            var digit = GetExactDigit(d);
            if (!digit.HasValue)
            {
                throw new Exception($"Unrecognizable digit: {d}");
            }

            return digit.Value;
        }).ToList();
    }

    /// <summary>
    /// Use case 3 - parsing with result
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    public static string GetParsingResult(string entry)
    {
        var digits = ParseEntryToDigits(entry);
        var parsedDigits = digits.Select(d => GetExactDigit(d)).ToList();
        if(parsedDigits.Any(a=>!a.HasValue))
        {
            return FormatNumber(parsedDigits, Consts.Illegal);
        }

        var isAccountNumber = IsAccountNumber(parsedDigits);
        return FormatNumber(parsedDigits, isAccountNumber ? string.Empty : Consts.Error);
    }

    private static string FormatNumber(List<int?> number, string suffix)
    {
        return $"{string.Join("", number.Select(d => d.HasValue ? d.ToString() : "?"))} {suffix}".Trim();
    }

    /// <summary>
    /// UseCase 4 - accept potential errors
    /// </summary>
    public static string GetParsingResultWithDeviation(string entry)
    {
        var digits = ParseEntryToDigits(entry);
        
        // if exact match is correct
        var parsedDigits = digits.Select(d => GetExactDigit(d)).ToList();
        if (parsedDigits.All(a => a.HasValue) && IsAccountNumber(parsedDigits))
        {
            return FormatNumber(parsedDigits, string.Empty);
        }

        // otherwise look for alternatives
        var mapped = digits.Select(d =>
        {
            var digit = GetExactDigit(d);
            var nearDigits = GetNearDigits(d);

            return (digit.HasValue ? nearDigits.Prepend(digit.Value) : nearDigits).ToList();
        }).ToList();

        // if some positions have no alternatives
        if(mapped.Any(a=>a.Count == 0))
        {
            return FormatNumber(parsedDigits, Consts.Illegal);
        }

        // do Ambiguous


        // if no Ambiguouity then Error or Illegal
    }

    public static IList<string> ParseEntryToDigits(string entry)
    {
        Guard.Against.NullOrEmpty(entry);

        var lines = entry.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Guard.Against.OutOfRange(lines.Length, "Number of lines with text.", 3, 3);
        List<string> number = new List<string>(9);
        for (int i = 0; i < 9; i++)
        {
            var offset = i * 3;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(lines[0].Substring(offset, 3));
            stringBuilder.Append(lines[1].Substring(offset, 3));
            stringBuilder.Append(lines[2].Substring(offset, 3));
            number.Add(stringBuilder.ToString());
        }

        return number;
    }

    public static string GetEntryFromDigits(List<string> digits)
    {
        Guard.Against.NullOrEmpty(digits);
        Guard.Against.OutOfRange(digits.Count, "Number of digits.", 9, 9);
        StringBuilder builder = new StringBuilder();
        // generate 3 lines
        for (int i = 0; i < 3; i++)
        {
            foreach (var digit in digits)
            {
                builder.Append(digit.Substring(i * 3, 3));
            }

            builder.AppendLine(string.Empty);
        }

        return builder.ToString();
    }

    public static int? GetExactDigit(string digit)
    {
        int? directMatch = null;
        switch (digit)
        {
            case Consts.Zero:   directMatch = 0; break;
            case Consts.One:    directMatch = 1; break;
            case Consts.Two:    directMatch = 2; break;
            case Consts.Three:  directMatch = 3; break;
            case Consts.Four:   directMatch = 4; break;
            case Consts.Five:   directMatch = 5; break;
            case Consts.Six:    directMatch = 6; break;
            case Consts.Seven:  directMatch = 7; break;
            case Consts.Eight:  directMatch = 8; break;
            case Consts.Nine:   directMatch = 9; break;
        }

        return directMatch;
    }

    public static List<int> GetNearDigits(string digit)
    {
        var digits = new List<int>();

        if (IsNearByOneSign(digit, Consts.Zero))    digits.Add(0);
        if (IsNearByOneSign(digit, Consts.One))     digits.Add(1);
        if (IsNearByOneSign(digit, Consts.Two))     digits.Add(2);
        if (IsNearByOneSign(digit, Consts.Three))   digits.Add(3);
        if (IsNearByOneSign(digit, Consts.Four))    digits.Add(4);
        if (IsNearByOneSign(digit, Consts.Five))    digits.Add(5);
        if (IsNearByOneSign(digit, Consts.Six))     digits.Add(6);
        if (IsNearByOneSign(digit, Consts.Seven))   digits.Add(7);
        if (IsNearByOneSign(digit, Consts.Eight))   digits.Add(8);
        if (IsNearByOneSign(digit, Consts.Nine))    digits.Add(9);

        return digits;
    }

    public static bool IsNearByOneSign(string originalDigit, string otherDigit)
    {
        Guard.Against.NullOrEmpty(originalDigit);
        Guard.Against.NullOrEmpty(otherDigit);
        Guard.Against.LengthOutOfRange(otherDigit, originalDigit.Length, originalDigit.Length);

        var isNear = false;
        for(int i = 0; i< originalDigit.Length; i++)
        {
            if (originalDigit[i] != otherDigit[i])
            {
                if(isNear)
                {
                    return false;
                }

                isNear = true;
            }
        }

        return isNear;
    }

    public static bool IsAccountNumber(List<int?> numbers)
    {
        Guard.Against.NullOrEmpty(numbers);
        Guard.Against.OutOfRange(numbers.Count, "Count", 9, 9);
        int multiplier = 9;
        var sum = numbers.Select((item, index) => (multiplier - index) * item).Sum();
        return sum % 11 == 0;
    }
}
