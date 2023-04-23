﻿using Newtonsoft.Json.Linq;
using System;
using System.IO;
using YamlDotNet.Core.Tokens;

namespace GRYLibrary.Core.Miscellaneous.FilePath
{
    public class AbsoluteFilePath :AbstractFilePath
    {
        private string _FilePath;
        public override string FilePath
        {
            get
            {
                return this._FilePath;
            }
            set
            {
                if(!Utilities.IsAbsoluteLocalFilePath(value))
                {
                    throw new ArgumentException($"Expected absolute path but was not an absolute path: '{value}'.");
                }
                this._FilePath = value;
            }
        }

        public override string GetPath(string basePath)
        {
            return this.GetPath();
        }

        public override string GetPath()
        {
            return this.FilePath;
        }
    }
}
