using System;

namespace CommonLib.Source.Common.Utils
{
    public static class ZUtils
    {
        public static void EnsureLicensed()
        {
            Z.EntityFramework.Extensions.LicenseManager.AddLicense("453;100-MyZZZExtensions", "0898915f-770a-555c-8230-312a30ca53e3");
            Z.BulkOperations.LicenseManager.AddLicense("453;300-MyZZZExtensions", "92700e07-e982-73b7-eba1-031178a0a0ff");
            Z.Expressions.EvalManager.AddLicense("591;400-DELTAFOX", "178117B-319C725-33BC307-F4F33A3-E0B2"); // "451;500-DELTAFOX", "D1F01D2-FCE70F3-B8CE373-11EB816-0998"
            Z.Expressions.CompilerManager.AddLicense("710;400-DELTAFOX", "7138308-E02E6B1-04EE951-5450431-41A6");
            if (!Z.EntityFramework.Extensions.LicenseManager.ValidateLicense() 
                || !Z.BulkOperations.LicenseManager.ValidateLicense()
                || !Z.Expressions.EvalManager.ValidateLicense()
                || !Z.Expressions.CompilerManager.ValidateLicense())
                throw new Exception("ZProjects License is not valid");
        }
    }
}
