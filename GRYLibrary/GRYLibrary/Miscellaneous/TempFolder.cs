﻿using System;

namespace GRYLibrary.Core.Miscellaneous
{
    public class TempFolder :IDisposable
    {
        public string Path { get; set; }
        public TempFolder()
        {
            this.Path = System.IO.Path.Join(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString().Substring(0, 8));
            Utilities.EnsureDirectoryExists(this.Path);
        }
        public void Dispose()
        {
            Utilities.EnsureDirectoryDoesNotExist(this.Path);
        }
    }
}