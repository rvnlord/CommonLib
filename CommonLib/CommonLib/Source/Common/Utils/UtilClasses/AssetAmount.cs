using System;
using CommonLib.Source.Common.Converters;
using CommonLib.Source.Common.Extensions;

namespace CommonLib.Source.Common.Utils.UtilClasses
{
    public class AssetAmount : IComparable<AssetAmount>
    {
        public string Asset { get; set; }
        public decimal Amount { get; set; }

        public AssetAmount(decimal amount, string symbol)
        {
            Amount = amount;
            Asset = symbol;
        }

        public decimal ToDecimal() => Amount;
        public double ToDouble() => Amount.ToDouble();

        public override string ToString() => $"{Amount:0.00000000} {Asset}";
        public override int GetHashCode() => Amount.GetHashCode() ^ Asset.GetHashCodeInvariant();
        public int CompareTo(AssetAmount assetAmount) => assetAmount == null ? 1 : Amount.CompareTo(assetAmount.Amount);
        public int CompareTo(decimal assetAmount) => Amount.CompareTo(assetAmount);

        public override bool Equals(object obj)
        {
            if (obj is not AssetAmount other)
                return false;
            return Amount == other.Amount && Asset == other.Asset;
        }

        public static decimal operator *(AssetAmount amm1, AssetAmount amm2) => amm1?.Multiply(amm2) ?? 0;
        public static decimal operator *(AssetAmount amm1, decimal amm2) => amm1?.Multiply(amm2) ?? 0;
        public decimal Multiply(AssetAmount amm2) => Amount * (amm2?.Amount ?? 0);
        public decimal Multiply(decimal amm2) => Amount * amm2;
        public static decimal operator /(AssetAmount amm1, AssetAmount amm2) => amm1?.Divide(amm2) ?? 0;
        public static decimal operator /(AssetAmount amm1, decimal amm2) => amm1?.Divide(amm2) ?? 0;
        public static decimal operator /(decimal amm1, AssetAmount amm2) => amm1 / (amm2?.Amount ?? 0);
        public decimal Divide(AssetAmount amm2) => Amount / (amm2?.Amount ?? 0);
        public decimal Divide(decimal amm2) => Amount / amm2;
        public static decimal operator +(AssetAmount amm1, AssetAmount amm2) => amm1?.Add(amm2) ?? 0;
        public static decimal operator +(AssetAmount amm1, decimal amm2) => amm1?.Add(amm2) ?? 0;
        public decimal Add(AssetAmount amm2) => Amount + (amm2?.Amount ?? 0);
        public decimal Add(decimal amm2) => Amount + amm2;
        public static decimal operator -(AssetAmount amm1, AssetAmount amm2) => amm1?.Subtract(amm2) ?? 0;
        public static decimal operator -(AssetAmount amm1, decimal amm2) => amm1?.Subtract(amm2) ?? 0;
        public static decimal operator -(decimal amm1, AssetAmount amm2) => amm1 - (amm2?.Amount ?? 0);
        public decimal Subtract(AssetAmount amm2) => Amount - (amm2?.Amount ?? 0);
        public decimal Subtract(decimal amm2) => Amount - amm2;
        
        public static bool operator >(AssetAmount amm1, AssetAmount amm2) => amm1?.CompareTo(amm2) > 0;
        public static bool operator >(AssetAmount amm1, decimal amm2) => amm1?.CompareTo(amm2) > 0;
        public static bool operator <(AssetAmount amm1, AssetAmount amm2) => amm1 is null ? amm2 is { } : amm1.CompareTo(amm2) < 0;
        public static bool operator <(AssetAmount amm1, decimal amm2) => amm1 is null || amm1.CompareTo(amm2) < 0;
        public static bool operator >=(AssetAmount amm1, AssetAmount amm2) => amm1 is null ? amm2 is null : amm1.CompareTo(amm2) >= 0;
        public static bool operator >=(AssetAmount amm1, decimal amm2) => !(amm1 is null) && amm1.CompareTo(amm2) >= 0;
        public static bool operator <=(AssetAmount amm1, AssetAmount amm2) => amm1 is null || amm1.CompareTo(amm2) <= 0;
        public static bool operator <=(AssetAmount amm1, decimal amm2) => amm1 is null || amm1.CompareTo(amm2) <= 0;

        public static bool operator ==(AssetAmount amm1, AssetAmount amm2)
        {
            if (amm1 is null && amm2 is null)
                return true;
            if (amm1 is null || amm2 is null)
                return false;
            return amm1.Amount == amm2.Amount;
        }
        public static bool operator ==(AssetAmount amm1, decimal amm2)
        {
            if (amm1 is null)
                return false;
            return amm1.Amount == amm2;
        }
        public static bool operator !=(AssetAmount amm1, AssetAmount amm2) => !(amm1 == amm2);
        public static bool operator !=(AssetAmount amm1, decimal amm2) => !(amm1 == amm2);
    }
}
