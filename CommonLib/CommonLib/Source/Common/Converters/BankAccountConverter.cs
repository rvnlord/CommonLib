using CommonLib.Source.Common.Utils.UtilClasses;

namespace CommonLib.Source.Common.Converters
{
    public static class BankAccountConverter
    {
        public static BankAccount ToBankAccount(this string str) => new BankAccount(str);
    }
}
