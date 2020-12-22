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
using System.Threading.Tasks;

namespace MightyStruct.Runtime
{
    public class Pointer
    {
        public IPrimitiveStruct Inner { get; }
        public Segment Base { get; }
        public long Offset { get; }

        public Pointer(IPrimitiveStruct inner, Segment @base, long offset)
        {
            Inner = inner;
            Base = @base;

            Offset = offset;
        }

        public Task AddAsync(short amount)
        {
            Inner.Value += amount;
            return Inner.UpdateAsync();
        }
    }
}
