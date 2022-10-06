using System;
using System.ComponentModel;
using System.Linq;
using CommonLib.Source.Common.Extensions;
using CommonLib.Source.Common.Utils.TypeUtils;
using CommonLib.Source.Common.Utils.UtilClasses;

namespace CommonLib.Source.Common.Converters
{
    public static class EnumConverter
    {
        public static string EnumToStringN(this Enum en)
        {
            if (en == null)
                throw new ArgumentNullException(nameof(en));

            return Enum.GetName(en.GetType(), en)?.ReplaceInvariant("_", " ").Trim();
        }

        public static string EnumToString(this Enum en)
        {
            var strEnumN = en.EnumToStringN();
            if (string.IsNullOrWhiteSpace(strEnumN))
                throw new InvalidEnumArgumentException("Enum has no value for the given number");
            return strEnumN;
        }

        public static string EnumToString(this object o) => ((Enum) o).EnumToString();

        public static DdlItem EnumToDdlItem(this Enum en)
        {
            return new DdlItem(en.ToInt(), en.EnumToString());
        }

        public static T? DescriptionToEnumN<T>(this string descr) where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("T must be an Enum");
            var enumVals = EnumUtils.GetValues<T>().ToArray();
            foreach (var ev in enumVals)
                if ((ev as Enum).GetDescription() == descr)
                    return ev;
            return null;
        }

        public static T DescriptionToEnum<T>(this string descr) where T : struct
        {
            return descr.DescriptionToEnumN<T>() ?? throw new NullReferenceException("There is no Enum value that matches the description");
        }

        public static T ToEnum<T>(this object value) where T : struct
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            if (!typeof(T).IsEnum) throw new ArgumentException("T must be an Enum");
            return (T)Enum.Parse(typeof(T), value.ToString().RemoveMany(" ", "-"), true);
        }

        public static T? ToEnumN<T>(this object value) where T : struct
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            if (!typeof(T).IsEnum) throw new ArgumentException("T must be an Enum");
            T? parsedEnum = null;
            try
            {
                parsedEnum = (T)Enum.Parse(typeof(T), value.ToString().RemoveMany(" ", "-"), true);
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is ArgumentException || ex is OverflowException) { }

            return parsedEnum;
        }
    }
}
