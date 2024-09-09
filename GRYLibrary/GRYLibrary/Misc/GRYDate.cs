using System;
using System.Globalization;

namespace GRYLibrary.Core.Misc
{
    /// <summary>
    /// Represents a datetime without milliseconds.
    /// </summary>
    public struct GRYDate : IEquatable<GRYDate>, IComparable<GRYDate>, IComparable
    {
        internal static readonly string DateFormat = "yyyy-MM-dd";
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public static GRYDate GetCurrentDate() => GRYDateTime.GetCurrentDateTime().ToGRYDate();
        public static GRYDate GetCurrentDateInUTC() => GRYDateTime.GetCurrentDateTimeInUTC().ToGRYDate();

        public GRYDate(int year, int month, int day)
        {
            this.Year = year;
            this.Month = month;
            this.Day = day;
            this.ToDate();//check if date is valid
        }
        public readonly DateOnly ToDate() => new DateOnly(this.Year, this.Month, this.Day);
        public static GRYDate FromDate(DateOnly value) => new GRYDate(value.Year, value.Month, value.Day);
        public static GRYDate FromString(string @string) => FromDate(DateOnly.ParseExact(@string, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None));

        public override readonly int GetHashCode() => base.GetHashCode();

        public override readonly string ToString() => $"{this.Year.ToString().PadLeft(4, '0')}-{this.Month.ToString().PadLeft(2, '0')}-{this.Day.ToString().PadLeft(2, '0')}";
        public override readonly bool Equals(object obj) => obj is GRYDate time && this.Equals(time);

        public readonly bool Equals(GRYDate other) => this.Year == other.Year &&
                   this.Month == other.Month &&
                   this.Day == other.Day;

        public int CompareTo(GRYDate other) => this.ToDate().CompareTo(other.ToDate());

        public int CompareTo(object obj) => this.ToDate().CompareTo(obj);

        public static bool operator ==(GRYDate left, GRYDate right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GRYDate left, GRYDate right)
        {
            return !(left == right);
        }

        public static bool operator <(GRYDate left, GRYDate right)
        {
            return left.ToDate() < right.ToDate();
        }
        public static bool operator >(GRYDate left, GRYDate right)
        {
            return left.ToDate() > right.ToDate();
        }
        public static bool operator <=(GRYDate left, GRYDate right)
        {
            return left.ToDate() <= right.ToDate();
        }
        public static bool operator >=(GRYDate left, GRYDate right)
        {
            return left.ToDate() >= right.ToDate();
        }
    }
}
