﻿/*
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
using System.Threading.Tasks;

namespace MightyStruct.Basic
{
    public class UserType : IType
    {
        public string Name { get; }

        public Dictionary<string, IPotential<IType>> Attributes { get; }

        public UserType(string name)
        {
            Name = name;

            Attributes = new Dictionary<string, IPotential<IType>>();
        }

        public Task<IStruct> Resolve(Context context)
        {
            return Task.FromResult<IStruct>(new UserStruct(this, context));
        }
    }
}
