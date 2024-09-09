using GRYLibrary.Core.AOA;
using GRYLibrary.Core.AOA.SerializeHelper;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

namespace GRYLibrary.Core.XMLSerializer
{
    /// <summary>
    /// Represents a key-value-pair which is serializable by implementing <see cref="IGRYSerializable"/>.
    /// </summary>
    public class KeyValuePair<TKey, TValue> : IGRYSerializable
    {
        public KeyValuePair()
        {
        }
        public KeyValuePair(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }
        public TKey Key { get; set; }

        public TValue Value { get; set; }

        public System.Collections.Generic.KeyValuePair<object, object> ToDotNetKeyValuePair() => new System.Collections.Generic.KeyValuePair<object, object>(this.Key, this.Value);

        #region Overhead
        public override bool Equals(object @object) => Generic.GenericEquals(this, @object);

        public override int GetHashCode() => Generic.GenericGetHashCode(this);

        public override string ToString() => Generic.GenericToString(this);

        public XmlSchema GetSchema() => Generic.GenericGetSchema(this);

        public void ReadXml(XmlReader reader) => Generic.GenericReadXml(this, reader);

        public void WriteXml(XmlWriter writer) => Generic.GenericWriteXml(this, writer);

        public ISet<Type> GetExtraTypesWhichAreRequiredForSerialization() => new HashSet<Type>();
        #endregion
    }
}