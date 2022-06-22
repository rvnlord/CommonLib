using System.Collections.Generic;
using System.Linq;
using NBitcoin;
using QBitNinja.Client;

namespace CommonLib.Source.Common.Extensions
{
    public static class BitcoinPubKeyAddressExtensions
    {
        private static readonly QBitNinjaClient _client = new QBitNinjaClient(Network.Main);

        public static decimal GetBalance(this BitcoinAddress address)
        {
            var balanceModel = _client.GetBalance(address, true).Result;
            decimal balance = 0;
            if (balanceModel.Operations.Count <= 0) 
                return balance;

            var unspentCoins = new List<Coin>();
            foreach (var operation in balanceModel.Operations)
                unspentCoins.AddRange(operation.ReceivedCoins.Select(coin => coin as Coin));
            balance = unspentCoins.Sum(x => x.Amount.ToDecimal(MoneyUnit.BTC));
            return balance;
        }
    }
}
