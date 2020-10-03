using MightyStruct.Abstractions;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace MightyStruct.Core
{
    public class ArrayStruct : DynamicObject, IStruct
    {
        public IType Type { get; }

        public Context Context { get; }

        public List<IStruct> Items { get; }

        public ArrayStruct(IType type, Context context)
        {
            Type = type;

            Context = new Context(context, this);

            Items = new List<IStruct>();
        }

        public async Task ParseAsync()
        {
            if (Type is DefiniteArrayType)
            {
                var type = Type as DefiniteArrayType;

                var baseType = type.BaseType.Resolve(Context);
                int length = type.Length.Resolve(Context);

                for (int i = 0; i < length; i++)
                {
                    IStruct @struct = baseType.CreateInstance(Context);
                    await @struct.ParseAsync();
                    Items.Add(@struct);
                }
            }
            else if(Type is IndefiniteArrayType)
            {
                var type = Type as IndefiniteArrayType;

                int index = 0;

                IStruct @struct = type.BaseType.Resolve(Context).CreateInstance(Context);
                await @struct.ParseAsync();

                var context = new Context(Context);

                context.Variables.Add("_index", index);
                context.Variables.Add("_", @struct);


                var baseType = type.BaseType.Resolve(Context);
                while (type.Condition.Resolve(context))
                {
                    @struct = baseType.CreateInstance(Context);
                    await @struct.ParseAsync();

                    index++;

                    context.Variables["_index"] = index;
                    context.Variables["_"] = @struct;
                }
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
