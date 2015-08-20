/*******************************************************************************
 * Copyright (c) 2015 Bo Kang
 *   
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *  
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *******************************************************************************/

namespace CSharpLogic
{
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    [TestFixture]
    public partial class EquationTest
    {
        [Test]
        public void Test_Algebra_1()
        {
            //1+2=x
            var x = new Var('x');
            var lhs = new Term(Expression.Add, new List<object>() { 1, 2 });
            var equation = new Equation(lhs, x);
            bool result = equation.ContainsVar();
            Assert.True(result);
            Assert.True(equation.ToString().Equals("(1+2)=x"));

            Equation outputEq;
            bool? evalResult = equation.Eval(out outputEq);
            Assert.Null(evalResult);
            Assert.NotNull(outputEq);
            Assert.True(outputEq.ToString().Equals("(x-3)=0"));
            Assert.True(equation.Traces.Count == 6);
        }

        [Test]
        public void Test_Algebra_2()
        {
            //x+2=3
            var x = new Var('x');
            var lhs = new Term(Expression.Add, new List<object>() { x, 2 });
            var equation = new Equation(lhs, 3);
            bool result = equation.ContainsVar();
            Assert.True(result);
            Assert.True(equation.ToString().Equals("(x+2)=3"));

            Equation outputEq;
            bool? evalResult = equation.Eval(out outputEq);
            Assert.Null(evalResult);
            Assert.NotNull(outputEq);
            Assert.True(outputEq.ToString().Equals("(x-1)=0"));
            Assert.True(equation.Traces.Count == 5);
        }
    }
}
