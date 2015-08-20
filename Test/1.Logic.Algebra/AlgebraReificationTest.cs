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
    public class AlgebraReificationTest
    {
        [Test]
        public void Term_Algebra_Reify_1()
        {
            /*
             *  //y+y=> 
             *  //y = 2
             */
            var y = new Var('y');
            var term = new Term(Expression.Add, new List<object>() { y, y });
            //Assert.True(term.ToString().Equals("(y+y)"));
            var eqGoal = new EqGoal(y, 2);
            var term1 = term.Reify(eqGoal) as Term;
            Assert.NotNull(term1);
            object obj = term1.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.Equals(4));
            Assert.True(term1.Traces.Count == 1);
        }
    }
}
