using Ardalis.GuardClauses;

namespace Wilczura.Ocr.Domain;

internal class Digit
{
    internal const string Zero = " _ | ||_|";
    internal const string One = "     |  |";
    internal const string Two = " _  _||_ ";
    internal const string Three = " _  _| _|";
    internal const string Four = "   |_|  |";
    internal const string Five = " _ |_  _|";
    internal const string Six = " _ |_ |_|";
    internal const string Seven = " _   |  |";
    internal const string Eight = " _ |_||_|";
    internal const string Nine = " _ |_| _|";

    internal static Dictionary<int, string> DigitsMap = new Dictionary<int, string>
    {
        { 0 , Zero },
        { 1 , One },
        { 2 , Two },
        { 3 , Three },
        { 4 , Four },
        { 5 , Five },
        { 6 , Six },
        { 7 , Seven },
        { 8 , Eight },
        { 9 , Nine }
    };

    internal int? DigitNumber { get; init; }

    private readonly string _digitCode;

    internal Digit(string digitCode)
    {
        _digitCode = digitCode;
        DigitNumber = GetExactDigitNumber();
    }

    public override string ToString()
    {
        return DigitNumber.HasValue ? DigitNumber.Value.ToString() : "?";
    }

    internal int? GetExactDigitNumber()
    {
        int? directMatch = null;
        switch (_digitCode)
        {
            case Digit.Zero: directMatch = 0; break;
            case Digit.One: directMatch = 1; break;
            case Digit.Two: directMatch = 2; break;
            case Digit.Three: directMatch = 3; break;
            case Digit.Four: directMatch = 4; break;
            case Digit.Five: directMatch = 5; break;
            case Digit.Six: directMatch = 6; break;
            case Digit.Seven: directMatch = 7; break;
            case Digit.Eight: directMatch = 8; break;
            case Digit.Nine: directMatch = 9; break;
        }

        return directMatch;
    }

    internal List<Digit> GetNearDigits()
    {
        return GetNearDigitNumbers().Select(dn => new Digit(DigitsMap[dn])).ToList();
    }

    private static bool IsNearByOneSign(string originalDigit, string otherDigit)
    {
        Guard.Against.NullOrEmpty(originalDigit);
        Guard.Against.NullOrEmpty(otherDigit);
        Guard.Against.LengthOutOfRange(otherDigit, originalDigit.Length, originalDigit.Length);

        var isNear = false;
        for (int i = 0; i < originalDigit.Length; i++)
        {
            if (originalDigit[i] != otherDigit[i])
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

    private List<int> GetNearDigitNumbers()
    {
        var digits = new List<int>();

        if (IsNearByOneSign(_digitCode, Digit.Zero)) digits.Add(0);
        if (IsNearByOneSign(_digitCode, Digit.One)) digits.Add(1);
        if (IsNearByOneSign(_digitCode, Digit.Two)) digits.Add(2);
        if (IsNearByOneSign(_digitCode, Digit.Three)) digits.Add(3);
        if (IsNearByOneSign(_digitCode, Digit.Four)) digits.Add(4);
        if (IsNearByOneSign(_digitCode, Digit.Five)) digits.Add(5);
        if (IsNearByOneSign(_digitCode, Digit.Six)) digits.Add(6);
        if (IsNearByOneSign(_digitCode, Digit.Seven)) digits.Add(7);
        if (IsNearByOneSign(_digitCode, Digit.Eight)) digits.Add(8);
        if (IsNearByOneSign(_digitCode, Digit.Nine)) digits.Add(9);

        return digits;
    }
}
