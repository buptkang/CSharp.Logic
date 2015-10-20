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
        public void Test_LineGen_1()
        {
			//2x+3y-1=0
            var x = new Var("x");
            var term1 = new Term(Expression.Multiply, new List<object>() {2, x});
            var y = new Var("y");
            var term2 = new Term(Expression.Multiply, new List<object>() {3, y});
            var term = new Term(Expression.Add, new List<object>() {term1, term2, -1});
            var equation = new Equation(term, 0);

            object obj;
            bool? result = equation.Eval(out obj, false);
			Assert.Null(result);
            var outputEq = obj as Equation;
			Assert.NotNull(outputEq);
			Assert.True(outputEq.Equals(equation));

            result = equation.Eval(out obj, true);
			Assert.Null(result);
            outputEq = obj as Equation;
			Assert.NotNull(outputEq);
            Assert.False(outputEq.Equals(equation));
			Assert.True(outputEq.ToString().Equals("((2*x)+(3*y))=1"));
        }

        [Test]
        public void Test_LineGen_2()
        {
           /* //4y=x
            var x = new Var("x");
            var y = new Var("y");
            var term2 = new Term(Expression.Multiply, new List<object>() { 4, y });
            var equation = new Equation(term2, x);

            object obj;
            bool?  result = equation.Eval(out obj, true);
            Assert.Null(result);
            var outputEq = obj as Equation;
            Assert.NotNull(outputEq);*/
        }
    }
}
