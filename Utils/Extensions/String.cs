using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PodNoms.Api.Utils.Extensions
{
    public static class StringExtensions
    {
        public static string Slugify(this string phrase, IEnumerable<string> source)
        {
            string str = phrase.RemoveAccent().ToLower();
            // invalid chars           
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim 
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            str = Regex.Replace(str, @"\s", "-"); // hyphens   
            str = phrase.RemoveAccent().ToLower();

            str = str.Replace(" ", "");
            var count = 1;
            while (source != null &&
                !string.IsNullOrEmpty(source.Where(e => e == str).Select(e => e).DefaultIfEmpty("").FirstOrDefault()))
            {
                str = $"{str}_{count++}";
            }
            return str;
        }

        public static string RemoveAccent(this string txt)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }
    }
}