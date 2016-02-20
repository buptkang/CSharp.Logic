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

using System.Diagnostics;
using System.Linq;

namespace CSharpLogic
{
    public partial class Equation
    {
        public bool Reify(EqGoal goal)
        {
            var lhsTerm = Lhs as Term;
            var rhsTerm = Rhs as Term;
            var lhsVar = Lhs as Var;
            var rhsVar = Rhs as Var;

            string strategy = "Reify equation's internal variable by substituing a given fact.";

            if (lhsVar != null)
            {
                var lhsNum = LogicSharp.Reify(lhsVar, goal.ToDict());
                if (lhsNum != null && !lhsNum.Equals(lhsVar))
                {
                    var cloneEq = Clone();
                    cloneEq.Lhs = lhsNum;

                    string rule = SubstitutionRule.ApplySubstitute();
                    string appliedRule = SubstitutionRule.ApplySubstitute(this, cloneEq);
                    var ts = new TraceStep(this, cloneEq, SubstitutionRule.SubstituteKC(), rule, appliedRule);
                    cloneEq._innerLoop.Add(ts);
                    cloneEq.GenerateATrace(strategy);
                    CachedEntities.Add(cloneEq);
                    return true;
                }
            }

            if (rhsVar != null)
            {
                var rhsNum = LogicSharp.Reify(rhsVar, goal.ToDict());
                if (rhsNum != null && !rhsNum.Equals(lhsVar))
                {
                    
                    var cloneEq = Clone();
                    cloneEq.Rhs = rhsNum;
                    string rule = SubstitutionRule.ApplySubstitute();
                    string appliedRule = SubstitutionRule.ApplySubstitute(this, goal);
                    var ts = new TraceStep(this, cloneEq, SubstitutionRule.SubstituteKC(), rule, appliedRule);
                    cloneEq._innerLoop.Add(ts);
                    cloneEq.GenerateATrace(strategy);
                    CachedEntities.Add(cloneEq);
                    return true;
                }                
            }

            if(lhsTerm != null)
            {
                var term1 = lhsTerm.Reify(goal) as Term;
                if (lhsTerm.Equals(term1) || term1 == null)
                {
                    return false;
                }
                var obj = term1.Eval(); 

                var cloneEq = Clone();
                cloneEq.Lhs = obj;
                string rule = SubstitutionRule.ApplySubstitute();
                string appliedRule = SubstitutionRule.ApplySubstitute(this, goal);
                var ts = new TraceStep(this, cloneEq, SubstitutionRule.SubstituteKC(), rule, appliedRule);
                cloneEq._innerLoop.Add(ts);
                cloneEq.GenerateATrace(strategy);
                CachedEntities.Add(cloneEq);

                return true;
            }

            if (rhsTerm != null)
            {
                object obj = rhsTerm.Reify(goal);
                if (rhsTerm.Equals(obj))
                {
                    return false;
                }
                var cloneEq = Clone();
                cloneEq.Rhs = obj;
                string rule = SubstitutionRule.ApplySubstitute();
                string appliedRule = SubstitutionRule.ApplySubstitute(this, goal);
                var ts = new TraceStep(this, cloneEq, SubstitutionRule.SubstituteKC(), rule, appliedRule);
                cloneEq._innerLoop.Add(ts);
                cloneEq.GenerateATrace(strategy);

                CachedEntities.Add(cloneEq);
                return true;
            }
            return false;
        }

        public bool UnReify(EqGoal goal)
        {
            if (CachedEntities.Count == 0) return false;

            var goalVar = goal.Lhs as Var;
            Debug.Assert(goalVar != null);

            if (ContainsVar(goalVar))
            {
                CachedEntities.Clear();
                return true;
            }
            return false;
        }
    }
}
