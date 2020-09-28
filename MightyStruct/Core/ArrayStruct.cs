using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;

namespace MightyStruct.Core
{
    public class ArrayStruct : DynamicObject, IStruct
    {
        public IType Type { get; }
        public ArrayType ArrayType { get; }

        public IStruct Parent { get; }
        public IStruct Root { get; }

        public Stream Stream { get; }

        public List<IStruct> Items { get; }

        public ArrayStruct(IType type, IStruct parent, Stream stream)
        {
            Type = type;
            ArrayType = Type as ArrayType;

            Parent = parent;
            Root = Parent?.Root;

            Stream = stream;

            Items = new List<IStruct>();
        }

        public async Task ParseAsync()
        {
            int index = 0;
            IStruct prevStruct = null;

            while (ArrayType.Condition(index, prevStruct))
            {
                IStruct @struct = ArrayType.BaseType.CreateInstance(this, new SubStream(Stream, Stream.Position));
                await @struct.ParseAsync();
                Items.Add(@struct);

                prevStruct = @struct;
                index++;
            }
        }

        public async Task UpdateAsync()
        {
            foreach (var item in Items)
            {
                await item.UpdateAsync();
            }
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            int idx = (int)indexes[0];
            var item = Items[idx];
            if (item is IPrimitiveStruct)
            {
                result = (item as IPrimitiveStruct).Value;
            }
            else
            {
                result = item;
            }
            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            int idx = (int)indexes[0];
            var item = Items[idx];
            if (item is IPrimitiveStruct)
            {
                (item as IPrimitiveStruct).Value = value;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
