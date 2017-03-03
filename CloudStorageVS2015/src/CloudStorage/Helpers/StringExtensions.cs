using System;

namespace CloudStorage.Helpers
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string text)
        {
            return string.IsNullOrEmpty(text);
        }

        public static bool IsNullOrWhiteSpace(this string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }

        public static string GetControllerName(this string text)
        {
            string result = string.Empty;

            const string controllerSuffix = "Controller";
            if (!text.IsNullOrWhiteSpace() && text.EndsWith(controllerSuffix))
            {
                var index = text.LastIndexOf(controllerSuffix, StringComparison.Ordinal);
                result = text.Substring(0, index);
            }

            return result;
        }
    }
}