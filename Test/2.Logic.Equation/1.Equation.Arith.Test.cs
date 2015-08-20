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
        #region Arithmetic Only

        [Test]
        public void Test_Arith_1()
        {
            //1+2=3
            var lhs = new Term(Expression.Add, new List<object>() { 1, 2 });
            var equation = new Equation(lhs, 3);
            bool result = equation.ContainsVar();
            Assert.False(result);
            Assert.True(equation.ToString().Equals("(1+2)=3"));

            Equation outputEq;
            bool? evalResult = equation.Eval(out outputEq);
            Assert.NotNull(evalResult);
            Assert.True(evalResult.Value);
            Assert.NotNull(outputEq);
            Assert.True(outputEq.ToString().Equals("3=3"));
            Assert.True(equation.Traces.Count == 1);
        }

        [Test]
        public void Test_Arith_2()
        {
            //1+2=4
            var lhs = new Term(Expression.Add, new List<object>() { 1, 2 });
            var equation = new Equation(lhs, 4);
            bool result = equation.ContainsVar();
            Assert.False(result);
            Assert.True(equation.ToString().Equals("(1+2)=4"));

            Equation outputEq;
            bool? evalResult = equation.Eval(out outputEq);
            Assert.NotNull(evalResult);
            Assert.False(evalResult.Value);
            Assert.NotNull(outputEq);
            Assert.True(outputEq.ToString().Equals("3=4"));
        }

        [Test]
        public void Test_Arith_3()
        {
            //1+2+3=6
            var lhs = new Term(Expression.Add, new List<object>() { 1, 2, 3 });
            var equation = new Equation(lhs, 6);
            bool result = equation.ContainsVar();
            Assert.False(result);
            Assert.True(equation.ToString().Equals("(1+2+3)=6"));

            Equation outputEq;
            bool? evalResult = equation.Eval(out outputEq);
            Assert.NotNull(evalResult);
            Assert.True(evalResult.Value);
            Assert.NotNull(outputEq);
            Assert.True(outputEq.ToString().Equals("6=6"));
            Assert.True(equation.Traces.Count == 2);
        }

        [Test]
        public void Test_Arith_4()
        {
            //1*2*3=7
            var lhs = new Term(Expression.Multiply, new List<object>() { 1, 2, 3 });
            var equation = new Equation(lhs, 7);
            bool result = equation.ContainsVar();
            Assert.False(result);
            Assert.True(equation.ToString().Equals("(2*3)=7"));

            Equation outputEq;
            bool? evalResult = equation.Eval(out outputEq);
            Assert.NotNull(evalResult);
            Assert.False(evalResult.Value);
            Assert.NotNull(outputEq);
            Assert.True(outputEq.ToString().Equals("6=7"));
            Assert.True(equation.Traces.Count == 2);
        }

        #endregion

    }
}
