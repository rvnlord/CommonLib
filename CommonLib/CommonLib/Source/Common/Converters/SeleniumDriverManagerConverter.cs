using System;
using System.Threading.Tasks;
using CommonLib.Source.Common.Utils.UtilClasses;

namespace CommonLib.Source.Common.Converters
{
    public static class SeleniumDriverManagerConverter
    {
        public static T Parse<T>(this SeleniumDriverManager sdm, Func<SeleniumDriverManager, T> parser)
        {
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));

            return parser(sdm);
        }

        public static async Task<T> Parse<T>(this Task<SeleniumDriverManager> taskStr, Func<SeleniumDriverManager, T> parser)
        {
            if (taskStr == null)
                throw new ArgumentNullException(nameof(taskStr));

            return (await taskStr.ConfigureAwait(false)).Parse(parser);
        }
    }
}
