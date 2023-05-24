namespace brok1.Instance.Localization
{
    public static class LocalizationExtensions
    {
        public static string ReplaceLocals(this string text, string[] replace)
        {
            foreach (var item in replace)
            {
                int ind = text.IndexOf("{}");
                text = text.Remove(ind, 2).Insert(ind, item);
            }
            return text;
        }
    }
}
