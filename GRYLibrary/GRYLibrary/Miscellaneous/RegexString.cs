using System;
using System.Text.RegularExpressions;

namespace GRYLibrary.Core.Miscellaneous
{
    public class RegexString
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
                if(_Regex.IsMatch(value))
                {
                    _Value = value;
                }
                else
                {
                    throw new ArgumentException($"Value \"{_Value}\" is not assignable because it does not match the regex \"{_Regex}\".");
                }
            }
        }
        private Regex _Regex;
        public Regex GetRegex()
        {
            return _Regex;
        }
        public void SetRegex(Regex regex)
        {
            _Regex = regex;
        }
    }
}
