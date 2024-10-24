using System;

namespace GRYLibrary.Core.Misc.Strings
{
    public class HTMLString
    {
        private string _Value;
        public string Value
        {
            get
            {
                return this._Value;
            }
            set
            {
                this._Value = value;
            }
        }


        public static HTMLString From(string value)
        {
            return new HTMLString() { Value = value };
        }

        public override string ToString()
        {
            throw new NotSupportedException($"Not supported. Please use the {Value}-property to access the value of this {this.GetType().FullName}.");
        }
    }
}
