using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer;
using System.IO;
using System.Xml;

namespace GRYLibrary.Core.XMLSerializer
{
    /// <summary>
    /// Represents a very easy usable XML-Serializer.
    /// </summary>
    /// <typeparam name="T">The type of the object which should be serialized.</typeparam>
    public class SimpleGenericXMLSerializer<T> where T : new()
    {

        public string Serialize(T @object)
        {
            IExtendedXmlSerializer serializer = new ConfigurationContainer().UseAutoFormatting()
                                                                .UseOptimizedNamespaces()
                                                                .EnableImplicitTyping(typeof(T))
                                                                .EnableReferences()
                                                                .Create();
            string document = serializer.Serialize(new XmlWriterSettings { Indent = true }, @object);
            return document;

        }
        public T Deserialize(string xml)
        {
            IExtendedXmlSerializer serializer = new ConfigurationContainer().UseAutoFormatting()
                                                                .UseOptimizedNamespaces()
                                                                .EnableImplicitTyping(typeof(T))
                                                                .Create();
            T document = serializer.Deserialize<T>(xml);
            return document;
        }
    }
}