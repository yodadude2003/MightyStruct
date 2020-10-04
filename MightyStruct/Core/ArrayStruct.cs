using MightyStruct.Abstractions;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace MightyStruct.Core
{
    public class ArrayStruct : DynamicObject, IStruct, IEnumerable<IStruct>
    {
        private Context Context { get; }

        private IType BaseType { get; }
        private IPotential<bool> LoopCondition { get; }

        private List<IStruct> Items { get; }

        public int Length => Items.Count;
        public int size => Items.Count;

        public ArrayStruct(Context context, IType baseType, IPotential<bool> loopCondition)
        {
            Context = new Context(context, this);

            BaseType = baseType;
            LoopCondition = loopCondition;

            Items = new List<IStruct>();
        }

        public async Task ParseAsync()
        {
            int index = 0;

            var context = new Context(Context);
            context.Variables = new LoopVariables(Context.Parent, 0, null);
            do
            {
                IStruct @struct = await BaseType.Resolve(context);
                await @struct.ParseAsync();
                Items.Add(@struct);

                var vars = new LoopVariables(Context.Parent, ++index, @struct);
                context.Variables = vars;

            } while (!(await LoopCondition.Resolve(context)));

            (Context.Stream as SubStream)?.Lock();
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

        public IEnumerator<IStruct> GetEnumerator()
        {
            return ((IEnumerable<IStruct>)Items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<IStruct>)Items).GetEnumerator();
        }
    }
}
