using MightyStruct.Abstractions;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;

namespace MightyStruct.Core
{
    public class UserStruct : DynamicObject, IStruct
    {
        public IType Type { get; }

        public Context Context { get; }

        public Dictionary<string, IStruct> Attributes { get; }

        public UserStruct(IType type, Context context)
        {
            Type = type;

            Context = new Context(context, this);

            Attributes = new Dictionary<string, IStruct>();
        }

        public async Task ParseAsync()
        {
            foreach (var attr in (Type as UserType).Attributes)
            {
                var name = attr.Key;
                var type = attr.Value;

                var evaluatedType = type.Resolve(Context);

                IStruct @struct = evaluatedType.CreateInstance(Context);
                await @struct.ParseAsync();

                Attributes.Add(name, @struct);
            }
        }

        public async Task UpdateAsync()
        {
            foreach (var attr in Attributes)
            {
                var @struct = attr.Value;
                await @struct.UpdateAsync();
            }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (binder.Name == "_io")
            {
                result = Context.Stream;
                return true;
            }
            else if (binder.Name == "_parent")
            {
                result = Context.Parent;
                return true;
            }
            else if (Attributes.ContainsKey(binder.Name))
            {
                var @struct = Attributes[binder.Name];
                if (@struct is IPrimitiveStruct)
                    result = (@struct as IPrimitiveStruct).Value;
                else
                    result = @struct;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (Attributes.ContainsKey(binder.Name))
            {
                var @struct = Attributes[binder.Name];
                if (@struct is IPrimitiveStruct)
                {
                    (@struct as IPrimitiveStruct).Value = value;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
