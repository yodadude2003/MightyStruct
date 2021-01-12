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

using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

using System.Threading.Tasks;

namespace MightyStruct.Runtime
{
    public class Expression<T> : IPotential<T>
    {
        public string Expr { get; }
        public ScriptRunner<T> ScriptRunner { get; }

        public Expression(string expr)
        {
            Expr = expr;
            var options = ScriptOptions.Default
                .WithReferences("Microsoft.CSharp");
            ScriptRunner = CSharpScript.Create<T>(Expr, options, typeof(Variables)).CreateDelegate();
        }

        public async Task<T> Resolve(Context context)
        {
            var result = await ScriptRunner.Invoke(context.Variables);
            return result;
        }
    }
}
