using System;
using System.Threading.Tasks;
using CommonLib.Source.Models;
using Nethereum.JsonRpc.Client;
using Nethereum.UI;

namespace CommonLib.Source.Common.Extensions
{
    public static class IEthereumHostProviderExtensions
    {
        public static async Task<ApiResponse<string>> TryEnableProviderAsync(this IEthereumHostProvider ethHostProvider)
        {
            try
            {
                return new ApiResponse<string>(await ethHostProvider.EnableProviderAsync());
            }
            catch (Exception ex) when (ex is TaskCanceledException or ObjectDisposedException or RpcResponseException)
            {
                return new ApiResponse<string>(StatusCodeType.Status401Unauthorized, ex.Message, ex);
            }
        }
    }
}
