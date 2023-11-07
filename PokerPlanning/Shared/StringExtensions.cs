namespace PokerPlanning.Shared
{
    public static class StringExtensions
    {
        public static int SafeParseInt32(this string toParse)
        {
            return int.TryParse(toParse, out var result) ? result : 0;
        }
        public static double SafeParseDouble(this string toParse)
        {
            return Double.TryParse(toParse, out var result) ? result : 0.00;
        }
        public static bool IsNumeric(this string toParse)
        {
            return int.TryParse(toParse, out _);
        }
    }
}
