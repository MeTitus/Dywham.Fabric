using System;
using System.Linq;

namespace Dywham.Fabric.Utils
{
    public static class StringExtensionMethods
    {
        public static string Scramble(this string text)
        {
            var random = new Random();

            return string.Join("", text.Split().OrderBy(x => random.Next()).ToArray());
        }
    }
}