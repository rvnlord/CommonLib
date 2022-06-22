using CommonLib.Source.Common.Utils.TypeUtils;

namespace CommonLib.Source.Common.Converters
{
    public static class Base256Converter
    {
        public const string _alphabet = "!#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~ ¡¢£¤¥¦§¨©ª«¬­®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿĀāĂăĄąĆćĈĉĊċČčĎďĐđĒēĔĕĖėĘęĚěĜĝĞğĠġĢģĤĥĦħĨĩĪīĬĭĮįİıĲĳĴĵĶķĸĹĺĻļĽľĿŀŁł";
        private static readonly int[] _invAlphabet = StringUtils.ReverseLookupAlphabet(_alphabet);

        public static string ToBase256String(this byte[] data)
        {
            var result = new char[data.Length];
            for (var i = 0; i < data.Length; i++)
                result[i] = _alphabet[data[i]];
            return new string(result);
        }

        public static byte[] Base256ToByteArray(this string data)
        {
            var result = new byte[data.Length];
            for (var i = 0; i < data.Length; i++)
                result[i] = (byte)_invAlphabet[data[i]];
            return result;
        }
    }
}
