using MightyStruct.Abstractions;
using MightyStruct.Serializers;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace MightyStruct.Core
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

            return ParseType(xml, /* TODO: static dictionary of primative types */);
        }

        public static UserType ParseType(XElement xmlType, Dictionary<string, IType> scopedTypes)
        {
            var typeName = xmlType.Element("name");
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
                var attrName = attr.Element("name");
                var typeExpr = attr.Element("type");
                var repeatType = attr.Element("repeat");

                IPotential<IType> typePotential = null;
                if (repeatType == null)
                {
                    typePotential = new NamedTypePotential(scopedTypes, new Expression<string>(typeExpr.Value));
                }
                else
                {
                    var lengthExpr = attr.Element("repeat-expr");
                    var untilExpr = attr.Element("repeat-until");

                    if (lengthExpr != null)
                    {
                        var baseType = new NamedTypePotential(scopedTypes, new Expression<string>(typeExpr.Value));
                        var arrayType = new DefiniteArrayType(baseType, new Expression<int>(lengthExpr.Value));
                        typePotential = new TrivialTypePotential(arrayType);
                    }
                    else if (untilExpr != null)
                    {
                        var baseType = new NamedTypePotential(scopedTypes, new Expression<string>(typeExpr.Value));
                        var arrayType = new IndefiniteArrayType(baseType, new Expression<bool>(untilExpr.Value));
                        typePotential = new TrivialTypePotential(arrayType);
                    }
                }

                type.Attributes.Add(attrName.Value, typePotential);
            }

            return type;
        }

        public IStruct CreateInstance(IStruct parent, Stream stream, Context context)
        {
            throw new System.NotImplementedException();
        }
    }
}
