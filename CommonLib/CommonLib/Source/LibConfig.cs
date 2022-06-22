using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using CommonLib.Source.Common.Converters;
using CommonLib.Source.Common.Utils.TypeUtils;
using NLog;
using Org.BouncyCastle.Math;

namespace CommonLib.Source
{
    public static class LibConfig
    {
        public static readonly Random _r = new();
        //public static readonly Logger _logger = LogUtils.Logger;

        public const string Space = " ";
        public const string Dot = ".";
        public const string Comma = ",";
        public const string PszBase58 = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        public static IReadOnlyList<string> Numbers { get; } = "1234567890".ToArray().Select(c => c.ToStringInvariant()).ToArray();
        public static IReadOnlyList<string> Operators { get; } = "+-/*".ToArray().Select(c => c.ToStringInvariant()).ToArray();
        public static IReadOnlyList<string> NumbersAndOperators => Numbers.Concat(Operators).ToArray();

        public const double TOLERANCE = 0.00001;

        public static readonly object _globalSync = new();
        
        public static readonly char[] PszBase58Chars = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz".ToCharArray();

        public static readonly Dictionary<string, bool> PanelAnimations = new();

        public static readonly BigInteger Bn58 = BigInteger.ValueOf(58);
        
        public static CultureInfo CULTURE { get; set; } = new CultureInfo("pl-PL")
        {
            NumberFormat = new NumberFormatInfo { NumberDecimalSeparator = "." },
            DateTimeFormat = { ShortDatePattern = "dd-MM-yyyy" } // nie tworzyć nowego obiektu DateTimeFormat tutaj tylko przypisać jego interesujące nas właściwości, bo nowy obiekt nieokreślone właściwości zainicjalizuje wartościami dla InvariantCulture, czyli angielskie nazwy dni, miesięcy itd.
        };

        public static readonly MethodInfo CloneMethod = typeof(object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);

    }
}
