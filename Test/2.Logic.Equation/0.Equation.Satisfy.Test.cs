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
        public void Goal_Gen_1()
        {
            //2=2
            var eq = new Equation(2, 2);
            EqGoal eqGoal;
            bool result = eq.IsEqGoal(out eqGoal);
            Assert.False(result);

            //3=4
            eq = new Equation(3,4);
            result = eq.IsEqGoal(out eqGoal);
            Assert.False(result);

            //3=5-2
            var term = new Term(Expression.Add, new List<object>() {5, -2});
            eq = new Equation(3, term);
            result = eq.IsEqGoal(out eqGoal);
            Assert.False(result);

            //x = x
            var variable = new Var('x');
            eq = new Equation(variable, variable);
            result = eq.IsEqGoal(out eqGoal);
            Assert.False(result);

            //x = 2x-x
            term      = new Term(Expression.Multiply, new List<object>(){2, variable});
            var term0 = new Term(Expression.Multiply, new List<object>() { -1, variable });
            var term1 = new Term(Expression.Add, new List<object>() {term, term0});
            eq = new Equation(variable, term1);
            result = eq.IsEqGoal(out eqGoal);
            Assert.False(result);
        }
    }
}
