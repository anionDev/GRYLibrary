﻿using System.Text;

namespace GRYLibrary
{
    /// <summary>
    /// Represents a simple Manager for persisting on the file-system and reloading an object.
    /// </summary>
    /// <typeparam name="T">The type of the object which should be persisted.</typeparam>
    public sealed class SimpleObjectPersistence<T> where T : new()
    {
        public T Object { get; set; }
        public Encoding Encoding { get; private set; }
        public string File { get; set; }
        private readonly SimpleGenericXMLSerializer<T> _Serializer = null;
        public SimpleObjectPersistence(string file, Encoding encoding)
        {
            this.File = file;
            this.Encoding = encoding;
            this._Serializer = new SimpleGenericXMLSerializer<T>
            {
                Encoding = this.Encoding
            };
            this.LoadObject();
        }
        public SimpleObjectPersistence(string file, Encoding encoding, T @object) : this(file, encoding)
        {
            this.Object = @object;
            this.SaveObject();
        }
        public SimpleObjectPersistence(string file) : this(file, new UTF8Encoding(false))
        {
        }
        public SimpleObjectPersistence(string file, T @object) : this(file, new UTF8Encoding(false), @object)
        {
        }
        public void LoadObject()
        {
            if (!System.IO.File.Exists(this.File))
            {
                this.ResetObject();
            }
            this.Object = this._Serializer.Deserialize(System.IO.File.ReadAllText(this.File, this.Encoding));
        }

        public void ResetObject()
        {
            this.Object = new T();
            this.SaveObject();
        }

        public void SaveObject()
        {
            Utilities.EnsureFileExists(this.File);
            System.IO.File.WriteAllText(this.File, this._Serializer.SerializeWithIndent(this.Object), this.Encoding);
        }
    }
}
