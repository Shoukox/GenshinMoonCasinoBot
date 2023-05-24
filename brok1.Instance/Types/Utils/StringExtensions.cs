namespace brok1.Instance.Types.Utils
{
    public static class StringExtensions
    {
        public static string ParseHTMLSymbols(this string str)
        {
            if (str == null)
                return "";
            return str.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        }
    }
}
