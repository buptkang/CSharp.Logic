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
        public void Test_Equation_1()
        {
            // d^2 = (1.0-4.0)^2+(2.0-6.0)^2
            object pt1XCoord = 1.0;
            object pt1YCoord = 2.0;
            object pt2XCoord = 4.0;
            object pt2YCoord = 6.0;

            var variable = new Var('d');
            var lhs = new Term(Expression.Power, new List<object>() { variable, 2.0});

            var term1 = new Term(Expression.Subtract, new List<object>() { pt1XCoord, pt2XCoord });
            var term11 = new Term(Expression.Power, new List<object>() { term1, 2.0 });

            var term2 = new Term(Expression.Subtract, new List<object>() { pt1YCoord, pt2YCoord });
            var term22 = new Term(Expression.Power, new List<object>() { term2, 2.0});

            var rhs = new Term(Expression.Add, new List<object>() { term11, term22 }); 
            //var obj = rhs.Eval();

            //Assert.True(obj.Equals(25));
            var eq = new Equation(lhs, rhs);
            EqGoal goal;
            bool result = eq.IsEqGoal(out goal);
            Assert.True(result);
            Assert.NotNull(goal);
            Assert.True(goal.Rhs.Equals(5));
        }

        [Test]
        public void Test_Equation_2()
        {
            // 9 = (a-4.0)^2+(2.0-6.0)^2

        }

        [Test]
        public void Goal_Gen_2()
        {
            //x = 3
            var x = new Var('a');
            var eq = new Equation(x, 3);
            EqGoal eqGoal;
            bool result = eq.IsEqGoal(out eqGoal);
            Assert.True(result);

            //x = y
            var y = new Var('y');
            eq = new Equation(x, y);
            result = eq.IsEqGoal(out eqGoal);
            Assert.True(result);
        }

        [Test]
        public void Goal_Gen_3()
        {
            //x = 2+3
            var x = new Var('x');
            var term = new Term(Expression.Add, new List<object>() { 2, 3 });
            var eq = new Equation(x, term);
            EqGoal eqGoal;
            bool result = eq.IsEqGoal(out eqGoal);
            Assert.True(eqGoal.Traces.Count == 1);
            Assert.True(eqGoal.Lhs.Equals(x));
            Assert.True(eqGoal.Rhs.Equals(5));
            Assert.True(result);
        }

        [Test]
        public void Goal_Gen_4()
        {
            // 3 = x
            // 1+2=x
            // 1+x=3
            // 3*2=x
            // 2*x=6
        }

        [Test]
        public void Goal_Gen_5()
        {
            //x + 1 = 2
            //x + 1 - 3 = 2
            //x + 1 - 5　＝　2+1
            //x = 2+x+1            
        }

        public void Goal_Gen_6()
        {
            // a - b = 2

        }
    }
}
