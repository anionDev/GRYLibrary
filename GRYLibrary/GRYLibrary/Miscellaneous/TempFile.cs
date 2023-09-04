using System;

namespace GRYLibrary.Core.Miscellaneous
{
    public sealed class TempFile :IDisposable
    {
        public string Path { get; set; }
        public TempFile():this(null)
        {
            this.Path = System.IO.Path.Join(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString()[..8]);
            Utilities.EnsureFileExists(this.Path);
        }
        public TempFile(string extension)
        {
            this.Path = System.IO.Path.Join(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString()[..8]);
            if(extension != null)
            {
                this.Path = $"{this.Path}.{extension}";
            }
            Utilities.EnsureFileExists(this.Path);
        }
        public void Dispose()
        {
            Utilities.EnsureFileDoesNotExist(this.Path);
        }
    }
}
