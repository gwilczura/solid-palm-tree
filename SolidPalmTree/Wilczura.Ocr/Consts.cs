namespace Wilczura.Ocr;

public static class Consts
{

    public const string Illegal = "ILL";
    public const string Error = "Err";
    public const string Ambiguous = "AMB";

    public const string Zero    = " _ | ||_|";
    public const string One     = "     |  |";
    public const string Two     = " _  _||_ ";
    public const string Three   = " _  _| _|";
    public const string Four    = "   |_|  |";
    public const string Five    = " _ |_  _|";
    public const string Six     = " _ |_ |_|";
    public const string Seven   = " _   |  |";
    public const string Eight   = " _ |_||_|";
    public const string Nine    = " _ |_| _|";

    public static Dictionary<int, string> DigitsMap = new Dictionary<int, string>
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

    public static Dictionary<string, int> DigitsReverseMap = new Dictionary<string, int>
    {
        { Zero  , 0 },
        { One   , 1 },
        { Two   , 2 },
        { Three , 3 },
        { Four  , 4 },
        { Five  , 5 },
        { Six   , 6 },
        { Seven , 7 },
        { Eight , 8 },
        { Nine  , 9 }
    };
}
