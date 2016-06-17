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
    public class AlgebraTest
    {
        #region Commutative Law

        [Test]
        public void Test_Commutative_1()
        {
            //3+x -> x+3             
            var x = new Var('x');
            var a = new Term(Expression.Add, new List<object>() { 3, x });
            object result = a.EvalAlgebra();
            var gTerm = result as Term;
            Assert.NotNull(gTerm);
            Assert.NotNull(result);
            Assert.True(result.ToString().Equals("x+3"));
            Assert.True(a.Traces.Count == 1);
        }

        [Test]
        public void Test_Commutative_1_NonLinear_1()
        {
            // 3+x^2 -> x^2+3
            var x = new Var('x');
            var x_square = new Term(Expression.Power, new List<object>() {x, 2});
            var term = new Term(Expression.Add, new List<object>() {3, x_square});
            object result = term.EvalAlgebra();
            var gTerm = result as Term;
            Assert.NotNull(gTerm);
            Assert.NotNull(result);
            Assert.True(result.ToString().Equals("x^2+3"));
        }

        [Test]
        public void Test_Commutative_1_NonLinear_2()
        {
            // x+x^2 -> x^2+x
            var x = new Var('x');
            var x_square = new Term(Expression.Power, new List<object>() { x, 2 });
            var term = new Term(Expression.Add, new List<object>() { x, x_square });
            object result = term.EvalAlgebra();
            var gTerm = result as Term;
            Assert.NotNull(gTerm);
            Assert.NotNull(result);
            Assert.True(result.ToString().Equals("x^2+x"));
        }

        [Test]
        public void Test_Commutative_2()
        {
            //x*3 -> 3*x
            var x = new Var('x');
            var a = new Term(Expression.Multiply, new List<object>() { x, 3 });
            object result = a.EvalAlgebra();
            Assert.NotNull(result);
            Assert.True(result.ToString().Equals("3x"));
            Assert.True(a.Traces.Count == 1);
        }

        [Test]
        public void Test_Commutative_2_NonLinear()
        {
            //x^2*3 -> 3*x^2
            var x = new Var('x');
            var x_square = new Term(Expression.Power, new List<object>() { x, 2 });
            var a = new Term(Expression.Multiply, new List<object>() { x_square, 3 });
            object result = a.EvalAlgebra();
            Assert.True(result.ToString().Equals("3(x^2)"));
        }

        [Test]
        public void Test_Commutative_3()
        {
            //3*x*3 -> 9*x
            var x = new Var('x');
            var a = new Term(Expression.Multiply, new List<object>() { 3, x, 3 });
            object result = a.EvalAlgebra();
            Assert.NotNull(result);
            Assert.True(result.ToString().Equals("9x"));
            Assert.True(a.Traces.Count == 1);
        }

        [Test]
        public void Test_Commutative_3_NonLinear()
        {
            //3*x^2*3 -> 9*x
            var x = new Var('x');
            var x_square = new Term(Expression.Power, new List<object>() { x, 2 });
            var a = new Term(Expression.Multiply, new List<object>() { 3, x_square, 3 });
            object result = a.EvalAlgebra();
            Assert.NotNull(result);
            Assert.True(result.ToString().Equals("9(x^2)"));
            Assert.True(a.Traces.Count == 1);
        }

        [Test]
        public void Test_Commutative_4()
        {
            //1+(1+a) -> (a+1)+1
            var a = new Var('a');
            var term = new Term(Expression.Add, new List<object>() { 1, a });
            var term1 = new Term(Expression.Add, new List<object>() { 1, term });
            Assert.True(term1.ToString().Equals("1+1+a"));

            object result = term1.EvalAlgebra();
            Assert.NotNull(result);
            Assert.True(result.ToString().Equals("a+2"));
            Assert.True(term1.Traces.Count == 1);

            var lst = term1.Traces[0].Item2 as List<TraceStep>;
            Assert.NotNull(lst);
            Assert.True(lst.Count == 4);
        }

        [Test]
        public void Test_Commutative_5()
        {
            //(1+a)*2 -> 2*(a+1)
            var a = new Var('a');
            var term = new Term(Expression.Add, new List<object>() { 1, a });
            var term1 = new Term(Expression.Multiply, new List<object>() { term, 2 });
            Assert.True(term1.ToString().Equals("(1+a)2"));

            object result = term1.EvalAlgebra();
            Assert.NotNull(result);
            Assert.True(result.ToString().Equals("2a+2"));
            Assert.True(term1.Traces.Count == 1);

            var lst = term1.Traces[0].Item2 as List<TraceStep>;
            Assert.NotNull(lst);
            Assert.True(lst.Count == 4);
        }

        #endregion

        #region Identity

        [Test]
        public void Test_Identity_1()
        {
            //x+x->(1+1)x->2x
            var x = new Var('x');
            var term = new Term(Expression.Add, new List<object>() { x, x });
            object result = term.EvalAlgebra();
            Assert.NotNull(result);
            Assert.True(result.ToString().Equals("2x"));
            Assert.True(term.Traces.Count == 1);
        }

        [Test]
        public void Test_Identity_2()
        {
            //y+2*y ->1*y+2*y
            var y = new Var('y');
            var term = new Term(Expression.Multiply, new List<object>() { 2, y });
            var term1 = new Term(Expression.Add, new List<object>() { y, term });
            object result = term1.EvalAlgebra();
            Assert.NotNull(result);
            Assert.True(result.ToString().Equals("3y"));
            Assert.True(term1.Traces.Count == 1);
        }

        #endregion

        #region Zero Law

        [Test]
        public void Test_Zero_1()
        {
            //x+0=>x
            var x = new Var('x');
            var term = new Term(Expression.Add, new List<object>() { x, 0 });
            Assert.True(term.ToString().Equals("x+0"));
            var result = term.Eval();
            Assert.True(result.ToString().Equals("x"));
            Assert.NotNull(result);
            Assert.True(term.Traces.Count == 1);
        }

        [Test]
        public void Test_Zero_2()
        {
            //0*x=>0
            var x = new Var('x');
            var term = new Term(Expression.Multiply, new List<object>() { 0, x });
            //Assert.True(term.ToString().Equals("0x"));
            var result = term.Eval();
            Assert.NotNull(result);
            Assert.True(result.Equals(0));
            Assert.True(term.Traces.Count == 1);
        }

        #endregion

        #region Distributive

        [Test]
        public void Test_Distributive_01()
        {
            //3*3*y
            var y = new Var('y');
            var term = new Term(Expression.Multiply, new List<object>() { 3, 3, y });
            object obj = term.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.ToString().Equals("9y"));
            Assert.True(term.Traces.Count == 1);
        }

        [Test]
        public void Test_Distributive_0()
        {
            //(1+1)*y
            var term  = new Term(Expression.Add, new List<object>() { 1, 1 });
            var y = new Var('y');
            var term1 = new Term(Expression.Multiply, new List<object>() { term, y });
            object obj = term1.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.ToString().Equals("2y"));
            Assert.True(term1.Traces.Count == 1);

            //(1+1+1)*y -> (2+1)*y
            term = new Term(Expression.Add, new List<object>() { 1, 1 ,1});
            term1 = new Term(Expression.Multiply, new List<object>() { term, y });
            obj = term1.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.ToString().Equals("3y"));
            Assert.True(term1.Traces.Count == 1);
        }

        [Test]
        public void Test_Distributive_1()
        {
            //y+y=> 1*y+1*y=> (1+1)*y => 2*y
            var y = new Var('y');
            var term = new Term(Expression.Add, new List<object>(){y, y});
            //Assert.True(term.ToString().Equals("(y+y)"));
            object obj = term.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.ToString().Equals("2y"));
            Assert.True(term.Traces.Count == 1);
        }

        [Test]
        public void Test_Distributive_2()
        {
            // y+2*y  -> (1+2)*y  
            var y = new Var('y');
            var term1 = new Term(Expression.Multiply, new List<object>() { 2, y });
            var term = new Term(Expression.Add, new List<object>() { y, term1 });
            object obj = term.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.ToString().Equals("3y"));
            Assert.True(term.Traces.Count == 1);
        }

        [Test]
        public void Test_Distributive_3()
        {
            // y+2*y-4*y  -> -1*y
            var y = new Var('y');
            var term1 = new Term(Expression.Multiply, new List<object>() { 2, y });
            var term2 = new Term(Expression.Multiply, new List<object>() { -4, y });
            var term = new Term(Expression.Add, new List<object>() {y, term1, term2});
            object obj = term.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.ToString().Equals("-y"));
            Assert.True(term.Traces.Count == 1);
        }

        [Test]
        public void Test_Distributive_4()
        {
            //a*y+y+x           -> (a+1)*y+x
            var x = new Var('x');
            var y = new Var('y');
            var a = new Var('a');
            var term1 = new Term(Expression.Multiply, new List<object>() { a, y });
            var term = new Term(Expression.Add, new List<object>() {term1, y, x});
            object obj = term.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.ToString().Equals("x+(a+1)y"));
            Assert.True(term.Traces.Count == 1);
        }

        [Test]
        public void Test_Distributive_5()
        {
            //3*(x+1) -> 3x + 3
            var x = new Var('x');
            var term = new Term(Expression.Add, new List<object>() { x, 1 });
            var term1 = new Term(Expression.Multiply, new List<object> { 3, term });
            object obj = term1.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.ToString().Equals("3x+3"));
            Assert.True(term1.Traces.Count == 1);
        }

        [Test]
        public void Test_Distributive_6()
        {
            //a*(x+1) -> ax+a
            var x = new Var('x');
            var term = new Term(Expression.Add, new List<object>() { x, 1 });
            var a = new Var('a');
            var term1 = new Term(Expression.Multiply, new List<object> { a, term });
            //TODO
            /*object obj = term1.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.ToString().Equals("((a*x)+a)"));
            Assert.True(term1.Traces.Count == 7);*/
        }

        [Test]
        public void Test_Distributive_7()
        {
            //quadratic:
            //3x^2+x^2 -> (3+1)x^2 -> 4x^2
            var x      = new Var('x');
            var xTerm = new Term(Expression.Power, new List<object>() {x,2});
            var t1 = new Term(Expression.Multiply, new List<object>() {3, xTerm});            
            Assert.True(t1.QuadraticTerm());

            var x2 = new Var('x');
            var t2 = new Term(Expression.Power, new List<object>() {x2, 2});
            Assert.True(t2.QuadraticTerm());

            var t3 = new Term(Expression.Add, new List<object>() {t1, t2});
            Assert.True(t3.QuadraticTerm());

            object obj = t3.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.ToString().Equals("4(x^2)"));
        }

        [Test]
        public void Test_Distributive_8()
        {
            //1*y^2+2*y^2     -> (1+2)*y^2
            var y = new Var('y');
            var yTerm = new Term(Expression.Power, new List<object>() { y, 2 });
            var t1 = new Term(Expression.Multiply, new List<object>() { 1, yTerm });
            Assert.True(t1.QuadraticTerm());

            var y2 = new Var('y');
            var yTerm2 = new Term(Expression.Power, new List<object>() { y2, 2 });
            var t2 = new Term(Expression.Multiply, new List<object>() { 2, yTerm2 });
            Assert.True(t2.QuadraticTerm());

            var t3 = new Term(Expression.Add, new List<object>() { t1, t2 });
            Assert.True(t3.QuadraticTerm());

            object obj = t3.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.ToString().Equals("3(y^2)"));
        }

        [Test]
        public void Test_Distributive_9()
        {
            //x^2+x^2+x^2     -> (1+1+1)*x^2
            var x = new Var('x');
            var t1 = new Term(Expression.Power, new List<object>() { x, 2 });

            var x2 = new Var('x');
            var t2 = new Term(Expression.Power, new List<object>() { x2, 2 });

            var x3 = new Var('x');
            var t3 = new Term(Expression.Power, new List<object>() { x3, 2});

            var t4 = new Term(Expression.Add, new List<object>() { t1, t2, t3});

            object obj = t4.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.ToString().Equals("3(x^2)"));
        }

        [Test]
        public void Test_Distributive_10()
        {
            //2*x^2+x^2+y     -> (2+1)x^2+y
            var x = new Var('x');
            var xTerm = new Term(Expression.Power, new List<object>() { x, 2 });
            var t1 = new Term(Expression.Multiply, new List<object>() {2, xTerm});

            var x2 = new Var('x');
            var t2 = new Term(Expression.Power, new List<object>() { x2, 2 });

            var t3 = new Var('y');

            var t4 = new Term(Expression.Add, new List<object>() { t1, t2, t3 });
            object obj = t4.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.ToString().Equals("3(x^2)+y"));
        }

        [Test]
        public void Test_Distributive_11()
        {
            //x/3+9 -> (x+27)/3
            var x = new Var('x');
            var term1 = new Term(Expression.Divide, new List<object>() { x, 3 });
            var term2 = new Term(Expression.Add, new List<object>() { term1, 9 });

            object obj = term2.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.ToString().Equals("(x+27)/3"));
        }

        [Test]
        public void Test_Distributive_12()
        {
            //(2/3)x+2 -> (2*x+6)/3
            var x = new Var('x');
            var term1 = new Term(Expression.Divide, new List<object>() {2, 3});
            var term2 = new Term(Expression.Multiply, new List<object>() {term1, x});
            var term3 = new Term(Expression.Add, new List<object>(){term2, 2});

            object obj = term3.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.ToString().Equals("(2x+6)/3"));
        }

        #endregion

        #region Associative

        [Test]
        public void Test_Associative_1()
        {
            //(3*(2*x)) => (3*2)*x
            var x = new Var('x');
            var term  = new Term(Expression.Multiply, new List<object>() {2, x});
            var term1 = new Term(Expression.Multiply, new List<object>() {3, term});
            object obj = term1.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.ToString().Equals("6x"));
            Assert.True(term1.Traces.Count == 1);
        }

        [Test]
        public void Test_Associative_2()
        {
            //(a+1)+1 -> a+(1+1)
            var a = new Var('a');
            var term = new Term(Expression.Add, new List<object>() { a, 1 });
            var term1 = new Term(Expression.Add, new List<object>() { term, 1 });
            object obj = term1.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.ToString().Equals("a+2"));
            Assert.True(term1.Traces.Count == 1);
        }

        [Test]
        public void Test_Associative_3()
        {
            //(1/3)x -> (1*x)/3
            var x = new Var('x');
            var term = new Term(Expression.Divide, new List<object>() {1, 3});
            var term1 = new Term(Expression.Multiply, new List<object>() {term, x});

            object obj = term1.Eval();
            Assert.NotNull(obj);
            var gTerm = obj as Term;
            Assert.NotNull(gTerm);
            Assert.True(gTerm.Op.Method.Name.Equals("Divide"));
            Assert.True(obj.ToString().Equals("x/3"));
        }

        [Test]
        public void Test_Associative_4()
        {
            //(2/3)x -> (2*x)/3
            var x = new Var('x');
            var term = new Term(Expression.Divide, new List<object>() { 2, 3 });
            var term1 = new Term(Expression.Multiply, new List<object>() { term, x });

            object obj = term1.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.ToString().Equals("(2x)/3"));
        }

        [Test]
        public void Test_Associative_5()
        {
//            //(x+1)+(y+1) -> x+y+2
            var x = new Var('x');
            var y = new Var('y');

            var term1 = new Term(Expression.Add, new List<object>() {x, 1});
            var term2 = new Term(Expression.Add, new List<object>() {y, 1});
            var term = new Term(Expression.Add, new List<object>() {term1, term2});

            object obj = term.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.ToString().Equals("x+y+2"));
        }

        #endregion

        #region Integrated

        [Test]
        public void Test_LinePatterMatch()
        {
            //(2*y)+(2*x)+(-1*y)+(2*x)+4
            var x = new Var('x');
            var y = new Var('y');

            var term1 = new Term(Expression.Multiply, new List<object>() { 2, y });
            var term2 = new Term(Expression.Multiply, new List<object>() { 2, x });
            var term3 = new Term(Expression.Multiply, new List<object>() { -1, y });
            var term4 = new Term(Expression.Multiply, new List<object>() { 2, x });
            var term = new Term(Expression.Add, new List<object>() { term1, term2, term3, term4, 4});
            object obj = term.Eval();
            Assert.NotNull(obj);
            var gTerm = obj as Term;
            Assert.NotNull(gTerm);
            var lst = gTerm.Args as List<object>;
            Assert.NotNull(lst);
            Assert.True(lst.Count == 3);
        }

        [Test]
        public void Test_LinePatternMatch2()
        {
            //4*x+y+-1*4
            var x = new Var('x');
            var y = new Var('y');
            var term1 = new Term(Expression.Multiply, new List<object>() { 4, x });
            var term3 = new Term(Expression.Multiply, new List<object>() { -1, 4});
            var term = new Term(Expression.Add, new List<object>() { term1, y, term3});
            object obj = term.Eval();
            Assert.NotNull(obj);
            var gTerm = obj as Term;
        }

        [Test]
        public void Test_Simple_1()
        {
            var variable = new Var('x');
            var term1 = new Term(Expression.Multiply, new List<object>() { 2, variable });
            var term2 = new Term(Expression.Multiply, new List<object>() { -1, variable});
            //2x-x -> x
            var term = new Term(Expression.Add, new List<object>() { term1, term2 });
            var obj = term.Eval();
            Assert.True(obj.Equals(variable));
        }

        [Test]
        public void Test_Line_Match_0()
        {
            //3y+2x-9
            var y = new Var('y');
            var term1 = new Term(Expression.Multiply, new List<object>() { 3, y });
            var x = new Var('x');
            var term2 = new Term(Expression.Multiply, new List<object>() { 2, x });           
            var term3 = new Term(Expression.Add, new List<object>() { term1, term2,-9 });

            var obj = term3.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.ToString().Equals("2x+3y-9"));
        }

        [Test]
        public void Test_Line_Match_1()
        {
            //3y-(2x-9)
            var y = new Var('y');
            var term1 = new Term(Expression.Multiply, new List<object>() {3, y});
            var x = new Var('x');
            var term2 = new Term(Expression.Multiply, new List<object>() {2, x});
            var term3 = new Term(Expression.Add, new List<object>() {term2, -9});
            var term4 = new Term(Expression.Multiply, new List<object>() {-1, term3});
            var term5 = new Term(Expression.Add, new List<object>() {term1, term4});

            object obj = term4.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.ToString().Equals("-2x+9"));

            obj = term5.Eval();
            Assert.NotNull(obj);
            Assert.True(obj.ToString().Equals("-2x+3y+9"));
        }

        #endregion

        #region reification

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

        [Test]
        public void Term_Algebra_Reify_2()
        {
            /*
             *  //a*b=> 
             *  //a=1
             */
            var y = new Var('y');
            var a = new Var('a');
            var b = new Var('b');
            var term = new Term(Expression.Multiply, new List<object>() { a, b });
            //Assert.True(term.ToString().Equals("(y+y)"));
            var eqGoal = new EqGoal(a, 1);
            var term1 = term.Reify(eqGoal) as Term;
            Assert.NotNull(term1);
            var obj = term1.Eval() as Var;
            Assert.NotNull(obj);
        }

        #endregion
    }
}