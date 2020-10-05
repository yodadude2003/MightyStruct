using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

using System.Threading.Tasks;

namespace MightyStruct.Runtime
{
    public class Expression<T> : IPotential<T>
    {
        public Script<T> Script { get; }

        public Expression(string expr)
        {
            var options = ScriptOptions.Default
                .WithReferences("Microsoft.CSharp");
            Script = CSharpScript.Create<T>(expr, options, typeof(Variables));
        }

        public async Task<T> Resolve(Context context)
        {
            var result = await Script.RunAsync(context.Variables);
            return result.ReturnValue;
        }
    }
}
