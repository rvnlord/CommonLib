using System;
using System.ComponentModel;
using System.Linq;
using CommonLib.Source.Common.Converters;
using CommonLib.Source.Common.Extensions;
using CommonLib.Source.Common.Extensions.Collections;

namespace CommonLib.Source.Common.Utils.UtilClasses
{
    public class ExtendedTime : IComparable<ExtendedTime>
    {
        private static TimeZoneKind _localTImeZone
        {
            get
            {
                var localTz = TimeZoneInfo.Local.Id;
                if (localTz == "Europe/Warsaw")
                    localTz = "Central European Standard Time";
                return localTz.DescriptionToEnumN<TimeZoneKind>() ?? TimeZoneKind.Unspecified;
            }
        }

        private UnixTimestamp _unixTimestamp;
        private DateTime _rfc1123;
        private TimeZoneKind _timeZone;

        public static ExtendedTime UtcNow => new(DateTime.UtcNow);
        public static ExtendedTime LocalNow => new(DateTime.Now, TimeZoneKind.CurrentLocal);
        public static ExtendedTime FirstUnix => new(UnixTimestamp.First);
        public long Ticks => _rfc1123.Ticks;

        public DateTime Rfc1123
        {
            get => _rfc1123;
            set
            {
                _rfc1123 = value.As(DateTimeKind.Unspecified);
                _unixTimestamp = _rfc1123.ToUnixTimestamp();
            }
        }

        public UnixTimestamp UnixTimestamp
        {
            get => _unixTimestamp;
            set { _unixTimestamp = value; _rfc1123 = value.ToDateTime(); }
        }

        public TimeZoneKind TimeZone
        {
            get => _timeZone;
            set => _timeZone = value == TimeZoneKind.CurrentLocal ? _localTImeZone : value;
        }

        public ExtendedTime(DateTime rfc1123, TimeZoneKind timeZone = TimeZoneKind.UTC)
        {
            Rfc1123 = rfc1123;
            TimeZone = timeZone;
        }

        public ExtendedTime(UnixTimestamp unixTimestamp, TimeZoneKind timeZone = TimeZoneKind.UTC)
        {
            UnixTimestamp = unixTimestamp;
            TimeZone = timeZone;
        }

        public ExtendedTime(double unixTimestamp, TimeZoneKind timeZone = TimeZoneKind.UTC)
        {
            UnixTimestamp = new UnixTimestamp(unixTimestamp);
            TimeZone = timeZone;
        }

        public ExtendedTime(int year, int month, int day, TimeZoneKind timeZone = TimeZoneKind.UTC)
        {
            Rfc1123 = new DateTime(year, month, day);
            TimeZone = timeZone;
        }

        public ExtendedTime(int year, int month, int day, int hour, int minute, int second, TimeZoneKind timeZone = TimeZoneKind.UTC)
        {
            Rfc1123 = new DateTime(year, month, day, hour, minute, second);
            TimeZone = timeZone;
        }

        public ExtendedTime ToTimezone(TimeZoneKind timeZone)
        {
            if (timeZone == TimeZoneKind.CurrentLocal)
                timeZone = _localTImeZone;
            if (timeZone == TimeZone)
                return this;
            var dt = TimeZoneInfo.ConvertTime(_rfc1123,
                TimeZoneInfo.FindSystemTimeZoneById(TimeZone.GetDescription()),
                TimeZoneInfo.FindSystemTimeZoneById(timeZone.GetDescription()));
            return new ExtendedTime(dt, timeZone);
        }

        public ExtendedTime ToLocal()
        {
            return ToTimezone(TimeZoneKind.CurrentLocal);
        }

        public ExtendedTime ToUTC() => ToTimezone(TimeZoneKind.UTC);

        public int CompareTo(ExtendedTime otherTime)
        {
            if (otherTime == null)
                throw new ArgumentNullException(nameof(otherTime));

            return ToUTC().Rfc1123.CompareTo(otherTime.ToUTC().Rfc1123);
        }

        public static ExtendedTime Now(TimeZoneKind timeZone) => new ExtendedTime(DateTime.UtcNow).ToTimezone(timeZone);

        public string ToTimeDateString() => $"{_rfc1123:HH:mm:ss dd-MM-yyyy}";

        public string ToDateTimeString() => $"{_rfc1123:dd-MM-yyyy HH:mm:ss}";

        public string ToString(string format) => _rfc1123.ToString(format);

        public override string ToString() => $"{ToTimeDateString()} {TimeZone.GetDescription()} ({_unixTimestamp})";

        public override bool Equals(object obj)
        {
            var time = obj as ExtendedTime;
            if (time == null)
                return false;
            return ToUTC().Rfc1123 == time.ToUTC().Rfc1123;
        }

        public static bool operator >=(ExtendedTime et1, ExtendedTime et2)
        {
            return et1.CompareTo(et2) >= 0;
        }

        public static bool operator <=(ExtendedTime et1, ExtendedTime et2)
        {
            return et1.CompareTo(et2) <= 0;
        }

        public static bool operator <(ExtendedTime et1, ExtendedTime et2)
        {
            return et1.CompareTo(et2) < 0;
        }

        public static bool operator >(ExtendedTime et1, ExtendedTime et2)
        {
            return et1.CompareTo(et2) > 0;
        }

        public static bool operator ==(ExtendedTime et1, ExtendedTime et2)
        {
            if (et1 is null && et2 is null)
                return true;
            if (et1 is null || et2 is null)
                return false;
            return et1.Equals(et2);
        }

        public static bool operator !=(ExtendedTime et1, ExtendedTime et2)
        {
            return !(et1 == et2);
        }

        public static ExtendedTime operator +(ExtendedTime et, TimeSpan ts)
        {
            if (et == null)
                throw new ArgumentNullException(nameof(et));

            return et.Add(ts);
        }

        public ExtendedTime Add(TimeSpan ts)
        {
            return (Rfc1123 + ts).ToExtendedTime(TimeZone);
        }

        public static TimeSpan operator -(ExtendedTime et1, ExtendedTime et2)
        {
            if (et1 == null)
                throw new ArgumentNullException(nameof(et1));

            return et1.Subtract(et2);
        }

        public TimeSpan Subtract(ExtendedTime et2)
        {
            if (et2 == null)
                throw new ArgumentNullException(nameof(et2));

            return ToUTC().Rfc1123 - et2.ToUTC().Rfc1123;
        }

        public static ExtendedTime operator -(ExtendedTime et, TimeSpan ts)
        {
            if (et == null)
                throw new ArgumentNullException(nameof(et));

            return et.Subtract(ts);
        }

        public ExtendedTime Subtract(TimeSpan ts)
        {
            return (Rfc1123 - ts).ToExtendedTime(TimeZone);
        }

        public ExtendedTime RoundUp(TimeSpan ts) => new(Rfc1123.RoundUp(ts), TimeZone);
        public ExtendedTime RoundDown(TimeSpan ts) => new(Rfc1123.RoundDown(ts), TimeZone);

        public override int GetHashCode() => _unixTimestamp.GetHashCode() ^ _rfc1123.GetHashCode();
        public UnixTimestamp ToUnixTimestamp() => _unixTimestamp;
        public ExtendedTime Clone() => new(Rfc1123, TimeZone);
    }

    public struct UnixTimestamp : IComparable<UnixTimestamp>, IEquatable<UnixTimestamp>
    {
        private readonly double _timeStamp;

        public long Seconds => _timeStamp.ToLong();
        public long MilliSeconds => WithPrecision(13).ToLong();

        public UnixTimestamp(double timeStamp) => _timeStamp = timeStamp;
        public UnixTimestamp(UnixTimestamp timeStamp) => _timeStamp = timeStamp.ToDouble();

        public override string ToString() => _timeStamp.ToStringInvariant();
        public string ToNoDecimalString() => Seconds.ToStringInvariant();

        public string ToString(string decimalSeparator) => _timeStamp.ToStringInvariant(decimalSeparator);

        public string ToNoCommaAccurateString(int length)
        {
            var origTsStr = _timeStamp.ToStringInvariant();
            var tsStr = origTsStr.RemoveMany(",", ".").Take(length);
            var tsLength = tsStr.Length;
            return tsLength < length
                ? $"{tsStr}{Enumerable.Repeat(0, length - tsLength).JoinAsString()}"
                : tsStr;
        }

        public string ToString(uint precision, bool keepDecimals = false)
        {
            var strNonce = WithPrecision(precision).ToStringInvariant().RemoveScientificNotation();
            if (keepDecimals)
                return strNonce;
            return strNonce.BeforeFirst(".");
        }

        public double WithPrecision(uint precision)
        {
            if (precision == 0)
                throw new ArgumentException("Precision can't be 0");

            return _timeStamp * 10d.Pow(precision - 10d);
        }

        public UnixTimestamp SetDecimals(int n)
        {
            return new UnixTimestamp(Math.Round(_timeStamp, n));
        }

        public UnixTimestamp RemoveDecimals()
        {
            return SetDecimals(0);
        }

        public override bool Equals(object obj)
        {
            return obj != null && _timeStamp.Eq(((UnixTimestamp)obj).ToDouble());
        }

        public override int GetHashCode()
        {
            return _timeStamp.GetHashCode();
        }

        public static bool operator >=(UnixTimestamp ut1, UnixTimestamp ut2)
        {
            return ut1.CompareTo(ut2) >= 0;
        }

        public static bool operator <=(UnixTimestamp ut1, UnixTimestamp ut2)
        {
            return ut1.CompareTo(ut2) <= 0;
        }

        public static bool operator <(UnixTimestamp ut1, UnixTimestamp ut2)
        {
            return ut1.CompareTo(ut2) < 0;
        }

        public static bool operator >(UnixTimestamp ut1, UnixTimestamp ut2)
        {
            return ut1.CompareTo(ut2) > 0;
        }

        public static bool operator ==(UnixTimestamp ut1, UnixTimestamp ut2)
        {
            return ut1.Equals(ut2);
        }

        public static bool operator !=(UnixTimestamp ut1, UnixTimestamp ut2)
        {
            return !(ut1 == ut2);
        }

        public static TimeSpan operator -(UnixTimestamp ut1, UnixTimestamp ut2)
        {
            return ut1.Subtract(ut2);
        }

        public TimeSpan Subtract(UnixTimestamp ut2)
        {
            return ToDateTime().Subtract(ut2.ToDateTime());
        }

        public static UnixTimestamp operator +(UnixTimestamp ut, TimeSpan ts)
        {
            return ut.Add(ts);
        }

        public UnixTimestamp Add(TimeSpan ts)
        {
            return ToDateTime().Add(ts).ToUnixTimestamp();
        }

        public static UnixTimestamp operator +(UnixTimestamp ut, long seconds)
        {
            return ut.Add(seconds);
        }

        public UnixTimestamp Add(long seconds)
        {
            return ToDateTime().AddSeconds(seconds).ToUnixTimestamp();
        }

        public static UnixTimestamp operator -(UnixTimestamp ut, long seconds)
        {
            return ut.Subtract(seconds);
        }

        public UnixTimestamp Subtract(long seconds)
        {
            return ToDateTime().AddSeconds(-seconds).ToUnixTimestamp();
        }

        public static UnixTimestamp operator -(UnixTimestamp ut, TimeSpan ts)
        {
            return ut.Subtract(ts);
        }

        public UnixTimestamp Subtract(TimeSpan ts)
        {
            return ToDateTime().Subtract(ts).ToUnixTimestamp();
        }

        public static UnixTimestamp operator ++(UnixTimestamp ut)
        {
            return ut.Increment();
        }

        public UnixTimestamp Increment()
        {
            return Add(1);
        }

        public static UnixTimestamp operator --(UnixTimestamp ut)
        {
            return ut.Decrement();
        }

        public UnixTimestamp Decrement()
        {
            return Subtract(1);
        }

        public int CompareTo(UnixTimestamp ut2)
        {
            return _timeStamp.CompareTo(ut2._timeStamp);
        }

        public long? ToLongN() => _timeStamp.ToLongN();
        public long ToLong() => _timeStamp.ToLong();
        public double? ToDoubleN() => _timeStamp.ToDoubleN();
        public double ToDouble() => _timeStamp.ToDouble();
        public DateTime ToDateTime() => new DateTime(1970, 1, 1).AddSeconds(_timeStamp);
        public ExtendedTime ToExtendedTime(TimeZoneKind timeZone = TimeZoneKind.UTC) => new(this, timeZone);
        public static UnixTimestamp First => new ExtendedTime(new DateTime(1970, 1, 1)).ToUnixTimestamp();
        public static UnixTimestamp UtcNow => DateTime.UtcNow.ToUnixTimestamp();

        public bool Equals(UnixTimestamp other)
        {
            return _timeStamp.Equals(other._timeStamp);
        }

        public static UnixTimestamp Decrement(UnixTimestamp item)
        {
            throw new NotImplementedException();
        }

        public static UnixTimestamp Increment(UnixTimestamp item)
        {
            throw new NotImplementedException();
        }
    }

    public enum TimeZoneKind
    {
        [Description("Unspecified")] Unspecified,
        [Description("Current Local")] CurrentLocal,
        [Description("Dateline Standard Time")] DatelineStandardTime,
        [Description("UTC-11")] UTCm11,
        [Description("Hawaiian Standard Time")] HawaiianStandardTime,
        [Description("Aleutian Standard Time")] AleutianStandardTime,
        [Description("Marquesas Standard Time")] MarquesasStandardTime,
        [Description("Alaskan Standard Time")] AlaskanStandardTime,
        [Description("UTC-09")] UTCm09,
        [Description("Pacific Standard Time")] PacificStandardTime,
        [Description("Pacific Standard Time (Mexico)")] PacificStandardTimeMexico,
        [Description("UTC-08")] UTCm08,
        [Description("US Mountain Standard Time")] USMountainStandardTime,
        [Description("Mountain Standard Time (Mexico)")] MountainStandardTimeMexico,
        [Description("Mountain Standard Time")] MountainStandardTime,
        [Description("Central America Standard Time")] CentralAmericaStandardTime,
        [Description("Central Standard Time (Mexico)")] CentralStandardTimeMexico,
        [Description("Canada Central Standard Time")] CanadaCentralStandardTime,
        [Description("Central Standard Time")] CentralStandardTime,
        [Description("Easter Island Standard Time")] EasterIslandStandardTime,
        [Description("SA Pacific Standard Time")] SAPacificStandardTime,
        [Description("Eastern Standard Time (Mexico)")] EasternStandardTimeMexico,
        [Description("Haiti Standard Time")] HaitiStandardTime,
        [Description("Cuba Standard Time")] CubaStandardTime,
        [Description("US Eastern Standard Time")] USEasternStandardTime,
        [Description("Eastern Standard Time")] EasternStandardTime,
        [Description("Paraguay Standard Time")] ParaguayStandardTime,
        [Description("Venezuela Standard Time")] VenezuelaStandardTime,
        [Description("Central Brazilian Standard Time")] CentralBrazilianStandardTime,
        [Description("Atlantic Standard Time")] AtlanticStandardTime,
        [Description("SA Western Standard Time")] SAWesternStandardTime,
        [Description("Pacific SA Standard Time")] PacificSAStandardTime,
        [Description("Turks And Caicos Standard Time")] TurksAndCaicosStandardTime,
        [Description("Newfoundland Standard Time")] NewfoundlandStandardTime,
        [Description("Tocantins Standard Time")] TocantinsStandardTime,
        [Description("E. South America Standard Time")] ESouthAmericaStandardTime,
        [Description("Argentina Standard Time")] ArgentinaStandardTime,
        [Description("Greenland Standard Time")] GreenlandStandardTime,
        [Description("SA Eastern Standard Time")] SAEasternStandardTime,
        [Description("Montevideo Standard Time")] MontevideoStandardTime,
        [Description("Magallanes Standard Time")] MagallanesStandardTime,
        [Description("Saint Pierre Standard Time")] SaintPierreStandardTime,
        [Description("Bahia Standard Time")] BahiaStandardTime,
        [Description("UTC-02")] UTCm02,
        [Description("Mid-Atlantic Standard Time")] MidAtlanticStandardTime,
        [Description("Azores Standard Time")] AzoresStandardTime,
        [Description("Cape Verde Standard Time")] CapeVerdeStandardTime,
        [Description("UTC")] UTC,
        [Description("Morocco Standard Time")] MoroccoStandardTime,
        [Description("GMT Standard Time")] GMTStandardTime,
        [Description("Greenwich Standard Time")] GreenwichStandardTime,
        [Description("W. Central Africa Standard Time")] WCentralAfricaStandardTime,
        [Description("W. Europe Standard Time")] WEuropeStandardTime,
        [Description("Central Europe Standard Time")] CentralEuropeStandardTime,
        [Description("Romance Standard Time")] RomanceStandardTime,
        [Description("Central European Standard Time")] CentralEuropeanStandardTime,
        [Description("Namibia Standard Time")] NamibiaStandardTime,
        [Description("Jordan Standard Time")] JordanStandardTime,
        [Description("GTB Standard Time")] GTBStandardTime,
        [Description("Middle East Standard Time")] MiddleEastStandardTime,
        [Description("Syria Standard Time")] SyriaStandardTime,
        [Description("West Bank Standard Time")] WestBankStandardTime,
        [Description("South Africa Standard Time")] SouthAfricaStandardTime,
        [Description("FLE Standard Time")] FLEStandardTime,
        [Description("Israel Standard Time")] IsraelStandardTime,
        [Description("Egypt Standard Time")] EgyptStandardTime,
        [Description("Kaliningrad Standard Time")] KaliningradStandardTime,
        [Description("E. Europe Standard Time")] EEuropeStandardTime,
        [Description("Libya Standard Time")] LibyaStandardTime,
        [Description("Arabic Standard Time")] ArabicStandardTime,
        [Description("Arab Standard Time")] ArabStandardTime,
        [Description("Belarus Standard Time")] BelarusStandardTime,
        [Description("Russian Standard Time")] RussianStandardTime,
        [Description("E. Africa Standard Time")] EAfricaStandardTime,
        [Description("Turkey Standard Time")] TurkeyStandardTime,
        [Description("Iran Standard Time")] IranStandardTime,
        [Description("Arabian Standard Time")] ArabianStandardTime,
        [Description("Astrakhan Standard Time")] AstrakhanStandardTime,
        [Description("Azerbaijan Standard Time")] AzerbaijanStandardTime,
        [Description("Caucasus Standard Time")] CaucasusStandardTime,
        [Description("Russia Time Zone 3")] RussiaTimeZone3,
        [Description("Mauritius Standard Time")] MauritiusStandardTime,
        [Description("Saratov Standard Time")] SaratovStandardTime,
        [Description("Georgian Standard Time")] GeorgianStandardTime,
        [Description("Afghanistan Standard Time")] AfghanistanStandardTime,
        [Description("West Asia Standard Time")] WestAsiaStandardTime,
        [Description("Pakistan Standard Time")] PakistanStandardTime,
        [Description("Ekaterinburg Standard Time")] EkaterinburgStandardTime,
        [Description("India Standard Time")] IndiaStandardTime,
        [Description("Sri Lanka Standard Time")] SriLankaStandardTime,
        [Description("Nepal Standard Time")] NepalStandardTime,
        [Description("Central Asia Standard Time")] CentralAsiaStandardTime,
        [Description("Bangladesh Standard Time")] BangladeshStandardTime,
        [Description("Omsk Standard Time")] OmskStandardTime,
        [Description("Myanmar Standard Time")] MyanmarStandardTime,
        [Description("W. Mongolia Standard Time")] WMongoliaStandardTime,
        [Description("SE Asia Standard Time")] SEAsiaStandardTime,
        [Description("Altai Standard Time")] AltaiStandardTime,
        [Description("North Asia Standard Time")] NorthAsiaStandardTime,
        [Description("N. Central Asia Standard Time")] NCentralAsiaStandardTime,
        [Description("Tomsk Standard Time")] TomskStandardTime,
        [Description("North Asia East Standard Time")] NorthAsiaEastStandardTime,
        [Description("Singapore Standard Time")] SingaporeStandardTime,
        [Description("China Standard Time")] ChinaStandardTime,
        [Description("W. Australia Standard Time")] WAustraliaStandardTime,
        [Description("Taipei Standard Time")] TaipeiStandardTime,
        [Description("Ulaanbaatar Standard Time")] UlaanbaatarStandardTime,
        [Description("North Korea Standard Time")] NorthKoreaStandardTime,
        [Description("Aus Central W. Standard Time")] AusCentralWStandardTime,
        [Description("Transbaikal Standard Time")] TransbaikalStandardTime,
        [Description("Yakutsk Standard Time")] YakutskStandardTime,
        [Description("Tokyo Standard Time")] TokyoStandardTime,
        [Description("Korea Standard Time")] KoreaStandardTime,
        [Description("Cen. Australia Standard Time")] CenAustraliaStandardTime,
        [Description("AUS Central Standard Time")] AUSCentralStandardTime,
        [Description("E. Australia Standard Time")] EAustraliaStandardTime,
        [Description("AUS Eastern Standard Time")] AUSEasternStandardTime,
        [Description("West Pacific Standard Time")] WestPacificStandardTime,
        [Description("Tasmania Standard Time")] TasmaniaStandardTime,
        [Description("Vladivostok Standard Time")] VladivostokStandardTime,
        [Description("Lord Howe Standard Time")] LordHoweStandardTime,
        [Description("Russia Time Zone 10")] RussiaTimeZone10,
        [Description("Magadan Standard Time")] MagadanStandardTime,
        [Description("Sakhalin Standard Time")] SakhalinStandardTime,
        [Description("Bougainville Standard Time")] BougainvilleStandardTime,
        [Description("Norfolk Standard Time")] NorfolkStandardTime,
        [Description("Central Pacific Standard Time")] CentralPacificStandardTime,
        [Description("Russia Time Zone 11")] RussiaTimeZone11,
        [Description("New Zealand Standard Time")] NewZealandStandardTime,
        [Description("Fiji Standard Time")] FijiStandardTime,
        [Description("Kamchatka Standard Time")] KamchatkaStandardTime,
        [Description("UTC+12")] UTCp12,
        [Description("Chatham Islands Standard Time")] ChathamIslandsStandardTime,
        [Description("Tonga Standard Time")] TongaStandardTime,
        [Description("Samoa Standard Time")] SamoaStandardTime,
        [Description("UTC+13")] UTCp13,
        [Description("Line Islands Standard Time")] LineIslandsStandardTime
    }
}
