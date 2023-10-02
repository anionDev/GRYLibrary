using System;
using System.Globalization;

namespace GRYLibrary.Core.Miscellaneous
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
        public static GRYDate GetCurrentDate() { return GRYDateTime.GetCurrentDateTime().ToGRYDate(); }
        public static GRYDate GetCurrentDateInUTC() { return GRYDateTime.GetCurrentDateTimeInUTC().ToGRYDate(); }

        public GRYDate(int year, int month, int day)
        {
            this.Year = year;
            this.Month = month;
            this.Day = day;
            ToDate(this);//check if date is valid
        }
        public static DateOnly ToDate(GRYDate value)
        {
            if (value == default)
            {
                return DateOnly.MinValue;
            }
            else
            {
                return new DateOnly(value.Year, value.Month, value.Day);
            }
        }
        public static GRYDate FromDate(DateOnly value)
        {
            return new GRYDate(value.Year, value.Month, value.Day);
        }
        public static string ToString(GRYDate value)
        {
            return ToDate(value).ToString(DateFormat);
        }
        public static GRYDate FromString(string @string)
        {
            return FromDate(DateOnly.ParseExact(@string, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None));
        }

        public override readonly int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override readonly string ToString()
        {
            return $"{this.Year.ToString().PadLeft(4, '0')}-{this.Month.ToString().PadLeft(2, '0')}-{this.Day.ToString().PadLeft(2, '0')}";
        }
        public readonly string ToString(string format)
        {
            return ToDate(this).ToString(format);
        }

        public override readonly bool Equals(object obj)
        {
            return obj is GRYDate time && this.Equals(time);
        }

        public readonly bool Equals(GRYDate other)
        {
            return this.Year == other.Year &&
                   this.Month == other.Month &&
                   this.Day == other.Day;
        }

        public readonly int CompareTo(GRYDate other)
        {
            return ToDate(this).CompareTo(ToDate(other));
        }

        public readonly int CompareTo(object obj)
        {
            return ToDate(this).CompareTo(obj);
        }

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
            return ToDate(left) < ToDate(right);
        }
        public static bool operator >(GRYDate left, GRYDate right)
        {
            return ToDate(left) > ToDate(right);
        }
        public static bool operator <=(GRYDate left, GRYDate right)
        {
            return ToDate(left) <= ToDate(right);
        }
        public static bool operator >=(GRYDate left, GRYDate right)
        {
            return ToDate(left) >= ToDate(right);
        }
    }
}
