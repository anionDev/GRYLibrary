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
            this.ToTime();//check if time is valid
        }
        public readonly TimeOnly ToTime()
        {
            return new TimeOnly(this.Hour, this.Minute, this.Second);
        }
        public static GRYTime FromDateTime(TimeOnly value)
        {
            return new GRYTime(value.Hour, value.Minute, value.Second);
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

        public int CompareTo(GRYTime other)
        {
            return this.ToTime().CompareTo(other.ToTime());
        }

        public int CompareTo(object obj)
        {
            return this.ToTime().CompareTo(obj);
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
            return left.ToTime() < right.ToTime();
        }
        public static bool operator >(GRYTime left, GRYTime right)
        {
            return left.ToTime() > right.ToTime();
        }
        public static bool operator <=(GRYTime left, GRYTime right)
        {
            return left.ToTime() <= right.ToTime();
        }
        public static bool operator >=(GRYTime left, GRYTime right)
        {
            return left.ToTime() >= right.ToTime();
        }
    }
}
