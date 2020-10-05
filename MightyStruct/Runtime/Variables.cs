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

using System;

namespace MightyStruct.Runtime
{
    public class Variables
    {
        public dynamic _self { get; }

        public virtual int _index => throw new Exception("_index can only be used in a looping context.");
        public virtual dynamic _ => throw new Exception("_ can only be used in a looping context.");

        public Variables(IStruct self)
        {
            _self = self;
        }
    }
}
