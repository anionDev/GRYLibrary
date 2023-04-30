namespace GRYLibrary.Core.GenericWebAPIServer.Settings.Configuration
{
    public class HTTP :Protocol
    {
        public static HTTP Create()
        {
            HTTP result = new HTTP();
            result.Port = 80;
            return result;
        }

        public override string GetProtocol()
        {
            return "http";
        }
    }
}
