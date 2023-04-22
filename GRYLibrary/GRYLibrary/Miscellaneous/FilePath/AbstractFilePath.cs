using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace GRYLibrary.Core.Miscellaneous.FilePath
{
    [XmlInclude(typeof(AbstractFilePath))]
    [XmlInclude(typeof(RelativeFilePath))]
    public abstract class AbstractFilePath
    {
        public abstract string FilePath { get; set; }

        internal static AbstractFilePath FromString(string logFile)
        {
            if(Utilities.IsAbsoluteLocalFilePath(logFile))
            {
                return new AbsoluteFilePath() { FilePath = logFile };
            }
            if(Utilities.IsRelativeLocalFilePath(logFile))
            {
                return new RelativeFilePath() { FilePath = logFile };
            }
            throw new ArgumentException($"Cannot calculate path-type of path '{logFile}'.");
        }

        public abstract string GetPath();
        public abstract string GetPath(string basePath);
    }
}
