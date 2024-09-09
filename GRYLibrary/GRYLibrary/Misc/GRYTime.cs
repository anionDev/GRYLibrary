using System;
using System.Globalization;

namespace GRYLibrary.Core.Misc
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
        public static GRYTime GetCurrentTime() => GRYDateTime.GetCurrentDateTime().ToGRYTime();
        public static GRYTime GetCurrentTimeInUTC() => GRYDateTime.GetCurrentDateTimeInUTC().ToGRYTime();

        public GRYTime(int hour, int minute, int second)
        {
            this.Hour = hour;
            this.Minute = minute;
            this.Second = second;
            this.ToTime();//check if time is valid
        }
        public readonly TimeOnly ToTime() => new TimeOnly(this.Hour, this.Minute, this.Second);
        public static GRYTime FromDateTime(TimeOnly value) => new GRYTime(value.Hour, value.Minute, value.Second);
        public static GRYTime FromString(string @string) => FromDateTime(TimeOnly.ParseExact(@string, TimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None));

        public override readonly int GetHashCode() => base.GetHashCode();

        public override readonly string ToString() => $"{this.Hour.ToString().PadLeft(2, '0')}:{this.Minute.ToString().PadLeft(2, '0')}:{this.Second.ToString().PadLeft(2, '0')}";

        public override readonly bool Equals(object obj) => obj is GRYTime time && this.Equals(time);

        public readonly bool Equals(GRYTime other) => this.Hour == other.Hour &&
                   this.Minute == other.Minute &&
                   this.Second == other.Second;

        public int CompareTo(GRYTime other) => this.ToTime().CompareTo(other.ToTime());

        public int CompareTo(object obj) => this.ToTime().CompareTo(obj);

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
