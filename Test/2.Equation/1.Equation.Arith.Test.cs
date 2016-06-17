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
    using System.Linq;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    [TestFixture]
    public partial class EquationTest
    {
        #region Arithmetic Without Fraction

        [Test]
        public void Test_Arith_1()
        {
            //1+2=3
            var lhs = new Term(Expression.Add, new List<object>() { 1, 2 });
            var equation = new Equation(lhs, 3);
            bool result = equation.ContainsVar();
            Assert.False(result);
            Assert.True(equation.ToString().Equals("1+2=3"));

            object obj = equation.Eval();
            Assert.NotNull(obj);
            var satisfied = obj as bool?;
            Assert.NotNull(satisfied);
            Assert.True(satisfied.Value);

            Assert.True(equation.CachedEntities.Count == 1);
            var cachedEq = equation.CachedEntities.ToList()[0] as Equation;
            Assert.NotNull(cachedEq);

            Assert.True(cachedEq.ToString().Equals("3=3"));
            Assert.True(cachedEq.Traces.Count == 1);
        }

        [Test]
        public void Test_Arith_2()
        {
            //1+2=4
            var lhs = new Term(Expression.Add, new List<object>() { 1, 2 });
            var equation = new Equation(lhs, 4);
            bool result = equation.ContainsVar();
            Assert.False(result);
            Assert.True(equation.ToString().Equals("1+2=4"));

            object obj = equation.Eval();
            Assert.NotNull(obj);
            var satisfied = obj as bool?;
            Assert.NotNull(satisfied);
            Assert.False(satisfied.Value);
            Assert.True(equation.CachedEntities.Count == 1);
            var cachedEq = equation.CachedEntities.ToList()[0] as Equation;
            Assert.NotNull(cachedEq);

            Assert.True(cachedEq.ToString().Equals("3=4"));
            Assert.True(cachedEq.Traces.Count == 1);
        }

        [Test]
        public void Test_Arith_3()
        {
            //1+2+3=6
            var lhs = new Term(Expression.Add, new List<object>() { 1, 2, 3 });
            var equation = new Equation(lhs, 6);
            bool result = equation.ContainsVar();
            Assert.False(result);
            Assert.True(equation.ToString().Equals("1+2+3=6"));

            object outputEq;
            bool? evalResult = equation.Eval(out outputEq);
            Assert.NotNull(evalResult);
            Assert.True(evalResult.Value);
            Assert.NotNull(outputEq);
            Assert.True(outputEq.ToString().Equals("6=6"));
            Assert.True(equation.Traces.Count == 1);
        }

        [Test]
        public void Test_Arith_4()
        {
            //1*2*3=7
            var lhs = new Term(Expression.Multiply, new List<object>() { 1, 2, 3 });
            var equation = new Equation(lhs, 7);
            bool result = equation.ContainsVar();
            Assert.False(result);
            //Assert.True(equation.ToString().Equals("2*3=7"));

            object outputEq;
            bool? evalResult = equation.Eval(out outputEq);
            Assert.NotNull(evalResult);
            Assert.False(evalResult.Value);
            Assert.NotNull(outputEq);
            Assert.True(outputEq.ToString().Equals("6=7"));
            Assert.True(equation.Traces.Count == 1);
        }

        #endregion

        #region Arithmetic with Fraction

        //1/1=1   -> 1=1
        //1/2=1   -> 1=2
        //2/3=4   -> 2=12
        //(3+1)/4=5 -> 3+1=20

        [Test]
        public void Test_Arith_Frac_1()
        {
            //1/1=1   -> 1=1
            var term1 = new Term(Expression.Divide, new List<object>() {1, 1});
            var eq = new Equation(term1, 1);
            object obj;
            bool? flag = eq.Eval(out obj);
            Assert.True(flag);
            Assert.False(obj.Equals(eq));
        }

        [Test]
        public void Test_Arith_Frac_2()
        {
            //1/2=1   -> 1=2
            var term1 = new Term(Expression.Divide, new List<object>() { 1, 2});
            var eq = new Equation(term1, 1);
            object obj;
            bool? flag = eq.Eval(out obj);
            //Assert.True(flag);
            Assert.False(obj.Equals(eq));
            var gEq = obj as Equation;
            Assert.NotNull(gEq);
            Assert.True(gEq.Lhs.ToString().Equals("1"));
            Assert.True(gEq.Rhs.ToString().Equals("2"));
        }

        [Test]
        public void Test_Arith_Frac_3()
        {
            //2/3=4   -> 2=12
            var term2 = new Term(Expression.Divide, new List<object>() {2, 3});
            var eq = new Equation(term2, 4);

            object obj;
            bool? flag = eq.Eval(out obj);
            //Assert.True(flag);
            Assert.False(obj.Equals(eq));
            var gEq = obj as Equation;
            Assert.NotNull(gEq);
            Assert.True(gEq.Lhs.ToString().Equals("2"));
            Assert.True(gEq.Rhs.ToString().Equals("12"));
        }

        [Test]
        public void Test_Arith_Frac_4()
        {
            //(3+1)/4=5 -> 3+1=20
            var term1 = new Term(Expression.Add,    new List<object>() {3, 1});
            var term2 = new Term(Expression.Divide, new List<object>() {term1, 4});
            var eq = new Equation(term2, 5);
            object obj;
            bool? flag = eq.Eval(out obj);
            var gEq = obj as Equation;
            Assert.NotNull(gEq);
            Assert.True(gEq.Lhs.ToString().Equals("4"));
            Assert.True(gEq.Rhs.ToString().Equals("20"));
        } 

        #endregion
    }
}
