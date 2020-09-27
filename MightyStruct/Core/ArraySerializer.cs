using System;
using System.Collections.Generic;
using System.IO;

namespace MightyStruct.Core
{
    public class ArraySerializer<T> : ISerializer<T[]>
    {
        public ISerializer<T> Serializer { get; }

        public ArraySerializer(ISerializer<T> serializer)
        {
            Serializer = serializer;
        }

        public T[] ReadFromStream(Stream stream)
        {
            List<T> items = new List<T>();
            while (true)
            {
                try
                {
                    items.Add(Serializer.ReadFromStream(stream));
                }
                catch (Exception)
                {
                    break;
                }
            }

            return items.ToArray();
        }

        public void WriteToStream(Stream stream, T[] values)
        {
            foreach (var value in values)
            {
                Serializer.WriteToStream(stream, value);
            }
        }
    }
}
