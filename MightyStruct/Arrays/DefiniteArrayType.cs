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

namespace MightyStruct.Arrays
{
    public class DefiniteLoopCondition : IPotential<bool>
    {
        public int Length { get; }

        public DefiniteLoopCondition(int length)
        {
            Length = length;
        }

        public Task<bool> Resolve(Context context)
        {
            var loopState = context.Variables as LoopVariables;
            bool shouldStop = loopState._index >= Length;
            return Task.FromResult(shouldStop);
        }
    }

    public class DefiniteArrayType : ArrayType
    {
        public IPotential<int> Length { get; }

        public DefiniteArrayType(IPotential<IType> baseType, IPotential<int> length) : base(baseType)
        {
            Length = length;
        }

        public override async Task<IStruct> Resolve(Context context)
        {
            IType baseType = await BaseType.Resolve(context);
            int length = await Length.Resolve(context);

            return new ArrayStruct(context, baseType, new DefiniteLoopCondition(length));
        }
    }
}
