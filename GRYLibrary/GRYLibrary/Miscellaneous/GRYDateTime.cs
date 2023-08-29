﻿using System;
using System.Globalization;

namespace GRYLibrary.Core.Miscellaneous
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
        public GRYDateTime(int year, int month, int day, int hour, int minute, int second)
        {
            this.Year = year;
            this.Month = month;
            this.Day = day;
            this.Hour = hour;
            this.Minute = minute;
            this.Second = second;
            ToDateTime(this);//check if date is valid
        }
        public static DateTime ToDateTime(GRYDateTime value)
        {
            if (value == default)
            {
                return DateTime.MinValue;
            }
            else
            {
                return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
            }
        }
        public static GRYDateTime FromDateTime(DateTime value)
        {
            return new GRYDateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
        }
        public static string ToString(GRYDateTime dateTime)
        {
            return ToDateTime(dateTime).ToString(DateFormat);
        }
        public static GRYDateTime FromString(string @string)
        {
            return FromDateTime(DateTime.ParseExact(@string, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"{this.Year.ToString().PadLeft(4, '0')}-{this.Month.ToString().PadLeft(2, '0')}-{this.Day.ToString().PadLeft(2, '0')} {this.Hour.ToString().PadLeft(2, '0')}:{this.Minute.ToString().PadLeft(2, '0')}:{this.Second.ToString().PadLeft(2, '0')}";
        }
        public string ToString(string format)
        {
            return ToDateTime(this).ToString(format);
        }

        public override bool Equals(object obj)
        {
            return obj is GRYDateTime time && this.Equals(time);
        }

        public bool Equals(GRYDateTime other)
        {
            return this.Year == other.Year &&
                   this.Month == other.Month &&
                   this.Day == other.Day &&
                   this.Hour == other.Hour &&
                   this.Minute == other.Minute &&
                   this.Second == other.Second;
        }

        public int CompareTo(GRYDateTime other)
        {
            return ToDateTime(this).CompareTo(ToDateTime(other));
        }

        public int CompareTo(object obj)
        {
            return ToDateTime(this).CompareTo(obj);
        }

        public static bool operator ==(GRYDateTime left, GRYDateTime right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GRYDateTime left, GRYDateTime right)
        {
            return !(left == right);
        }

        public static bool operator <(GRYDateTime left, GRYDateTime right)
        {
            return ToDateTime(left) < ToDateTime(right);
        }
        public static bool operator >(GRYDateTime left, GRYDateTime right)
        {
            return ToDateTime(left) > ToDateTime(right);
        }
        public static bool operator <=(GRYDateTime left, GRYDateTime right)
        {
            return ToDateTime(left) <= ToDateTime(right);
        }
        public static bool operator >=(GRYDateTime left, GRYDateTime right)
        {
            return ToDateTime(left) >= ToDateTime(right);
        }
    }
}
