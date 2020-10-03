using MightyStruct.Core;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MightyStruct
{
    class Program
    {

        static async Task Main(string[] args)
        {
            var numbered_string = new UserType(
                "numbered_string",
                new Dictionary<string, IType>()
                {
                    { "num", Types["u1"] },
                    { "str", Types["str"] }
                });

            var array = new IndefiniteArrayType("numbered_string[]", numbered_string,
                (p, i, s) => p.Stream.Position < p.Stream.Length);

            using (var stream = File.Open(args[0], FileMode.Open, FileAccess.ReadWrite))
            {
                var arr = array.CreateInstance(null, stream);
                await arr.ParseAsync();

                dynamic file = arr;

                file[0].str = "hello";
                file[1].num = (byte)0;

                await file.UpdateAsync();
            }
        }
    }
}
