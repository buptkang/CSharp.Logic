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
    public class AlgebraArithTest
    {
        [Test]
        public void Term_Algebra_Arith_1()
        {
            var x = new Var('x');
            //x-2*5
            var lst = new List<object>() { 2, 5 };
            var term = new Term(Expression.Multiply, lst);
            var lst1 = new List<object>() { x, term };
            var term1 = new Term(Expression.Subtract, lst1);
            object obj = term1.Eval();
            Assert.NotNull(obj);
            var gTerm = obj as Term;
            Assert.NotNull(gTerm);
            var glst = gTerm.Args as List<object>;
            Assert.NotNull(glst);
            Assert.True(glst.Count == 2);
            Assert.True(term1.Traces.Count == 1);
        }

        [Test]
        public void Term_Algebra_Arith_2()
        {
            var x = new Var('x');
            // 2*5 + x
            var lst = new List<object>() { 2, 5 };
            var term = new Term(Expression.Multiply, lst);
            var lst1 = new List<object>() { term, x };
            var term1 = new Term(Expression.Add, lst1);
            object obj = term1.Eval();
            Assert.NotNull(obj);
            var gTerm = obj as Term;
            Assert.NotNull(gTerm);
            var glst = gTerm.Args as List<object>;
            Assert.NotNull(glst);
            Assert.True(glst.Count == 2);
            Assert.True(term1.Traces.Count == 2);
        }

        [Test]
        public void Term_Algebra_Arith_3()
        {
            // 3 + x + 3
            var x = new Var('x');
            var lst = new List<object>() { 3, x, 3 };
            var term = new Term(Expression.Add, lst);
            object obj = term.Eval();
            Assert.NotNull(obj);
            var gTerm = obj as Term;
            Assert.NotNull(gTerm);
            var glst = gTerm.Args as List<object>;
            Assert.NotNull(glst);
            Assert.True(glst.Count == 2);
            Assert.True(term.Traces.Count == 2);
        }
    }
}
