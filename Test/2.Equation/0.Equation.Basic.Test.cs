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

namespace CSharpLogic
{
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    [TestFixture]
    public partial class EquationTest
    {
        [Test]
        public void Equation_Satisfy()
        {
            //2=2
            var eq = new Equation(2, 2);
            object obj;
            bool result = eq.IsEqGoal(out obj);            
            Assert.False(result);

            //3=4
            eq = new Equation(3,4);
            result = eq.IsEqGoal(out obj);
            Assert.False(result);

            //3=5-2
            var term = new Term(Expression.Add, new List<object>() {5, -2});
            eq = new Equation(3, term);
            result = eq.IsEqGoal(out obj);
            Assert.False(result);

            //x = x
            //TODO
            var variable = new Var('x');
            eq = new Equation(variable, variable);
/*            result = eq.IsEqGoal(out obj);
            Assert.True(result);*/

            //x = 2x-x
            //TODO
            term      = new Term(Expression.Multiply, new List<object>(){2, variable});
            var term0 = new Term(Expression.Multiply, new List<object>() { -1, variable });
            var term1 = new Term(Expression.Add, new List<object>() {term, term0});
            eq = new Equation(variable, term1);
/*            result = eq.IsEqGoal(out obj);
            Assert.True(result);*/
        }

        [Test]
        public void Goal_Gen_13()
        {
            //a = 1, a*b = -1;
            var a = new Var("a");
            var b = new Var("b");
            var eqGoal = new EqGoal(a, 1);

            var lhsTerm = new Term(Expression.Multiply, new List<object>() { a, b });
            var equation = new Equation(lhsTerm, -1);

            bool result = equation.Reify(eqGoal);
            Assert.True(result);
            Assert.True(equation.CachedEntities.Count == 1);
            var cachedEq = equation.CachedEntities.ToList()[0] as Equation;
            object obj;
            result = cachedEq.IsEqGoal(out obj);
            Assert.True(result);
            var gGoal = obj as EqGoal;
            Assert.NotNull(gGoal);
            Assert.True(gGoal.Rhs.Equals(-1));
        }

        [Test]
        public void Goal_Gen_14()
        {
            //14: a = 2, b=a
            var a = new Var("a");
            var b = new Var("b");
            var eqGoal = new EqGoal(a, 2);
            var equation = new Equation(b, a);
            bool result = equation.Reify(eqGoal);
            Assert.True(result);
            Assert.True(equation.CachedEntities.Count == 1);
            var cachedEq = equation.CachedEntities.ToList()[0] as Equation;
            object obj;
            result = cachedEq.IsEqGoal(out obj);
            Assert.True(result);
            var gGoal = obj as EqGoal;
            Assert.NotNull(gGoal);
            Assert.True(gGoal.Lhs.Equals(b));
            Assert.True(gGoal.Rhs.Equals(2));
        }

        [Test]
        public void Goal_Gen_15()
        {
            //14: a = 2, b=a
            var a = new Var("a");
            var b = new Var("b");
            var eqGoal = new EqGoal(a, 2);
            var equation = new EqGoal(b, a);
            bool result = equation.Reify(eqGoal);
            Assert.True(result);
            Assert.True(equation.CachedEntities.Count == 1);
            var cachedEq = equation.CachedEntities.ToList()[0] as Equation;
            object obj;
            result = cachedEq.IsEqGoal(out obj);
            Assert.True(result);
            var gGoal = obj as EqGoal;
            Assert.NotNull(gGoal);
            Assert.True(gGoal.Lhs.Equals(b));
            Assert.True(gGoal.Rhs.Equals(2));
        }

        //TODO
        public void Goal_Gen_1()
        {
            //x = 3
            var x = new Var('a');
            var eq = new Equation(x, 3);
            object obj;
            bool result = eq.IsEqGoal(out obj);
            Assert.True(result);

            //x = y
            var y = new Var('y');
            eq = new Equation(x, y);
            result = eq.IsEqGoal(out obj);
            Assert.True(result);
        }
    }
}
