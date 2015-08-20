using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CSharpLogic
{
    public interface Goal
    {
        bool EarlySafe();
        bool Reify(Dictionary<object, object> substitutions);
        bool Unify(Dictionary<object, object> substitutions);
    }

    public partial class EqGoal : Equation, Goal
    {
        private Func<Dictionary<object, object>, bool> Functor;

        public EqGoal(object lhs, object rhs, bool generated = false):
            base(lhs,rhs,generated)
        {
            Debug.Assert(lhs is Var);
            Functor = LogicSharp.Equal()(Lhs, Rhs);
        }

        public EqGoal(Equation eq) : base(eq)
        {
            Debug.Assert(Lhs is Var);
            Debug.Assert(Rhs != null);
            Functor = LogicSharp.Equal()(Lhs, Rhs);
        }

        public override bool Equals(object obj)
        {
            var eq = obj as EqGoal;
            if (eq != null)
            {
                if (Rhs == null) return Lhs.Equals(eq.Lhs);
                return Lhs.Equals(eq.Lhs) && Rhs.Equals(eq.Rhs);
            }
            return false;
        }

        public override int GetHashCode()
        {
            if (Rhs == null) return Lhs.GetHashCode();
            return Lhs.GetHashCode() ^ Rhs.GetHashCode();
        }
    }

    public static class EqGoalExtension
    {
        public static Dictionary<object, object> ToDict(this EqGoal goal)
        {
            object variable;
            object value;
            if (Var.IsVar(goal.Lhs))
            {
                variable = goal.Lhs;
                value = goal.Rhs;
            }
            else
            {
                variable = goal.Rhs;
                value = goal.Lhs;
            }

            var pair = new KeyValuePair<object, object>(variable, value);
            var substitute = new Dictionary<object, object>();
            substitute.Add(pair.Key, pair.Value);
            return substitute;
        }

        /// <summary>
        /// Trace Derivation purpose
        /// </summary>
        /// <param name="goal"></param>
        /// <returns></returns>
        public static EqGoal GetLatestDerivedGoal(this EqGoal goal)
        {
            //pre-processing of goal
            EqGoal tempGoal;
            if (goal.TraceCount != 0)
            {
                var trace = goal.Traces[0];
                Debug.Assert(trace.Target != null);
                var traceGoal = trace.Target as EqGoal;
                Debug.Assert(traceGoal != null);
                tempGoal = traceGoal;
            }
            else
            {
                tempGoal = goal;
            }
            return tempGoal;
        }
    }
}
