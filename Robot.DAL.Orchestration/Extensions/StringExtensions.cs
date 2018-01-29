namespace Robot.DAL.Orchestration.Extensions
{
    internal static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static string FormatWith(this string str, params object[] args)
        {
            return string.Format(str, args);
        }
    }
}
