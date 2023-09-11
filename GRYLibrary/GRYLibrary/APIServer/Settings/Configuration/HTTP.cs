namespace GRYLibrary.Core.APIServer.Settings.Configuration
{
    public class HTTP : Protocol
    {
        public static HTTP Create()
        {
            HTTP result = new HTTP
            {
                Port = 80
            };
            return result;
        }

        public override string GetProtocol()
        {
            return "http";
        }
    }
}
