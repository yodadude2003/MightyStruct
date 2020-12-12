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

using System.IO;
using System.Threading.Tasks;

namespace MightyStruct.Basic
{
    public class VoidStruct : IStruct
    {
        public Context Context { get; }
        public long Length { get; }

        public Stream Stream => Context.Stream;

        public VoidStruct(Context context, long length)
        {
            Context = new Context(context, this);

            Length = length;
        }

        public Task ParseAsync()
        {
            Context.Stream.SetLength(Length);
            Context.Stream.Position = Length;

            (Context.Stream as SubStream)?.Lock();

            return Task.CompletedTask;
        }

        public Task UpdateAsync()
        {
            return Task.CompletedTask;
        }
    }
}
