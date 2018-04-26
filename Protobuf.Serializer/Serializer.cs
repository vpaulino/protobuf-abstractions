using System;
using System.IO;

namespace Protobuf.Serializer
{
    public class Serializer
    {
        /// <summary>
        /// Serialize an instance of an object to a byte array with the protobuf format
        /// </summary>
        /// <typeparam name="T">type of instance of an object</typeparam>
        /// <param name="obj">the object being serialized</param>
        /// <returns>obj serialized</returns>
        public byte[] Serialize<T>(T obj) where T : class
        {
            if (obj == null)
                return null;
            try
            {
                using (var stream = new MemoryStream())
                {
                    ProtoBuf.Serializer.Serialize<T>(stream, obj);
                    return stream.ToArray();
                }
            }
            catch (Exception)
            {
                throw;
            }     
        }

        /// <summary>
        /// Creates an instance of an object from the byte array
        /// </summary>
        /// <typeparam name="T">type being deserialized</typeparam>
        /// <param name="bytes">byte array to be deserialized</param>
        /// <returns>an instance of an object</returns>
        public T Deserialize<T>(byte[] bytes) where T : class
        {

            if (bytes == null)
                return null;
            try
            {
                using (var stream = new MemoryStream(bytes))
                {
                    var instance = ProtoBuf.Serializer.Deserialize<T>(stream);
                    return instance;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
