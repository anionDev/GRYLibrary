using System;
using System.IO;

namespace GRYLibrary.Core.Miscellaneous.CustomDisposables
{
    public class TemporaryDirectory : CustomDisposable
    {
        public string TemporaryDirectoryPath { get; set; } = null;
        public TemporaryDirectory()
        {
            this.TemporaryDirectoryPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Utilities.EnsureDirectoryExists(this.TemporaryDirectoryPath);
            base.DisposeAction = () => Utilities.EnsureDirectoryDoesNotExist(this.TemporaryDirectoryPath);
        }
    }
}
