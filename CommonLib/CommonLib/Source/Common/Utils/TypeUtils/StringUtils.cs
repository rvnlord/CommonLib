using System.Linq;
using CommonLib.Source.Common.Extensions.Collections;

namespace CommonLib.Source.Common.Utils.TypeUtils
{
    public static class StringUtils
    {
        public static int[] ReverseLookupAlphabet(string alphabet)
        {
            var invAlphabet = new int[alphabet.Max() + 1];
            for (var i = 0; i < invAlphabet.Length; i++)
                invAlphabet[i] = -1;
            for (var i = 0; i < alphabet.Length; i++) // number of alphabet characters
                invAlphabet[alphabet[i]] = i;
            return invAlphabet;
        }

        public static string Repeat(this string s, int n) => Enumerable.Repeat(" ", n).JoinAsString();
    }
}