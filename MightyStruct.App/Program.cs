using MightyStruct.Core;
using MightyStruct.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MightyStruct
{
    class Program
    {
        public static Dictionary<string, IType> Types
        { get; } = new Dictionary<string, IType>()
        {
            { "u1", new PrimitiveType<byte>("u1", new UInt8Serializer()) },
            { "u2", new PrimitiveType<ushort>("u2", new UInt16Serializer()) },
            { "u4", new PrimitiveType<uint>("u4", new UInt32Serializer()) },
            { "u8", new PrimitiveType<ulong>("u8", new UInt64Serializer()) },

            { "str", new PrimitiveType<string>("str", new StringSerializer(Encoding.ASCII)) },
        };

        static async Task Main(string[] args)
        {
            var numbered_string = new UserType(
                "numbered_string",
                new Dictionary<string, IType>()
                {
                    { "num", Types["u1"] },
                    { "str", Types["str"] }
                });

            var array = new ArrayType("numbered_string[]", numbered_string,
                (i, s) => i == 0 ? true : s.Parent.Stream.Position < s.Parent.Stream.Length);

            using (var stream = File.Open(args[0], FileMode.Open, FileAccess.ReadWrite))
            {
                var arr = array.CreateInstance(null, stream);
                await arr.ParseAsync();

                dynamic file = arr;

                Console.WriteLine(file[1].str);
            }
        }
    }
}
