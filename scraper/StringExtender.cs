using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TZCompanyScraper
{
    public static class StringExtender
    {
        public static string Clean(this string value, KeyValuePair<string, string>[] replacements)
        {
            var result = value;
            foreach (var pair in replacements)
            {
                result = result.Replace(pair.Key, pair.Value);
            }
            result = Regex.Replace(result, "  *", " ");

            return result;
        }

        public static string EmptyIfNull(this string value)
        {
            return (value ?? String.Empty);

        }
    }
}
