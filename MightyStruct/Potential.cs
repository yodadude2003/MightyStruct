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

using System.Threading.Tasks;

namespace MightyStruct
{
    using Runtime;

    public interface IPotential<T>
    {
        Task<T> Resolve(Context context);
    }

    public class TrivialPotential<T> : IPotential<T>
    {
        public T Inner { get; }

        public TrivialPotential(T inner)
        {
            Inner = inner;
        }

        public Task<T> Resolve(Context _)
        {
            return Task.FromResult(Inner);
        }
    }
}
