using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MightyStruct.Core
{
    public class UserStruct : IStruct
    {
        public IType Type { get; }

        public IStruct Parent { get; }
        public IStruct Root { get; }

        public Stream Stream { get; }

        public Dictionary<string, IStruct> Attributes { get; }

        public UserStruct(IType type, IStruct parent, Stream stream)
        {
            Type = type;

            Parent = parent;
            Root = Parent?.Root;

            Attributes = new Dictionary<string, IStruct>();
        }

        public async Task ParseAsync()
        {
            foreach (var attr in (Type as UserType).Attributes)
            {
                var name = attr.Key;
                var type = attr.Value;

                IStruct @struct;
                if (type is IPrimitiveType)
                {
                    @struct = (type as IPrimitiveType).CreateInstance(this, new SubStream(Stream, Stream.Position));
                }
                else
                {
                    @struct = new UserStruct(type, this, new SubStream(Stream, Stream.Position));
                }

                await @struct.ParseAsync();

                Attributes.Add(name, @struct);
            }
        }

        public async Task UpdateAsync()
        {
            foreach (var attr in Attributes)
            {
                var @struct = attr.Value;
                await @struct.UpdateAsync();
            }
        }
    }
}
