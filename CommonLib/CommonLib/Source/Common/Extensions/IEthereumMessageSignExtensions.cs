using System;
using System.Threading.Tasks;
using CommonLib.Source.Models;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.AccountSigning;

namespace CommonLib.Source.Common.Extensions
{
    public static class IEthereumMessageSignExtensions
    {
        public static async Task<ApiResponse<string>> TrySendRequestAsync(this IEthereumMessageSign ethMessageSign, string message, object id = null)
        {
            try
            {
                return new ApiResponse<string>(await ethMessageSign.SendRequestAsync(new HexUTF8String(message), id));
            }
            catch (Exception ex) when (ex is TaskCanceledException or ObjectDisposedException or RpcResponseException)
            {
                if (ex.Message.ContainsIgnoreCase("Metamask Message Signature"))
                    return new ApiResponse<string>(StatusCodeType.Status401Unauthorized, ex.Message, ex);
                throw;
            }
        }
    }
}
