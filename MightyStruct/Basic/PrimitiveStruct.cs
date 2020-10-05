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
    public interface IPrimitiveStruct : IStruct
    {
        object Value { get; set; }
    }

    public class PrimitiveStruct<T> : IPrimitiveStruct
    {
        private ISerializer<T> Serializer { get; }

        private Context Context { get; }

        public T Value { get; set; }

        object IPrimitiveStruct.Value { get => Value; set => Value = (T)value; }

        public PrimitiveStruct(Context context, ISerializer<T> serializer)
        {
            Context = new Context(context, this);
            Serializer = serializer;
        }

        public async Task ParseAsync()
        {
            Value = await Serializer.ReadFromStreamAsync(Context.Stream);
        }

        public Task UpdateAsync()
        {
            Context.Stream.Seek(0, SeekOrigin.Begin);
            return Serializer.WriteToStreamAsync(Context.Stream, Value);
        }
    }
}
