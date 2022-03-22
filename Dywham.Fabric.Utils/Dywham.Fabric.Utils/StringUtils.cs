using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dywham.Fabric.Utils
{
    public static class StringUtils
    {
        public static string ParallelJoin<T>(string separator, IList<T> values, int maxDegreeOfParallelism = default(int))
        {
            if (values.Count == 0) return string.Empty;

            var dop = maxDegreeOfParallelism == default(int) ? Environment.ProcessorCount : maxDegreeOfParallelism;
            var tasks = new Task[dop];
            var buffers = new StringBuilder[dop];

            for (var p = 0; p < dop; p++)
            {
                buffers[p] = new StringBuilder();

                tasks[p] = new Task(state =>
                {
                    var partNo = (int)state;
                    var buffer = buffers[partNo];
                    var size = values.Count / dop;
                    var start = partNo * size;
                    var end = start + size;
                    var isLast = (partNo == (dop - 1));

                    if (isLast)
                    {
                        end = start + values.Count - size * (dop - 1);
                    }

                    for (var i = start; i < end; i++)
                    {
                        buffer.Append(values[i]);

                        if (i != end - 1)
                        {
                            buffer.Append(separator);
                        }
                    }

                }, p);

                tasks[p].Start();
            }

            Task.WaitAll(tasks);

            // ReSharper disable once CoVariantArrayConversion
            return string.Concat((object[])buffers);
        }

        /// <summary>
        /// Returns a copy of the original string containing only the set of whitelisted characters.
        /// </summary>
        /// <param name="value">The string that will be copied and scrubbed.</param>
        /// <param name="alphas">If true, all alphabetical characters (a-zA-Z) will be preserved; otherwise, they will be removed.</param>
        /// <param name="numerics">If true, all alphabetical characters (a-zA-Z) will be preserved; otherwise, they will be removed.</param>
        /// <param name="dashes">If true, all alphabetical characters (a-zA-Z) will be preserved; otherwise, they will be removed.</param>
        /// <param name="underlines">If true, all alphabetical characters (a-zA-Z) will be preserved; otherwise, they will be removed.</param>
        /// <param name="spaces">If true, all alphabetical characters (a-zA-Z) will be preserved; otherwise, they will be removed.</param>
        /// <param name="periods">If true, all decimal characters (".") will be preserved; otherwise, they will be removed.</param>
        public static string RemoveExcept(this string value, bool alphas = false, bool numerics = false, bool dashes = false, bool underlines = false, bool spaces = false, bool periods = false)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;

            if (new[] { alphas, numerics, dashes, underlines, spaces, periods }.All(x => x == false)) return value;

            var whitelistChars = new HashSet<char>(string.Concat(
                alphas ? "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ" : "",
                numerics ? "0123456789" : "",
                dashes ? "-" : "",
                underlines ? "_" : "",
                periods ? "." : "",
                spaces ? " " : ""
            ).ToCharArray());

            var scrubbedValue = value.Aggregate(new StringBuilder(), (sb, @char) =>
            {
                if (whitelistChars.Contains(@char)) sb.Append(@char);

                return sb;
            }).ToString();

            return scrubbedValue;
        }

        public static Stream GenerateStreamFromString(string data)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);

            writer.Write(data);
            writer.Flush();
            stream.Position = 0;
            
            return stream;
        }
    }
}
