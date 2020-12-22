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
using System.Threading.Tasks;

namespace MightyStruct.Basic
{
    public class OffsetType : IType
    {
        public IPotential<IType> BaseType { get; }

        public IPotential<IPrimitiveStruct> PointerValue { get; }
        public IPotential<IStruct> PointerBase { get; }
        public IPotential<long> Offset { get; }

        public OffsetType(IPotential<IType> baseType, IPotential<IPrimitiveStruct> pointerValue, IPotential<IStruct> pointerBase, IPotential<long> offset)
        {
            BaseType = baseType;

            PointerBase = pointerBase;
            PointerValue = pointerValue;

            Offset = offset;
        }

        public async Task<IStruct> Resolve(Context context)
        {
            var type = await BaseType.Resolve(context);

            var @base = await PointerBase.Resolve(context);
            var value = await PointerValue.Resolve(context);

            var offset = await Offset.Resolve(context);

            return await type.Resolve(new Context(context, new Pointer(value, @base?.Context.Segment, offset)));
        }
    }
}
