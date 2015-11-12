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

namespace CSharpLogicTest
{
    using CSharpLogic;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using NUnit.Framework;

    [TestFixture]
    public class TestTerm
    {
        [Test]
        public void UnifyTest()
        {
            var term1 = new Term(Expression.Add, new List<object>() { 1, 1 });
            var x = new Var('x');
            var term2 = new Term(Expression.Add, new List<object>() { 1, x });

            var dict = new Dictionary<object, object>();
            bool result = LogicSharp.Unify(term1, term2, dict);

            Assert.True(result);
            Assert.True(dict.Count == 1);
        }

        [Test]
        public void Test_print()
        {
            var variable = new Var('x');
            var term = new Term(Expression.Multiply, new Tuple<object, object>(1, variable));

            Assert.True(term.ToString().Equals("(1*x)"));
        }

        [Test]
        public void Test_print2()
        {
            var term00 = new Term(Expression.Add, new List<object>(){1,1});
            var x = new Var('x');
            var term0 = new Term(Expression.Multiply, new List<object>() {term00, x});

            var term1 = new Term(Expression.Add, new List<object>() {term0, 1, -4});

            Assert.True(term00.ToString().Equals("1+1"));

            Assert.True(term0.ToString().Equals("(1+1)x"));
            
            Assert.True(term1.ToString().Equals("(1+1)x+1-4"));
        }

        [Test]
        public void Test_print3()
        {
            var x = new Var('x');
            var term0 = new Term(Expression.Multiply, new List<object>() {3, x});
            var y = new Var('y');
            var term1 = new Term(Expression.Multiply, new List<object>() {-1, y});
            var term = new Term(Expression.Add, new List<object>() {term0, term1});
            Assert.True(term.ToString().Equals("3x-y"));
        }

        [Test]
        public void Test_Print4()
        {
            //d=2^2
            var t1 = new Term(Expression.Power, new List<object>() {2, 2});
            Assert.True(t1.ToString().Equals("2^2"));
            //d= (3+4)^0.5
            var t2 = new Term(Expression.Add, new List<object>() {3, 4});
            var t3 = new Term(Expression.Power, new List<object>() {t2, 0.5});
            Assert.True(t3.ToString().Equals("(3+4)^0.5"));
        }

        [Test]
        public void Test_Print5()
        {
            //x=-1+5/2
            var t1 = new Term(Expression.Divide, new List<object>() {5, 2});
            var t2 = new Term(Expression.Add, new List<object>() {-1, t1});
            Assert.True(t1.ToString().Equals("5/2"));
            Assert.True(t2.ToString().Equals("-1+5/2"));
        }

        [Test]
        public void Test_Print6()
        {
            //x=(-1+5)/2
            var t1 = new Term(Expression.Add, new List<object>() { -1, 5 });
            var t2 = new Term(Expression.Divide, new List<object>() { t1, 2 });
            Assert.True(t1.ToString().Equals("-1+5"));
            Assert.True(t2.ToString().Equals("(-1+5)/2"));
        }

        public void Test_Print7()
        {
            //2+-1*5
            var t1 = new Term(Expression.Multiply, new List<object>() {-1, 5});
            var t2 = new Term(Expression.Add, new List<object>() {2, t1});

            Assert.True(t1.ToString().Equals("-1*5"));
        }

        public void Test_Print8()
        {
            var x = new Var("x");
            var y = new Var("y");

            var t1 = new Term(Expression.Multiply, new List<object>() {4, x});
            var t2 = new Term(Expression.Multiply, new List<object>() {-1, t1});


            Assert.True(t2.ToString().Equals("a"));
        }


        [Test]
        public void Test_containVar()
        {
            var variable = new Var('x');
            var term = new Term(Expression.Add, new Tuple<object, object>(variable, 2));
            Assert.True(term.ContainsVar(variable));

            var term2 = new Term(Expression.Add, new Tuple<object, object>(term, 1.0));
            Assert.True(term2.ContainsVar(variable));
        }

        [Test]
        public void Test_Clone()
        {
            int a = 1;
            var b = new Var('b');
            int c = 1;
            var lst = new List<object>()
            {
                a,
                b,
                c
            };
            var term = new Term(Expression.Add, lst);
            var term1 = term.Clone();

            var args = term.Args as List<object>;
            Assert.NotNull(args);
            args[0] = 2;

            var args1 = term1.Args as List<object>;
            Assert.NotNull(args1);
            Assert.True(args1[0].Equals(1));
        }

        [Test]
        public void Test_Equal()
        {
            var x = new Var('x');
            var term1 = new Term(Expression.Add, new List<object>() { x, 1 });
            var term2 = new Term(Expression.Add, new List<object>() { x, 1 });
            Assert.True(term1.Equals(term2));
        }

        [Test]
        public void Test_Reconstruct()
        {
            var term = new Term(Expression.Add, new List<object>() { 1 });
            Assert.True(term.ReConstruct().Equals(1));
        }

    }
}