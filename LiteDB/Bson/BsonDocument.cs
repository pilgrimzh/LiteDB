﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace LiteDB
{
    /// <summary>
    /// Represent a document schemeless to use in collections. Based on JObject
    /// </summary>
    public class BsonDocument : BsonObject
    {
        public const int MAX_DOCUMENT_SIZE = 256 * 1024; // limits in 256 max document size to avoid large documents, memory usage and slow performance

        static BsonDocument()
        {
            fastBinaryJSON.BJSON.Parameters.UseExtensions = false;
            //fastBinaryJSON.BJSON.Parameters.IgnoreAttributes.Clear();
            //fastBinaryJSON.BJSON.Parameters.IgnoreAttributes.Add(typeof(BsonIgnoreAttribute));
        }

        public object Id 
        {
            get { return this["_id"].RawValue; }
            set { this["_id"] = new BsonValue(value); } 
        }

        public BsonDocument()
            : base()
        {
        }

        public BsonDocument(byte[] data)
            : base((Dictionary<string, object>)fastBinaryJSON.BJSON.Parse(data))
        {
        }

        public byte[] ToBson()
        {
            var bytes = fastBinaryJSON.BJSON.ToBJSON(this.RawValue);

            if (bytes.Length > MAX_DOCUMENT_SIZE)
                throw new LiteException("Document size too long");

            return bytes;
        }

        /// <summary>
        /// Get a value from a path inside a document. In this first version, only field name as path.
        /// In future, can accept like "Field.SubField.Array[0].NewField"
        /// </summary>
        public object GetFieldValue(string path)
        {
            return this[path].RawValue;
        }
    }
}
