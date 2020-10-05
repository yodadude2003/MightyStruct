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

using MightyStruct.Runtime;

using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace MightyStruct.Basic
{
    public class UserStruct : DynamicObject, IStruct
    {
        private UserType Type { get; }

        private Context Context { get; }

        private Dictionary<string, IStruct> Attributes { get; }

        public SafeStream _io => new SafeStream(Context.Stream);
        public IStruct _parent => Context.Parent;

        public UserStruct(UserType type, Context context)
        {
            Type = type;

            Context = new Context(context, this);
            Context.Variables = new Variables(Context.Self);

            Attributes = new Dictionary<string, IStruct>();
        }

        public async Task ParseAsync()
        {
            foreach (var attr in Type.Attributes)
            {
                var name = attr.Key;
                var type = attr.Value;

                var evaluatedType = await type.Resolve(Context);

                IStruct @struct = await evaluatedType.Resolve(Context);
                await @struct.ParseAsync();

                Attributes.Add(name, @struct);
            }

            (Context.Stream as SubStream).Lock();
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
                else if (@struct is VoidStruct)
                    result = (@struct as VoidStruct).Stream;
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
