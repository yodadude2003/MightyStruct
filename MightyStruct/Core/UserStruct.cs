using MightyStruct.Abstractions;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace MightyStruct.Core
{
    public class UserStruct : DynamicObject, IStruct
    {
        private UserType Type { get; }

        private Context Context { get; }

        private Dictionary<string, IStruct> Attributes { get; }

        public SafeStream _io => new SafeStream(Context.Stream);
        public IStruct _parent => Context.Parent;

        public UserStruct(UserType type, Context context)
        {
            Type = type;

            Context = new Context(context, this);
            Context.Variables = new Variables(Context.Self);

            Attributes = new Dictionary<string, IStruct>();
        }

        public async Task ParseAsync()
        {
            foreach (var attr in Type.Attributes)
            {
                var name = attr.Key;
                var type = attr.Value;

                var evaluatedType = await type.Resolve(Context);

                IStruct @struct = await evaluatedType.Resolve(Context);
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
            if (Attributes.ContainsKey(binder.Name))
            {
                var @struct = Attributes[binder.Name];
                if (@struct is IPrimitiveStruct)
                    result = (@struct as IPrimitiveStruct).Value;
                else if (@struct is VoidStruct)
                    result = (@struct as VoidStruct).Stream;
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
