using MightyStruct.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace MightyStruct.Serializers
{
    public class StringSerializer : ISerializer<string>
    {
        public Encoding Encoding { get; }

        public StringSerializer(Encoding encoding)
        {
            Encoding = encoding;
        }

        public string ReadFromStream(Stream stream)
        {
            List<byte> str = new List<byte>();

            int ch;
            while ((ch = stream.ReadByte()) > 0)
            {
                str.Add((byte)ch);
            }

            return Encoding.GetString(str.ToArray());
        }

        public void WriteToStream(Stream stream, string value)
        {
            List<byte> buffer = Encoding.GetBytes(value).ToList(); buffer.Add(0);
            stream.Write(buffer.ToArray(), 0, buffer.Count);
        }
    }
}
