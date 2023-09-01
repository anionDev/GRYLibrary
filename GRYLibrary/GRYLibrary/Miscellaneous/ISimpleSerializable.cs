namespace GRYLibrary.Core.Miscellaneous
{
    public interface ISimpleSerializable
    {
        public void DeserializeFromString(string content);
        public string SerializeToString();
    }
}
