/*
 *  Copyright 2020 Chosen Few Software
 *  This file is part of MightyStruct.
 *
 *  MightyStruct is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  MightyStruct is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with MightyStruct.  If not, see <https://www.gnu.org/licenses/>.
 */

using MightyStruct.Basic;
using MightyStruct.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;

namespace MightyStruct.Arrays
{
    public class ArrayStruct : DynamicObject, IStruct, IEnumerable<IStruct>
    {
        public Context Context { get; }

        private IPotential<IType> BaseType { get; }
        private IPotential<bool> LoopCondition { get; }

        private List<IStruct> Items { get; }

        public int size => Items.Count;

        public ArrayStruct(Context context, IPotential<IType> baseType, IPotential<bool> loopCondition)
        {
            Context = new Context(context, true);

            BaseType = baseType;
            LoopCondition = loopCondition;

            Items = new List<IStruct>();
        }

        public async Task ParseAsync()
        {
            Items.Clear();
            Context.Stream.Seek(0, SeekOrigin.Begin);
            (Context.Stream as SubStream).Unlock();

            int index = 0;

            var context = new Context(Context);
            context.Variables = new LoopVariables(context.Self, 0, null);
            do
            {
                IType evaluatedType = await BaseType.Resolve(context);
                IStruct @struct = await evaluatedType.Resolve(context);
                if (@struct != null)
                    await @struct.ParseAsync();
                Items.Add(@struct);

                var vars = new LoopVariables(context.Self, ++index, @struct);
                context.Variables = vars;

            } while (!(await LoopCondition.Resolve(context)));

            (Context.Stream as SubStream)?.Lock();
        }

        public async Task UpdateAsync()
        {
            foreach (var item in Items)
            {
                if (item != null)
                {
                    await item.UpdateAsync();
                }
            }
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            int idx = (int)indexes[0];
            var item = Items[idx];
            result = item;

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
