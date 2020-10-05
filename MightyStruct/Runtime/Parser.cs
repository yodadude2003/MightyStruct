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

using MightyStruct.Arrays;
using MightyStruct.Basic;
using MightyStruct.Serializers;

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace MightyStruct.Runtime
{
    public class Parser
    {
        public static Dictionary<string, IType> Types
        { get; } = new Dictionary<string, IType>()
        {
            { "u1", new PrimitiveType<byte>(new UInt8Serializer()) },
            { "u2", new PrimitiveType<ushort>(new UInt16Serializer()) },
            { "u4", new PrimitiveType<uint>(new UInt32Serializer()) },
            { "u8", new PrimitiveType<ulong>(new UInt64Serializer()) },

            { "str", new PrimitiveType<string>(new StringSerializer(Encoding.ASCII)) },
        };

        public static UserType ParseFromStream(Stream stream)
        {
            var xml = XElement.Load(stream);

            return ParseType(xml, Types);
        }

        public static UserType ParseType(XElement xmlType, Dictionary<string, IType> scopedTypes)
        {
            var typeName = xmlType.Attribute("name");
            var type = new UserType(typeName.Value);

            var subTypes = xmlType.Elements("type");
            var attributes = xmlType.Elements("attr");

            foreach (var subType in subTypes)
            {
                var parsed = ParseType(subType, scopedTypes);
                scopedTypes.Add(parsed.Name, parsed);
            }

            foreach (var attr in attributes)
            {
                var attrName = attr.Attribute("name");

                var typeAttr = attr.Attribute("type");
                var typeExpr = attr.Element("type");

                var offsetExpr = attr.Element("offset");
                var sizeExpr = attr.Element("size");

                var repeatType = attr.Element("repeat");

                IPotential<IType> typePotential;

                if (typeAttr != null || typeExpr != null)
                {
                    IPotential<string> namePotential = typeAttr != null ?
                        (IPotential<string>)new TrivialPotential<string>(typeAttr.Value) :
                        (IPotential<string>)new Expression<string>(typeExpr.Value);

                    typePotential = new NamedTypePotential(scopedTypes, namePotential);
                }
                else
                {
                    typePotential = new TrivialPotential<IType>(new VoidType(new Expression<long>(sizeExpr.Value)));
                }

                if (offsetExpr != null)
                {
                    var baseType = typePotential;
                    var offsetType = new OffsetType(baseType, new Expression<long>(offsetExpr.Value));

                    typePotential = new TrivialPotential<IType>(offsetType);
                }

                if (repeatType != null)
                {
                    var lengthExpr = attr.Element("repeat-expr");
                    var untilExpr = attr.Element("repeat-until");

                    var baseType = typePotential;

                    if (repeatType.Value == "expr")
                    {
                        var arrayType = new DefiniteArrayType(baseType, new Expression<int>(lengthExpr.Value));
                        typePotential = new TrivialPotential<IType>(arrayType);
                    }
                    else if (repeatType.Value == "until")
                    {
                        var arrayType = new IndefiniteArrayType(baseType, new Expression<bool>(untilExpr.Value));
                        typePotential = new TrivialPotential<IType>(arrayType);
                    }
                }

                type.Attributes.Add(attrName.Value, typePotential);
            }

            return type;
        }
    }
}
