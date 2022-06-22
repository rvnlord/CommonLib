using System;
using System.Linq;
using RestSharp;

namespace CommonLib.Source.Common.Extensions.Collections
{
    public static class RestRequestParametersCollectionExtensions
    {
        public static ParametersCollection RemoveBy(this ParametersCollection pc, Func<Parameter, bool> selector)
        {
            foreach (var p in pc.Where(selector))
                pc.RemoveParameter(p);
            return pc;
        }
    }
}
