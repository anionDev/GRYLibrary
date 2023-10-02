using System;
using System.Globalization;

namespace GRYLibrary.Core.Miscellaneous
{
    /// <summary>
    /// Represents a datetime without milliseconds.
    /// </summary>
    public struct GRYTime : IEquatable<GRYTime>, IComparable<GRYTime>, IComparable
    {
        internal static readonly string TimeFormat = "HH:mm:ss";
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }
        public static GRYTime GetCurrentTime() { return GRYDateTime.GetCurrentDateTime().ToGRYTime(); }
        public static GRYTime GetCurrentTimeInUTC() { return GRYDateTime.GetCurrentDateTimeInUTC().ToGRYTime(); }

        public GRYTime(int hour, int minute, int second)
        {
            this.Hour = hour;
            this.Minute = minute;
            this.Second = second;
            ToTime(this);//check if time is valid
        }
        public static TimeOnly ToTime(GRYTime value)
        {
            if (value == default)
            {
                return TimeOnly.MinValue;
            }
            else
            {
                return new TimeOnly(value.Hour, value.Minute, value.Second);
            }
        }
        public static GRYTime FromDateTime(TimeOnly value)
        {
            return new GRYTime(value.Hour, value.Minute, value.Second);
        }
        public static string ToString(GRYTime dateTime)
        {
            return ToTime(dateTime).ToString(TimeFormat);
        }
        public static GRYTime FromString(string @string)
        {
            return FromDateTime(TimeOnly.ParseExact(@string, TimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None));
        }

        public override readonly int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override readonly string ToString()
        {
            return $"{this.Hour.ToString().PadLeft(2, '0')}:{this.Minute.ToString().PadLeft(2, '0')}:{this.Second.ToString().PadLeft(2, '0')}";
        }
        public readonly string ToString(string format)
        {
            return ToTime(this).ToString(format);
        }

        public override readonly bool Equals(object obj)
        {
            return obj is GRYTime time && this.Equals(time);
        }

        public readonly bool Equals(GRYTime other)
        {
            return this.Hour == other.Hour &&
                   this.Minute == other.Minute &&
                   this.Second == other.Second;
        }

        public readonly int CompareTo(GRYTime other)
        {
            return ToTime(this).CompareTo(ToTime(other));
        }

        public readonly int CompareTo(object obj)
        {
            return ToTime(this).CompareTo(obj);
        }

        public static bool operator ==(GRYTime left, GRYTime right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GRYTime left, GRYTime right)
        {
            return !(left == right);
        }

        public static bool operator <(GRYTime left, GRYTime right)
        {
            return ToTime(left) < ToTime(right);
        }
        public static bool operator >(GRYTime left, GRYTime right)
        {
            return ToTime(left) > ToTime(right);
        }
        public static bool operator <=(GRYTime left, GRYTime right)
        {
            return ToTime(left) <= ToTime(right);
        }
        public static bool operator >=(GRYTime left, GRYTime right)
        {
            return ToTime(left) >= ToTime(right);
        }
    }
}
