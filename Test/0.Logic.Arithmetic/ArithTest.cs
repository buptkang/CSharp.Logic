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
    public class TestArith
    {
        [Test]
        public void Term_Arith_1()
        {
            //Addition
            var lst = new List<object>() {1, 2, 3};
            var term = new Term(Expression.Add, lst);
            object obj = term.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.Equals(6));
            Assert.True(term.Traces.Count == 2);
        }

        [Test]
        public void Term_Arith_2()
        {
            //Substraction
            var lst = new List<object>() { 1, -2, -3 };
            var term = new Term(Expression.Add, lst);
            object obj = term.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.Equals(-4));
            Assert.True(term.Traces.Count == 2);
        }

        [Test]
        public void Term_Arith_3()
        {
            //Substraction
            var lst = new List<object>() { 1, -2, -3 };
            var term = new Term(Expression.Multiply, lst);
            object obj = term.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.Equals(6));
            Assert.True(term.Traces.Count == 2);
        }

        [Test]
        public void Term_Arith_4()
        {
            //1+2*3
            var lst = new List<object>() {2, 3};
            var term = new Term(Expression.Multiply, lst);
            var lst1 = new List<object>() {1, term};
            var term1 = new Term(Expression.Add, lst1);
            object obj = term1.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.Equals(7));
            Assert.True(term1.Traces.Count == 2);
        }

        [Test]
        public void Term_Arith_5()
        {
            //1-2*3
            var lst = new List<object>() { 2, 3 };
            var term = new Term(Expression.Multiply, lst);
            var lst1 = new List<object>() { 1, term };
            var term1 = new Term(Expression.Subtract, lst1);
            object obj = term1.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.Equals(-5));
            Assert.True(term1.Traces.Count == 2);
        }

        [Test]
        public void Term_Arith_6()
        {
            //3/2
            var term = new Term(Expression.Divide, new List<object>() {3, 2});
            object obj = term.Eval();
            Assert.True(obj.Equals(1.5));
        }

        [Test]
        public void Term_Arith_7()
        {
            //3^2
            var term = new Term(Expression.Power, new List<object>() { 3, 2 });
            object obj = term.Eval();
            Assert.True(obj.Equals(9));            
        }

        [Test]
        public void Term_Arith_8()
        {
            //9^0.5, not 9^(1/2)
            var term = new Term(Expression.Power, new List<object>() { 9, 0.5 });
            object obj = term.Eval();
            Assert.True(obj.Equals(3));
        }
    }
}
