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

using System.Linq;

namespace CSharpLogic
{
    using System.Collections.Generic;

    public partial class EqGoal
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
        public static bool IsEqGoal(this Equation eq, out object goal, bool allowEval = true)
        {
            goal = null;

            if (!allowEval)
            {
                if (SatisfyGoalCondition(eq))
                {
                    var lst = new List<object>();
                    var goalTemp = new EqGoal(eq);
                    lst.Add(goalTemp);
                    return true;
                }
                return false;
            }

            eq.Eval();
            if (eq.CachedEntities.Count == 1)
            {
                var outEq = eq.CachedEntities.ToList()[0] as Equation;
                if (outEq != null && SatisfyGoalCondition(outEq))
                {
                    goal = new EqGoal(outEq) {Traces = eq.Traces};
                    return true;
                }
            }

            if (eq.CachedEntities.Count > 1)
            {
                var lst = new List<object>();
                foreach (var temp in eq.CachedEntities.ToList())
                {
                    var eqTemp = temp as Equation;
                    if (eqTemp != null && SatisfyGoalCondition(eqTemp))
                    {
                        var goalTemp = new EqGoal(eqTemp) { Traces = eqTemp.Traces };
                        lst.Add(goalTemp);
                    }
                }
                if (lst.Count == 0) return false;
                goal = lst;
                return true;
            }
            return false;
        }

        private static bool SatisfyGoalCondition(Equation eq)
        {
            var lhs = eq.Lhs;
            var rhs = eq.Rhs;
            bool rhsNumeral = LogicSharp.IsNumeric(rhs);
            return Var.IsVar(lhs) && rhsNumeral;
        }
    }
}
