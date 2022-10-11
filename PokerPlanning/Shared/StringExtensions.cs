namespace PokerPlanning.Shared
{
    public static class StringExtensions
    {
        public static int SafeParseInt32(this string toParse)
        {
            return int.TryParse(toParse, out var result) ? result : 0;
        }
    }
}
