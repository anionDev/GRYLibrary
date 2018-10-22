﻿namespace GRYLibrary
{
    public class ExtendedColor
    {
        public static readonly ExtendedColor Black = new ExtendedColor(255, 0, 0, 0);
        public static readonly ExtendedColor White = new ExtendedColor(255, 255, 255, 255);
        public static readonly ExtendedColor Transparency = new ExtendedColor(0, 0, 0, 0);
        private readonly string _ARGBStringValue;
        private readonly string _RGBStringValue;
        public byte A { get; }
        public byte R { get; }
        public byte G { get; }
        public byte B { get; }
        public System.Drawing.Color DrawingColor { get; }
        public System.Windows.Media.Brush Brush { get; }
        public System.Windows.Media.Color MediaColor { get; }
        public int ColorCode { get; }

        public ExtendedColor() : this(0)
        {
        }
        public ExtendedColor(byte a, byte r, byte g, byte b) : this(System.Drawing.Color.FromArgb(a, r, g, b).ToArgb())
        {
        }
        public ExtendedColor(byte r, byte g, byte b) : this(System.Drawing.Color.FromArgb(r, g, b).ToArgb())
        {
        }
        public ExtendedColor(int colorCode)
        {
            this.ColorCode = colorCode;
            this.DrawingColor = System.Drawing.Color.FromArgb(this.ColorCode);
            this.MediaColor = System.Windows.Media.Color.FromArgb(this.DrawingColor.A, this.DrawingColor.R, this.DrawingColor.G, this.DrawingColor.B);
            this._ARGBStringValue = this.ColorCode.ToString("X8");
            this._RGBStringValue = this.DrawingColor.R.ToString("X2") + this.DrawingColor.G.ToString("X2") + this.DrawingColor.B.ToString("X2");
            this.Brush = new System.Windows.Media.SolidColorBrush(this.MediaColor);
            this.A = this.DrawingColor.A;
            this.R = this.DrawingColor.R;
            this.G = this.DrawingColor.G;
            this.B = this.DrawingColor.B;
        }

        public string GetARGBString(bool withNumberSign = false)
        {
            if (withNumberSign)
            {
                return "#" + this._ARGBStringValue;
            }
            else
            {
                return this._ARGBStringValue;
            }
        }
        public string GetRGBString(bool withNumberSign = false)
        {
            if (withNumberSign)
            {
                return "#" + this._RGBStringValue;
            }
            else
            {
                return this._RGBStringValue;
            }
        }
        public override bool Equals(object @object)
        {
            if (@object is ExtendedColor)
            {
                return this.ColorCode == ((ExtendedColor)@object).ColorCode;
            }
            else
            {
                return false;
            }
        }
        public override int GetHashCode()
        {
            return this.ColorCode;
        }

    }
}