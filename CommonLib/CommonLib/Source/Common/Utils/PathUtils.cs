using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLib.Source.Common.Extensions;
using MoreLinq;

namespace CommonLib.Source.Common.Utils
{
    public static class PathUtils
    {
        public static string Combine(PathSeparator separator, params string[] paths)
        {
            var sb = new StringBuilder();
            foreach (var path in paths)
            {
                var p = path.Replace(@"\", "/");
                var s = sb.ToString();

                if (p.StartsWith("../"))
                {
                    s = sb.ToString().SkipLastWhile(c => c == '/');
                    while (p.StartsWith("../"))
                    {
                        s = s.BeforeLastOrWhole("/");
                        p = p.AfterFirst("../");
                    }
                }
                
                sb.Clear();
                p = p.Replace("./", "/").Replace("//", "/");
                if (p.Contains(':') && p.IndexOfInvariant(":") != 1) // ':' can be part of drive letter (G:\) or url (https://), it needs to be replaced with double slash only in the latter case
                    p = p.Replace(":/", "://");
                while (p.StartsWithInvariant("~/"))
                    p = p.Skip(2);
                var concat = $"{s}{(s.IsNullOrWhiteSpace() || p.StartsWith('.') ? "" : "/")}{p.TrimStart('/')}";
                concat = concat.Replace("://", ">>|&&<<").Replace("//", "/").Replace(">>|&&<<", "://");
                sb.Append(concat);
            }
            
            if (sb.ToString().StartsWith("/"))
                sb.Insert(0, ".");
            
            return separator == PathSeparator.BSlash ? sb.ToString().Replace("/", @"\") : sb.ToString();
        }
    }

    public enum PathSeparator
    {
        BSlash,
        FSlash
    }
}
