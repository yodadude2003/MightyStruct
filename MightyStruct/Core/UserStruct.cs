using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;

namespace MightyStruct.Core
{
    public class UserStruct : DynamicObject, IStruct
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
            Root = Parent?.Root ?? this;

            Stream = stream;

            Attributes = new Dictionary<string, IStruct>();
        }

        public async Task ParseAsync()
        {
            foreach (var attr in (Type as UserType).Attributes)
            {
                var name = attr.Key;
                var type = attr.Value;

                IStruct @struct = type.CreateInstance(this, new SubStream(Stream, Stream.Position));
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

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (Attributes.ContainsKey(binder.Name))
            {
                var @struct = Attributes[binder.Name];
                if (@struct is IPrimitiveStruct)
                    result = (@struct as IPrimitiveStruct).Value;
                else
                    result = @struct;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (Attributes.ContainsKey(binder.Name))
            {
                var @struct = Attributes[binder.Name];
                if (@struct is IPrimitiveStruct)
                {
                    (@struct as IPrimitiveStruct).Value = value;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
