using System;

namespace GRYLibrary.Core.Miscellaneous
{
    public class TempFile :IDisposable
    {
        public string Path { get; set; }
        public TempFile():this(null)
        {
            Path = System.IO.Path.Join(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString().Substring(0, 8));
            Utilities.EnsureFileExists(Path);
        }
        public TempFile(string extension)
        {
            Path = System.IO.Path.Join(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString().Substring(0, 8));
            if(extension != null)
            {
                Path = $"{Path}.{extension}";
            }
            Utilities.EnsureFileExists(Path);
        }
        public void Dispose()
        {
            Utilities.EnsureFileDoesNotExist(this.Path);
        }
    }
}
