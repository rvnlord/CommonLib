using System;
using CommonLib.Source.Common.Extensions.Collections;
using RestSharp;

namespace CommonLib.Source.Common.Extensions
{
    public static class RestRequestExtensions
    {
        public static void RemoveParameter(this RestRequest restRequest, string name)
        {
            if (restRequest == null)
                throw new ArgumentNullException(nameof(restRequest));

            restRequest.Parameters.RemoveBy(x => x.Name == name);
        }
    }
}
