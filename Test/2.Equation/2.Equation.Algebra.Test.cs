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
        #region One Variable and Linear

        [Test]
        public void Test_Algebra_OneVariable_Linear_1()
        {
            //1+2=x
            var x = new Var('x');
            var lhs = new Term(Expression.Add, new List<object>() { 1, 2 });
            var equation = new Equation(lhs, x);
            bool result = equation.ContainsVar();
            Assert.True(result);
            Assert.True(equation.ToString().Equals("1+2=x"));

            equation.Eval();
            Assert.True(equation.CachedEntities.Count == 1);
            var outputEq = equation.CachedEntities.ToList()[0] as Equation;
            Assert.NotNull(outputEq);
            Assert.True(outputEq.ToString().Equals("x=3"));
            Assert.True(outputEq.Traces.Count == 1);
        }

        [Test]
        public void Test_Algebra_OneVariable_Linear_2()
        {
            //x+2=3
            var x = new Var('x');
            var lhs = new Term(Expression.Add, new List<object>() { x, 2 });
            var equation = new Equation(lhs, 3);
            bool result = equation.ContainsVar();
            Assert.True(result);
            Assert.True(equation.ToString().Equals("x+2=3"));

            equation.Eval();

            Assert.True(equation.CachedEntities.Count == 1);
            var outputEq = equation.CachedEntities.ToList()[0] as Equation;
            Assert.NotNull(outputEq);
            Assert.True(outputEq.ToString().Equals("x=1"));
            Assert.True(outputEq.Traces.Count == 1);
        }

        [Test]
        public void Test_Algebra_OneVariable_Linear_Goal_Gen_1()
        {
            //x = 2+3
            var x = new Var('x');
            var term = new Term(Expression.Add, new List<object>() { 2, 3 });
            var eq = new Equation(x, term);
            object obj;
            bool result = eq.IsEqGoal(out obj);
            Assert.True(result);
            var eqGoal = obj as EqGoal;
            Assert.NotNull(eqGoal);
            Assert.True(eqGoal.Traces.Count == 1);
            Assert.True(eqGoal.Lhs.Equals(x));
            Assert.True(eqGoal.Rhs.Equals(5));
            Assert.True(result);
        }

        [Test]
        public void Goal_Gen_3()
        {
            //x=(-1)*0
            var x = new Var('x');
            var term = new Term(Expression.Multiply, new List<object>() { -1, 0 });
            var eq = new Equation(x, term);
            object obj;
            bool result = eq.IsEqGoal(out obj);
            Assert.True(result);
            var goal = obj as EqGoal;
            Assert.NotNull(goal);
            Assert.True(goal.Rhs.Equals(0));
        }

        [Test]
        public void Goal_Gen_4()
        {
            //-1*x=0
            var x = new Var("x");
            var term = new Term(Expression.Multiply, new List<object>() { -1, x });
            var eq = new Equation(term, 0);
            object obj;
            bool result = eq.IsEqGoal(out obj);
            Assert.True(result);
            var eqGoal = obj as EqGoal;
            Assert.NotNull(eqGoal);
            Assert.True(eqGoal.Rhs.Equals(0));
        }

        [Test]
        public void Goal_Gen_5()
        {
            //1*x=0
            var x = new Var("x");
            var term = new Term(Expression.Multiply, new List<object>() { 1, x });
            var eq = new Equation(term, 0);
            object obj;
            bool result = eq.IsEqGoal(out obj);
            Assert.True(result);
            var eqGoal = obj as EqGoal;
            Assert.NotNull(eqGoal);
            Assert.True(eqGoal.Rhs.Equals(0));
        }

        [Test]
        public void Goal_Gen_11()
        {
            //2-4/(3-y) = 5
            var y = new Var('y');
            var yInverse = new Term(Expression.Multiply, new List<object>() { -1, y });
            var term1 = new Term(Expression.Add, new List<object>() { 3, yInverse });
            var term2 = new Term(Expression.Add, new List<object>() { 2, -4 });
            var term11 = new Term(Expression.Divide, new List<object>() { term2, term1 });
            var eq = new Equation(term11, 5);

            object obj;
            bool result = eq.IsEqGoal(out obj);
            Assert.True(result);
            var gGoal = obj as EqGoal;
            Assert.NotNull(gGoal);
            Assert.True(gGoal.Rhs.Equals(3.4));
        }

        [Test]
        public void Goal_Gen_12()
        {
            //-1*v = 2
            var v = new Var('v');
            var term = new Term(Expression.Multiply, new List<object>() { -1, v });
            var eq = new Equation(term, 2);

            object obj;
            bool result = eq.IsEqGoal(out obj);
            Assert.True(result);
            var gGoal = obj as EqGoal;
            Assert.NotNull(gGoal);
            Assert.True(gGoal.Rhs.Equals(-2));

            Assert.True(gGoal.Traces.Count == 1);
        }

        [Test]
        public void Goal_Gen_10()
        {
            //(3-y)/2-4 = 5
            var y = new Var('y');
            var yInverse = new Term(Expression.Multiply, new List<object>() { -1, y });
            var term1 = new Term(Expression.Add, new List<object>() { 3, yInverse });
            var term2 = new Term(Expression.Add, new List<object>() { 2, -4 });
            var term11 = new Term(Expression.Divide, new List<object>() { term1, term2 });
            var eq = new Equation(term11, 5);

            object obj;
            bool result = eq.IsEqGoal(out obj);
            Assert.True(result);
            var gGoal = obj as EqGoal;
            Assert.NotNull(gGoal);
            Assert.True(gGoal.Rhs.Equals(13));
        }

        #endregion

        #region One Variable and Quadratic (Solving)

        [Test]
        public void Test_Algebra_OneVariable_Quadratic_1()
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
        public void Test_Algebra_OneVariable_Quadratic_2()
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
        public void Test_Algebra_OneVariable_Quadratic_3()
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
        public void Goal_Gen_6()
        {
            // d^2 = (1.0-4.0)^2+(2.0-6.0)^2
            // d>0
            object pt1XCoord = 1.0;
            object pt1YCoord = 2.0;
            object pt2XCoord = 4.0;
            object pt2YCoord = 6.0;

            var variable = new Var('d');
            var lhs = new Term(Expression.Power, new List<object>() { variable, 2.0 });

            var term1 = new Term(Expression.Subtract, new List<object>() { pt1XCoord, pt2XCoord });
            var term11 = new Term(Expression.Power, new List<object>() { term1, 2.0 });

            var term2 = new Term(Expression.Subtract, new List<object>() { pt1YCoord, pt2YCoord });
            var term22 = new Term(Expression.Power, new List<object>() { term2, 2.0 });

            var rhs = new Term(Expression.Add, new List<object>() { term11, term22 });
            //var obj = rhs.Eval();

            //Assert.True(obj.Equals(25));
            var eq = new Equation(lhs, rhs);
            object obj;
            bool result = eq.IsEqGoal(out obj);
            Assert.True(result);
            var lst = obj as List<object>;
            Assert.NotNull(lst);
            Assert.True(lst.Count == 2);
            //Trace Verify

            var gGoal = lst[0] as EqGoal;
            Assert.NotNull(gGoal);
            Assert.True(gGoal.Traces.Count == 1);
            var steps = gGoal.Traces[0].Item2 as List<TraceStep>;
            Assert.NotNull(steps);
            Assert.True(steps.Count == 7);
            var lastStep = steps[6];
            Assert.NotNull(lastStep);
        }

        [Test]
        public void Goal_Gen_7()
        {
            //a^2 = 25
            // 25 = (a-3.0)^2+(6.0-3.0)^2
            object pt1XCoord = new Var("a");
            object pt1YCoord = 6.0;
            object pt2XCoord = -3.0;
            object pt2YCoord = -3.0;

            var term1 = new Term(Expression.Add, new List<object>() { pt1XCoord, pt2XCoord });
            var term11 = new Term(Expression.Power, new List<object>() { term1, 2.0 });
            var term2 = new Term(Expression.Add, new List<object>() { pt1YCoord, pt2YCoord });
            var term22 = new Term(Expression.Power, new List<object>() { term2, 2.0 });
            var rhs = new Term(Expression.Add, new List<object>() { term11, term22 });
            var eq = new Equation(rhs, 25);

            object obj;
            bool result = eq.IsEqGoal(out obj);
            Assert.True(result);
            var lst = obj as List<object>;
            Assert.NotNull(lst);
            Assert.True(lst.Count == 2);
            var gGoal1 = lst[0] as EqGoal;
            Assert.NotNull(gGoal1);
            Assert.True(gGoal1.Traces.Count == 1);
        }

        [Test]
        public void Goal_Gen_8()
        {
            double value = 5.0d;
            var v = new Var("v");

            var term1_1 = new Term(Expression.Multiply, new List<object>() { -1, 5.0 });
            var term1 = new Term(Expression.Add, new List<object>() { 2.0, term1_1 });
            var term11 = new Term(Expression.Power, new List<object>() { term1, 2.0 });
            var term2_2 = new Term(Expression.Multiply, new List<object>() { -1, v });
            var term2 = new Term(Expression.Add, new List<object>() { 4, term2_2 });
            var term22 = new Term(Expression.Power, new List<object>() { term2, 2.0 });
            var rhs = new Term(Expression.Add, new List<object>() { term11, term22 });
            var lhs = new Term(Expression.Power, new List<object>() { value, 2.0 });

            var eq = new Equation(lhs, rhs);

            object obj;
            bool result = eq.IsEqGoal(out obj);
            Assert.True(result);
            var lst = obj as List<object>;
            Assert.NotNull(lst);
            Assert.True(lst.Count == 2);
        }

        [Test]
        public void Goal_Gen_9()
        {
            double value = 5.0d;
            var v = new Var("v");

            var term1_1 = new Term(Expression.Multiply, new List<object>() { -1, 5.0 });
            var term1 = new Term(Expression.Subtract, new List<object>() { 2.0, 5.0 });
            var term11 = new Term(Expression.Power, new List<object>() { term1, 2.0 });
            var term2_2 = new Term(Expression.Multiply, new List<object>() { -1, v });
            var term2 = new Term(Expression.Subtract, new List<object>() { 4, v });
            var term22 = new Term(Expression.Power, new List<object>() { term2, 2.0 });
            var rhs = new Term(Expression.Add, new List<object>() { term11, term22 });
            var lhs = new Term(Expression.Power, new List<object>() { value, 2.0 });

            var eq = new Equation(lhs, rhs);

            object obj;
            bool result = eq.IsEqGoal(out obj);
            Assert.False(result);
            //do not use substract when dealing with variable. use add instead.
        }

        #endregion

        #region Two Variable and Linear (line)

        [Test]
        public void Test_TwoVariables_Linear_1()
        {
            //2x+3y-1=0
            var x = new Var("x");
            var term1 = new Term(Expression.Multiply, new List<object>() { 2, x });
            var y = new Var("y");
            var term2 = new Term(Expression.Multiply, new List<object>() { 3, y });
            var term = new Term(Expression.Add, new List<object>() { term1, term2, -1 });
            var equation = new Equation(term, 0);

            object obj;
            bool? result = equation.Eval(out obj, false);
            Assert.Null(result);
            var outputEq = obj as Equation;
            Assert.NotNull(outputEq);
            Assert.True(outputEq.Equals(equation));

            result = equation.Eval(out obj, true);
            Assert.Null(result);
            outputEq = obj as Equation;
            Assert.NotNull(outputEq);
            Assert.False(outputEq.Equals(equation));
            Assert.True(outputEq.ToString().Equals("2x+3y=1"));
        }

        [Test]
        public void Test_TwoVariables_Linear_2()
        {
            //25: -2x+3y+9=0
            var x = new Var('x');
            var term1 = new Term(Expression.Multiply, new List<object>() { -2, x });
            var y = new Var('y');
            var term2 = new Term(Expression.Multiply, new List<object>() { 3, y });
            var lhs = new Term(Expression.Add, new List<object>() { term1, term2, 9 });
            var eq = new Equation(lhs, 0);
            object obj;
            bool? result = eq.EvalEquation(out obj, true, true);            
            Assert.Null(result);
        }

        [Test]
        public void Test_TwoVariables_Linear_3()
        {
            //25: 3y=2x-9
            var y = new Var('y');
            var lhs = new Term(Expression.Multiply, new List<object>() {3, y});
            var x = new Var('x');
            var term1 = new Term(Expression.Multiply, new List<object>() {2, x});
            var rhs = new Term(Expression.Add, new List<object>() {term1, -9});
            var eq = new Equation(lhs, rhs);
            object obj;
            bool? result = eq.EvalEquation(out obj, true, true);
            Assert.Null(result);
            Assert.True(obj.ToString().Equals("-2x+3y+9=0"));
        }

        [Test]
        public void Test_LineGen_2()
        {
            /* //4y=x
             var x = new Var("x");
             var y = new Var("y");
             var term2 = new Term(Expression.Multiply, new List<object>() { 4, y });
             var equation = new Equation(term2, x);

             object obj;
             bool?  result = equation.Eval(out obj, true);
             Assert.Null(result);
             var outputEq = obj as Equation;
             Assert.NotNull(outputEq);*/
        }

        #endregion

        #region Two Variable and Quadratic (Circle)

        [Test]
        public void Test_Algebra_TwoVariables_Quadratic_1()
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

        [Test]
        public void Test_Algebra_TwoVariables_Quadratic_2()
        {
            //(x+1)^2+y^2=6^2
        }

        #endregion
    }
}
