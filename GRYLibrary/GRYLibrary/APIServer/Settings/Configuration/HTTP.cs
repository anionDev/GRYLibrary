namespace GRYLibrary.Core.APIServer.Settings.Configuration
{
    public class HTTP : Protocol
    {
        public const ushort DefaultPort=80;
        public static HTTP Create()
        {
            return Create(DefaultPort);
        }
        public static HTTP Create(ushort port)
        {
            HTTP result = new HTTP
            {
                Port = port
            };
            return result;
        }

        public override string GetProtocol()
        {
            return "http";
        }
    }
}
