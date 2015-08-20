using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace CSharpLogic
{
    public partial class EqGoal : Equation, Goal
    {
        #region Goal Inheritance Functions

        public bool Reify(Dictionary<object, object> substitutions)
        {
            Lhs = LogicSharp.Reify(Lhs, substitutions);
            Rhs = LogicSharp.Reify(Rhs, substitutions);

            if (Var.ContainsVar(Lhs) || Var.ContainsVar(Rhs))
            {
                return true;
            }
            else
            {
                return Lhs.Equals(Rhs);
            }
        }

        public bool Unify(Dictionary<object, object> substitutions)
        {
            return Functor(substitutions);
        }

        public bool EarlySafe()
        {
            return !(Var.ContainsVar(Lhs) && Var.ContainsVar(Rhs));
        }

        #endregion
    }

    public static class EquationExtension
    {
        public static bool IsEqGoal(this Equation eq, out EqGoal goal)
        {
            goal = null;
            Equation outputEq;
            bool? result = eq.Eval(out outputEq, false);
            if (result != null) return false;

            if (SatisfyGoalCondition(outputEq))
            {
                goal = new EqGoal(outputEq);
                goal.Traces = eq.Traces;
                return true;
            }

            result = eq.Eval(out outputEq, true);
            if (result != null) return false;

            //MoveTerms()
            //TODO Goal Eval
            return false;
        }

        private static bool SatisfyGoalCondition(Equation eq)
        {
            var lhs = eq.Lhs;
            var rhs = eq.Rhs;
            return Var.IsVar(lhs);
        }

        private static Equation MoveTerms(Equation eq)
        {
            //TODO
            return null;
        }
    }
}
