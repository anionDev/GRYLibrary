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
                return _Value;
            }
            set
            {
                _Value = value;
            }
        }


        public static HTMLString From(string value)
        {
            return new HTMLString() { Value = value };
        }

        public override string ToString()
        {
            throw new NotSupportedException();
        }

    }
}
