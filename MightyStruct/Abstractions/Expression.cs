using DynamicExpresso;

namespace MightyStruct.Abstractions
{
    public class Expression<T> : IPotential<T>
    {
        public string Expr { get; }

        public Expression(string expr)
        {
            Expr = expr;
        }

        public T Resolve(Context context)
        {
            var interpreter = new Interpreter();

            interpreter.SetVariable("_self", context.Self);

            foreach (var variable in context.Variables)
            {
                interpreter.SetVariable(variable.Key, variable.Value);
            }

            return interpreter.Eval<T>(Expr);
        }
    }
}
