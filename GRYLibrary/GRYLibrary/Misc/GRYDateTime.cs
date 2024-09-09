using System;
using System.Globalization;

namespace GRYLibrary.Core.Misc
{
    /// <summary>
    /// Represents a datetime without milliseconds.
    /// </summary>
    public struct GRYDateTime : IEquatable<GRYDateTime>, IComparable<GRYDateTime>, IComparable
    {
        internal static readonly string DateFormat = "yyyy-MM-dd HH:mm:ss";
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }
        public static GRYDateTime GetCurrentDateTime() => FromDateTime(DateTime.Now);
        public static GRYDateTime GetCurrentDateTimeInUTC() => FromDateTime(DateTime.UtcNow);

        public GRYDateTime(int year, int month, int day, int hour, int minute, int second)
        {
            this.Year = year;
            this.Month = month;
            this.Day = day;
            this.Hour = hour;
            this.Minute = minute;
            this.Second = second;
            this.ToDateTime();//check if datetime is valid
        }
        public readonly DateTime ToDateTime() => new DateTime(this.Year, this.Month, this.Day, this.Hour, this.Minute, this.Second);
        public readonly GRYTime ToGRYTime() => new GRYTime(this.Hour, this.Minute, this.Second);
        public readonly GRYDate ToGRYDate() => new GRYDate(this.Year, this.Month, this.Day);
        public static GRYDateTime FromGRYDateAndTime(GRYDate date, GRYTime time) => new GRYDateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
        public static GRYDateTime FromDateTime(DateTime value) => new GRYDateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
        public static GRYDateTime FromString(string @string) => FromDateTime(DateTime.ParseExact(@string, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None));

        public override readonly int GetHashCode() => base.GetHashCode();

        public override readonly string ToString() => $"{this.Year.ToString().PadLeft(4, '0')}-{this.Month.ToString().PadLeft(2, '0')}-{this.Day.ToString().PadLeft(2, '0')} {this.Hour.ToString().PadLeft(2, '0')}:{this.Minute.ToString().PadLeft(2, '0')}:{this.Second.ToString().PadLeft(2, '0')}";
        public readonly string ToURLEncodedString() => this.ToString().Replace(" ", "%20").Replace(":", "%3A");

        public override readonly bool Equals(object obj) => obj is GRYDateTime time && this.Equals(time);

        public readonly bool Equals(GRYDateTime other) => this.Year == other.Year &&
                   this.Month == other.Month &&
                   this.Day == other.Day &&
                   this.Hour == other.Hour &&
                   this.Minute == other.Minute &&
                   this.Second == other.Second;

        public readonly int CompareTo(GRYDateTime other) => this.ToDateTime().CompareTo(other.ToDateTime());

        public readonly int CompareTo(object obj) => this.ToDateTime().CompareTo(obj);
        public GRYDateTime ToFullHour() => new GRYDateTime(this.Year, this.Month, this.Day, this.Hour, 0, 0);

        public static bool operator ==(GRYDateTime left, GRYDateTime right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GRYDateTime left, GRYDateTime right)
        {
            return !(left == right);
        }

        public static GRYDateTime operator +(GRYDateTime left, TimeSpan right)
        {
            return FromDateTime(left.ToDateTime() + right);
        }

        public static GRYDateTime operator -(GRYDateTime left, TimeSpan right)
        {
            return FromDateTime(left.ToDateTime() - right);
        }

        public static bool operator <(GRYDateTime left, GRYDateTime right)
        {
            return left.ToDateTime() < right.ToDateTime();
        }

        public static bool operator >(GRYDateTime left, GRYDateTime right)
        {
            return left.ToDateTime() > right.ToDateTime();
        }

        public static bool operator <=(GRYDateTime left, GRYDateTime right)
        {
            return left.ToDateTime() <= right.ToDateTime();
        }

        public static bool operator >=(GRYDateTime left, GRYDateTime right)
        {
            return left.ToDateTime() >= right.ToDateTime();
        }
    }
}
