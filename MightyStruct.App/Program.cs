using MightyStruct.Abstractions;
using MightyStruct.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MightyStruct
{
    class Program
    {

        static async Task Main(string[] args)
        {
            using (var typeStream = File.OpenRead(args[0]))
            using (var fileStream = File.Open(args[1], FileMode.Open, FileAccess.ReadWrite))
            {
                var type = Parser.ParseFromStream(typeStream);

                dynamic file = type.CreateInstance(new Context(null, null, fileStream));
                await file.ParseAsync();

                foreach (var num_string in file.arr)
                {
                    Console.WriteLine($"num: {num_string.num}, str: \"{num_string.str}\"");
                }
            }
        }
    }
}
