using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataLoader
{
    public static class Extensions
    {
        public static string StripHTMLandURLs(this string input)
        {
           // var replaceurl = Regex.Replace(input, @"(http|ftp|https):\/\/([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?", String.Empty);

            var removedTags = Regex.Replace(input, "&lt;[^&]*&gt;", String.Empty);

            
            var removedSpecialChars = Regex.Replace(removedTags, @"&nbps;|&lt;|&gt;|&amp;", String.Empty);
           return removedSpecialChars;
        }
    }

}
