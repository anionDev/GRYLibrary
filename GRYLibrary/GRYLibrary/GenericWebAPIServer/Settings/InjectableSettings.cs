
namespace GRYLibrary.Core.GenericWebAPIServer.Settings
{
    public interface IInjectableSettings
    {
        public string Address { get; }
    }
    public class InjectableSettings :IInjectableSettings
    {
        public string Address { get; private set; }
        public InjectableSettings(string address)
        {
            this.Address = address;
        }
    }
}
