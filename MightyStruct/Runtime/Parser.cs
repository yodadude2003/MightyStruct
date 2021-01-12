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
            // Unsigned integers (endianness independent)
            { "u1", new PrimitiveType<byte>(new UInt8Serializer()) },

            // Unsigned integers (little endian)
            { "u2le", new PrimitiveType<ushort>(new UInt16Serializer(Endianness.LittleEndian)) },
            { "u4le", new PrimitiveType<uint>(new UInt32Serializer(Endianness.LittleEndian)) },
            { "u8le", new PrimitiveType<ulong>(new UInt64Serializer(Endianness.LittleEndian)) },

            // Unsigned integers (big endian)
            { "u2be", new PrimitiveType<ushort>(new UInt16Serializer(Endianness.BigEndian)) },
            { "u4be", new PrimitiveType<uint>(new UInt32Serializer(Endianness.BigEndian)) },
            { "u8be", new PrimitiveType<ulong>(new UInt64Serializer(Endianness.BigEndian)) },

            // Signed integers (both endians | TODO: Implement s2, s8)
            { "s4le", new PrimitiveType<int>(new SInt32Serializer(Endianness.LittleEndian)) },
            { "s4be", new PrimitiveType<int>(new SInt32Serializer(Endianness.BigEndian)) },

            // Floating point types (both endians | TODO: Implement f2, f8)
            { "f4le", new PrimitiveType<float>(new Float32Serializer(Endianness.LittleEndian)) },
            { "f4be", new PrimitiveType<float>(new Float32Serializer(Endianness.BigEndian)) },

            // String types (null terminating)
            { "str_ascii", new PrimitiveType<string>(new StringSerializer(Encoding.ASCII)) },
            { "str_utf8", new PrimitiveType<string>(new StringSerializer(Encoding.UTF8)) },
            { "str_uni", new PrimitiveType<string>(new StringSerializer(Encoding.Unicode)) },
            { "str_uni_be", new PrimitiveType<string>(new StringSerializer(Encoding.BigEndianUnicode)) },
        };

        public static UserType ParseFromStream(Stream stream)
        {
            var xml = XElement.Load(stream);

            return ParseType(xml, new Dictionary<string, IType>(Types));
        }

        public static UserType ParseType(XElement xmlType, Dictionary<string, IType> scopedTypes)
        {
            var typeName = xmlType.Attribute("name");
            var type = new UserType(typeName.Value);

            var subTypes = xmlType.Elements("type");
            var attributes = xmlType.Elements("attr");

            scopedTypes.Add(type.Name, type);
            foreach (var subType in subTypes)
            {
                var parsed = ParseType(subType, new Dictionary<string, IType>(scopedTypes));
                type.SubTypes.Add(parsed.Name, parsed);
                scopedTypes.Add(parsed.Name, parsed);
            }

            foreach (var attr in attributes)
            {
                var nameAttr = attr.Attribute("name");

                var debugAttr = attr.Attribute("debug");

                var typeAttr = attr.Attribute("type");
                var typeExpr = attr.Element("type");

                var baseExpr = attr.Element("base");
                var pointerExpr = attr.Element("pointer");
                var offsetExpr = attr.Element("offset");

                var sizeExpr = attr.Element("size");

                var conditionExpr = attr.Element("if");

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

                if (debugAttr != null)
                {
                    var baseType = typePotential;
                    var breakpointType = new BreakpointType(baseType);
                    typePotential = new TrivialPotential<IType>(breakpointType);
                }

                if (pointerExpr != null)
                {
                    var baseType = typePotential;
                    var offsetType = new OffsetType(baseType,
                        new Expression<IPrimitiveStruct>(pointerExpr.Value),
                        baseExpr == null ? new TrivialPotential<IStruct>(null) : new Expression<IStruct>(baseExpr.Value),
                        offsetExpr == null ? new TrivialPotential<long>(0) : new Expression<long>(offsetExpr.Value)
                        );

                    typePotential = new TrivialPotential<IType>(offsetType);
                }

                if (conditionExpr != null)
                {
                    var baseType = typePotential;
                    var conditionalType = new ConditionalType(baseType, new Expression<bool>(conditionExpr.Value));

                    typePotential = new TrivialPotential<IType>(conditionalType);
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

                type.Attributes.Add(nameAttr.Value, typePotential);
            }

            return type;
        }
    }
}
