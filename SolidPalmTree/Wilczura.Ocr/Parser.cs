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
            return parsedDigits.FormatNumber(Consts.Illegible);
        }

        var isAccountNumber = IsAccountNumber(parsedDigits);
        return parsedDigits.FormatNumber(isAccountNumber ? string.Empty : Consts.Error);
    }

    /// <summary>
    /// UseCase 4 - accept potential single error
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    public static string GetParsingResultWithSingleDeviation(string entry)
    {
        var digits = ParseEntryToDigits(entry);

        // if exact match is correct
        var parsedDigits = digits.Select(d => GetExactDigit(d)).ToList();
        if (parsedDigits.All(a => a.HasValue) && IsAccountNumber(parsedDigits))
        {
            return parsedDigits.FormatNumber();
        }

        // otherwise look for alternatives
        var mapped = digits.Select(d =>
        {
            var digit = GetExactDigit(d);
            var nearDigits = GetNearDigits(d);

            return (digit.HasValue ? nearDigits.Prepend(digit.Value) : nearDigits).ToList();
        }).ToList();

        // if some positions have no alternatives
        if (mapped.Any(a => a.Count == 0))
        {
            return parsedDigits.FormatNumber(Consts.Illegible);
        }

        // do Ambiguous

        // this is suboptimal - we could be checking for "near by one digit"
        // already during the recursion - this would result in smaller set and less recursive calls
        // also if parsed result already have more than one "?" then it's impossible to get valid result
        var combinations = GetCombinations(mapped, []);
        var alternativeAccountNumbers = combinations
            .Where(c => IsNearByOneDigit(c, parsedDigits))
            .Where(c => IsAccountNumber(c.Cast<int?>().ToList()))
            .ToList();

        if (alternativeAccountNumbers.Count == 1)
        {
            return alternativeAccountNumbers[0].FormatNumber();
        }
        else if (alternativeAccountNumbers.Count > 1)
        {
            var firstPart = parsedDigits.FormatNumber(Consts.Ambiguous);
            var alternativesPart = string.Join("', '", alternativeAccountNumbers.Select(aan => aan.FormatNumber()).OrderBy(a => a));
            return $"{firstPart} ['{alternativesPart}']";
        }

        // if no ambiguity then Error or Illegible
        // parsing from scratch is also suboptimal
        return GetParsingResult(entry);
    }

    /// <summary>
    /// UseCase 4 (incorrect) - accept potential errors (multiple)
    /// </summary>
    public static string GetParsingResultWithDeviations(string entry)
    {
        var digits = ParseEntryToDigits(entry);
        
        // if exact match is correct
        var parsedDigits = digits.Select(d => GetExactDigit(d)).ToList();
        if (parsedDigits.All(a => a.HasValue) && IsAccountNumber(parsedDigits))
        {
            return parsedDigits.FormatNumber();
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
            return parsedDigits.FormatNumber(Consts.Illegible);
        }

        // do Ambiguous
        var combinations = GetCombinations(mapped, []);
        var alternativeAccountNumbers = combinations.Where(c => IsAccountNumber(c.Cast<int?>().ToList())).ToList();
        if(alternativeAccountNumbers.Count == 1)
        {
            return alternativeAccountNumbers[0].FormatNumber();
        }
        else if(alternativeAccountNumbers.Count > 1) 
        {
            var firstPart = parsedDigits.FormatNumber(Consts.Ambiguous);
            var alternativesPart = string.Join("', '", alternativeAccountNumbers.Select(aan => aan.FormatNumber()).OrderBy(a => a));
            return $"{firstPart} ['{alternativesPart}']";
        }

        // if no ambiguity then Error or Illegible
        return GetParsingResult(entry);
    }

    public static List<List<int>> GetCombinations(List<List<int>> map, List<List<int>> candidates)
    {
        Guard.Against.Null(candidates);
        Guard.Against.Null(map);

        if(map.Count == 0)
        {
            return candidates;
        }

        var newCandidates = candidates.Count == 0
            ? map[0].Select(a => new List<int> { a }).ToList()
            : map[0].SelectMany(a => candidates.Select(c => c.Append(a).ToList())).ToList();
        var newMap = map.Skip(1).ToList();

        return GetCombinations(newMap, newCandidates);
    }

    public static IList<string> ParseEntryToDigits(string entry)
    {
        Guard.Against.NullOrEmpty(entry);

        var lines = entry.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Guard.Against.OutOfRange(lines.Length, "Number of lines with text.", 3, 3);
        List<string> number = new(9);
        for (int i = 0; i < 9; i++)
        {
            var offset = i * 3;
            StringBuilder stringBuilder = new StringBuilder()
                .Append(lines[0].AsSpan(offset, 3))
                .Append(lines[1].AsSpan(offset, 3))
                .Append(lines[2].AsSpan(offset, 3));
            number.Add(stringBuilder.ToString());
        }

        return number;
    }

    public static string GetEntryFromDigits(List<string> digits)
    {
        Guard.Against.NullOrEmpty(digits);
        Guard.Against.OutOfRange(digits.Count, "Number of digits.", 9, 9);
        StringBuilder builder = new();
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

    public static bool IsNearByOneDigit(List<int> current, List<int?> other)
    {
        Guard.Against.NullOrEmpty(current);
        Guard.Against.NullOrEmpty(other);
        Guard.Against.OutOfRange(current.Count, "Current number length.", other.Count, other.Count);

        var isNear = false;
        for (int i = 0; i < current.Count; i++)
        {
            if (current[i] != other[i])
            {
                if (isNear)
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
