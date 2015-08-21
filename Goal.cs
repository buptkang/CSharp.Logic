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
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    public interface Goal
    {
        bool EarlySafe();
        bool Reify(Dictionary<object, object> substitutions);
        bool Unify(Dictionary<object, object> substitutions);
    }

    public partial class EqGoal : Equation, Goal
    {
        private Func<Dictionary<object, object>, bool> Functor;

        public EqGoal(object lhs, object rhs, bool generated = false) :
            base(lhs, rhs, generated)
        {
            Debug.Assert(lhs is Var);
            Functor = LogicSharp.Equal()(Lhs, Rhs);
        }

        public EqGoal(Equation eq)
            : base(eq)
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
