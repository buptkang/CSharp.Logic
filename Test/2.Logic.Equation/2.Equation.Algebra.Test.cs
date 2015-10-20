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

using System.Linq;
using System.Runtime;

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

            equation.Eval();
            Assert.True(equation.CachedEntities.Count == 1);
            var outputEq = equation.CachedEntities.ToList()[0] as Equation;
            Assert.NotNull(outputEq);
            Assert.True(outputEq.ToString().Equals("x=3"));
            Assert.True(outputEq.Traces.Count == 1);
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

            equation.Eval();

            Assert.True(equation.CachedEntities.Count == 1);
            var outputEq = equation.CachedEntities.ToList()[0] as Equation;
            Assert.NotNull(outputEq);
            Assert.True(outputEq.ToString().Equals("x=1"));
            Assert.True(outputEq.Traces.Count == 1);
        }

        [Test]
        public void Test_Algebra_3()
        {
            //a^2 = 25
            object pt1XCoord = new Var("a");
            object pt1YCoord = 6.0;
            object pt2XCoord = 3.0;
            object pt2YCoord = 3.0;

            var term1 = new Term(Expression.Subtract, new List<object>() { pt1XCoord, pt2XCoord });
            var term11 = new Term(Expression.Power, new List<object>() { pt1XCoord, 2.0 });
            var term2 = new Term(Expression.Subtract, new List<object>() { pt1YCoord, pt2YCoord });
            var term22 = new Term(Expression.Power, new List<object>() { term2, 2.0 });
            var rhs = new Term(Expression.Add, new List<object>() { term11, term22 });
            var eq = new Equation(term11, 25);

            eq.Eval();
            Assert.True(eq.CachedEntities.Count == 2);

            var outputEq1 = eq.CachedEntities.ToList()[0] as Equation;
            Assert.NotNull(outputEq1);
            Assert.True(outputEq1.ToString().Equals("a=5"));
            Assert.True(outputEq1.Traces.Count == 1);

            var outputEq2 = eq.CachedEntities.ToList()[1] as Equation;
            Assert.NotNull(outputEq2);
            Assert.True(outputEq2.ToString().Equals("a=-5"));
            Assert.True(outputEq2.Traces.Count == 1);
        }

        [Test]
        public void Test_Algebra_4()
        {
            //a^2+ 4^2 = 25
            object pt1XCoord = new Var("a");
            object pt1YCoord = 6.0;
            object pt2XCoord = 3.0;
            object pt2YCoord = 3.0;

            //var term1 = new Term(Expression.Subtract, new List<object>() { pt1XCoord, pt2XCoord });
            var term11 = new Term(Expression.Power, new List<object>() { pt1XCoord, 2.0 });
            var term2 = new Term(Expression.Subtract, new List<object>() { pt1YCoord, pt2YCoord });
            var term22 = new Term(Expression.Power, new List<object>() { 4, 2.0 });
            var rhs = new Term(Expression.Add, new List<object>() { term11, term22 });
            var eq = new Equation(rhs, 25);

            eq.Eval();
            Assert.True(eq.CachedEntities.Count == 2);

            var outputEq1 = eq.CachedEntities.ToList()[0] as Equation;
            Assert.NotNull(outputEq1);
            Assert.True(outputEq1.ToString().Equals("a=3"));
            Assert.True(outputEq1.Traces.Count == 1);

            var outputEq2 = eq.CachedEntities.ToList()[1] as Equation;
            Assert.NotNull(outputEq2);
            Assert.True(outputEq2.ToString().Equals("a=-3"));
            Assert.True(outputEq2.Traces.Count == 1);
        }

        [Test]
        public void Test_Algebra_5()
        {
            //25 = (a-3.0)^2+4^2
            object pt1XCoord = new Var("a");
            object pt1YCoord = 6.0;
            object pt2XCoord = -3.0;
            object pt2YCoord = 3.0;

            var term1 = new Term(Expression.Add, new List<object>() { pt1XCoord, pt2XCoord });
            var term11 = new Term(Expression.Power, new List<object>() { term1, 2.0 });
            var term2 = new Term(Expression.Add, new List<object>() { pt1YCoord, pt2YCoord });
            var term22 = new Term(Expression.Power, new List<object>() { 4, 2.0 });
            var rhs = new Term(Expression.Add, new List<object>() { term11, term22 });
            var eq = new Equation(25,rhs);

            eq.Eval();
            Assert.True(eq.CachedEntities.Count == 2);

            var outputEq1 = eq.CachedEntities.ToList()[0] as Equation;
            Assert.NotNull(outputEq1);
            Assert.True(outputEq1.ToString().Equals("a=6"));
            Assert.True(outputEq1.Traces.Count == 1);

            var outputEq2 = eq.CachedEntities.ToList()[1] as Equation;
            Assert.NotNull(outputEq2);
            Assert.True(outputEq2.ToString().Equals("a=0"));
            Assert.True(outputEq2.Traces.Count == 1);
        }

        [Test]
        public void Test_Algebra_6()
        {
            //a^2+b^2=25
            object pt1XCoord = new Var("a");
            object pt2XCoord = new Var('b');

            //var term1 = new Term(Expression.Add, new List<object>() { pt1XCoord, pt2XCoord });
            var term11 = new Term(Expression.Power, new List<object>() { pt1XCoord, 2.0 });
            //var term2 = new Term(Expression.Add, new List<object>() { pt1YCoord, pt2YCoord });
            var term22 = new Term(Expression.Power, new List<object>() { pt2XCoord, 2.0 });
            var rhs = new Term(Expression.Add, new List<object>() { term11, term22 });
            var eq = new Equation(rhs, 25);

            eq.Eval();
            Assert.True(eq.CachedEntities.Count == 1);
            var outputEq = eq.CachedEntities.ToList()[0];
            Assert.True(outputEq.Equals(eq));

        }
    }
}
