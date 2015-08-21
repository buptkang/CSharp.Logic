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
        public static bool IsEqGoal(this Equation eq, out object goal)
        {
            goal = null;
            Equation outputEq;
            bool? result = eq.Eval(out outputEq, false);
            if (result != null) return false;

            if (SatisfyGoalCondition(outputEq))
            {
                var eqGoal = new EqGoal(outputEq);
                eqGoal.Traces = eq.Traces;
                goal = eqGoal;
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
