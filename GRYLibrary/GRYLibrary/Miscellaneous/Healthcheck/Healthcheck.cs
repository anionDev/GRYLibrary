using System;
using System.Text;

namespace GRYLibrary.Core.Miscellaneous.Healthcheck
{
    public sealed class Healthcheck : IDisposable
    {
        public string File { get; }
        public bool AddTimestamp { get; set; } = true;
        public Encoding Encoding { get; set; } = new UTF8Encoding(false);
        public Healthcheck(string file)
        {
            this.File = file;
        }
        public void SetState(HealthcheckValue value, string message = "")
        {
            Utilities.EnsureFileExists(File);
            string text;
            if (AddTimestamp)
            {
                text = Utilities.DateTimeToISO8601String(DateTime.Now) + ": ";
            }
            else
            {
                text = string.Empty;
            }
            text = text + Enum.GetName(typeof(HealthcheckValue), value);
            if (!string.IsNullOrWhiteSpace(message))
            {
                text = $"{text} ({message})";
            }
            Utilities.AppendLineToFile(File, text, Encoding);
        }
        public void Dispose()
        {
            SetState(HealthcheckValue.NotRunning, "Disposed");
        }
    }
}
