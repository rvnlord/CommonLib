using System;

namespace CommonLib.Source.Common.Converters
{
    public static class AngleConverter
    {
        public static double RadiansToDegrees(this double radians) => radians * (180.0 / Math.PI);
        public static double DegreesToRadians(this double degrees) => Math.PI * degrees / 180.0;
    }
}
