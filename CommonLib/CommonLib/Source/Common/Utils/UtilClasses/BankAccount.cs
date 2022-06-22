using System;
using CommonLib.Source.Common.Converters;
using CommonLib.Source.Common.Extensions;

namespace CommonLib.Source.Common.Utils.UtilClasses
{
    public struct BankAccount : IEquatable<BankAccount>
    {
        public string CountryCode { get; set; }
        private readonly int _p1; // 2
        private readonly int _p2; // 6
        private readonly int _p3; // 10
        private readonly int _p4; // 14
        private readonly int _p5; // 18
        private readonly int _p6; // 22
        private readonly int _p7; // 26

        public BankAccount(string bankAccountNumber)
        {
            CountryCode = "";
            var bankAccStr = bankAccountNumber.Remove(" ");
            if (!bankAccStr.ContainsOnlyDigits() && bankAccStr.Length == 28)
            {
                CountryCode = bankAccStr.Take(2);
                bankAccStr = bankAccStr.Skip(2);
            }
            if (!bankAccStr.ContainsOnlyDigits() || bankAccStr.Length != 26)
                throw new ArgumentException($"{nameof(bankAccountNumber)} should contain only digits and be exactly 26 characters long");
            _p1 = bankAccStr.Take(2).ToInt();
            _p2 = bankAccStr.Skip(2).Take(4).ToInt();
            _p3 = bankAccStr.Skip(6).Take(4).ToInt();
            _p4 = bankAccStr.Skip(10).Take(4).ToInt();
            _p5 = bankAccStr.Skip(14).Take(4).ToInt();
            _p6 = bankAccStr.Skip(18).Take(4).ToInt();
            _p7 = bankAccStr.Skip(22).Take(4).ToInt();
        }

        public override string ToString() => $"{_p1:00} {_p2:0000} {_p3:0000} {_p4:0000} {_p5:0000} {_p6:0000} {_p7:0000}";

        public bool Equals(BankAccount other)
        {
            return _p1 == other._p1 && _p2 == other._p2 && _p3 == other._p3 && _p4 == other._p4 && _p5 == other._p5 && _p6 == other._p6 && _p7 == other._p7 && CountryCode == other.CountryCode;
        }

        public override bool Equals(object obj)
        {
            return obj is BankAccount other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _p1;
                hashCode = (hashCode * 397) ^ _p2;
                hashCode = (hashCode * 397) ^ _p3;
                hashCode = (hashCode * 397) ^ _p4;
                hashCode = (hashCode * 397) ^ _p5;
                hashCode = (hashCode * 397) ^ _p6;
                hashCode = (hashCode * 397) ^ _p7;
                hashCode = (hashCode * 397) ^ (CountryCode != null ? CountryCode.GetHashCodeInvariant() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(BankAccount left, BankAccount right) => left.Equals(right);
        public static bool operator !=(BankAccount left, BankAccount right) => !(left == right);
    }
}
