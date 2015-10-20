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
            var eqGoal = obj as EqGoal;
            if (eqGoal != null)
            {
                if (Rhs == null) return Lhs.Equals(eqGoal.Lhs);

                bool isNum1 = LogicSharp.IsNumeric(Rhs);
                bool isNum2 = LogicSharp.IsNumeric(eqGoal.Rhs);
                bool result;
                if (isNum1 && isNum2)
                {
                    result = LogicSharp.NumericEqual(Rhs, eqGoal.Rhs);
                }
                else
                {
                    result = Rhs.Equals(eqGoal.Rhs);
                }
                return Lhs.Equals(eqGoal.Lhs) && result;
            }

            var eq = obj as Equation;
            if (eq != null)
            {
                if (Rhs == null) return Lhs.Equals(eq.Lhs);
                bool isNum1 = LogicSharp.IsNumeric(Rhs);
                bool isNum2 = LogicSharp.IsNumeric(eq.Rhs);
                bool result;
                if (isNum1 && isNum2)
                {
                    result = LogicSharp.NumericEqual(Rhs, eq.Rhs);
                }
                else
                {
                    result = Rhs.Equals(eq.Rhs);
                }

                if (eq.Lhs == null) return false;
                return Lhs.ToString().Equals(eq.Lhs.ToString()) && result;
            }

            return false;
        }

        public bool ApproximateMatch(object obj)
        {
            var eqGoal = obj as EqGoal;
            if (eqGoal != null)
            {
                if (Rhs == null) return Lhs.Equals(eqGoal.Lhs);
                bool isNum1 = LogicSharp.IsNumeric(Rhs);
                bool isNum2 = LogicSharp.IsNumeric(eqGoal.Rhs);
                bool result;
                if (isNum1 && isNum2)
                {
                    result = LogicSharp.NumericApproximateEqual(Rhs, eqGoal.Rhs);
                }
                else
                {
                    result = Rhs.Equals(eqGoal.Rhs);
                }
                return Lhs.Equals(eqGoal.Lhs) && result;
            }

            var eq = obj as Equation;
            if (eq != null)
            {
                if (Rhs == null) return Lhs.Equals(eq.Lhs);
                bool isNum1 = LogicSharp.IsNumeric(Rhs);
                bool isNum2 = LogicSharp.IsNumeric(eq.Rhs);
                bool result;
                if (isNum1 && isNum2)
                {
                    result = LogicSharp.NumericApproximateEqual(Rhs, eq.Rhs);
                }
                else
                {
                    result = Rhs.Equals(eq.Rhs);
                }

                if (eq.Lhs == null) return false;
                return Lhs.ToString().Equals(eq.Lhs.ToString()) && result;
            }

            return false;
        }


        public bool Concrete
        {
            get { return Var.IsVar(Lhs) && LogicSharp.IsNumeric(Rhs); }
        }

        public override int GetHashCode()
        {
            if (Rhs == null) return Lhs.GetHashCode();
            return Lhs.GetHashCode() ^ Rhs.GetHashCode();
        }

        public override string ToString()
        {
            double dNum;
            bool isNum = LogicSharp.IsDouble(Rhs, out dNum);

            if (isNum)
            {
                int iNum;
                bool isInteger = LogicSharp.IsInt(Rhs, out iNum);
                if (isInteger) return base.ToString();

                double roundD = Math.Round(dNum, 4);
                return string.Format("{0}={1}", Lhs, roundD);
            }
            return base.ToString();
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
            throw new Exception("TODO");

           /* //pre-processing of goal
            EqGoal tempGoal;
            if (goal.Traces.Count != 0)
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
            return tempGoal;*/
        }
    }
}
