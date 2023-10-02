using GRYLibrary.Core.Log;

namespace GRYLibrary.Core.GeneralPurposeLogger
{
    public interface IServiceLogger:IGeneralLogger
    {
        public GRYLog Logger { get; set; }
    }
}
