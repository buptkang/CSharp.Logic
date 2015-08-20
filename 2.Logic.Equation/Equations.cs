using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace CSharpLogic
{
    public interface IEquationLogic
    {
        bool? EvalEquation(out Equation outputEq, bool withEqRules = true);
    }

    public static class EquationEvalExtension
    {
        /// <summary>
        /// if x = y, then y = x
        /// </summary>
        /// <param name="currentEq"></param>
        /// <param name="rootEq"></param>
        /// <returns></returns>
        public static Equation ApplySymmetric(this Equation currentEq, Equation rootEq)
        {
            Equation localEq = currentEq;
            object lhs = currentEq.Lhs;
            object rhs = currentEq.Rhs;
            if (SatisfySymmetricCondition(lhs, rhs))
            {
                var cloneEq = currentEq.Clone();
                object tempObj = cloneEq.Lhs;
                cloneEq.Lhs = cloneEq.Rhs;
                cloneEq.Rhs = tempObj;

                string rule = EquationsRule.Rule(EquationsRule.EquationRuleType.Symmetric);

                string appliedRule = EquationsRule.Rule(
                          EquationsRule.EquationRuleType.Symmetric,
                          localEq, null);
                rootEq.GenerateTrace(localEq, cloneEq, rule, appliedRule);
                localEq = cloneEq;
            }
            return localEq;
        }

        private static bool SatisfySymmetricCondition(object lhs, object rhs)
        {
            bool lhsNumeric = LogicSharp.IsNumeric(lhs);
            bool rhsNumeric = LogicSharp.IsNumeric(rhs);

            if (lhsNumeric && !rhsNumeric)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Inverse Properties
        /// 1. For any number a, there exists a number x such that a+x=0.
        /// 2. If a is any number except 0, there exists a number x such that ax = 1.
        /// </summary>
        /// <param name="currentEq"></param>
        /// <param name="rootEq"></param>
        /// <returns></returns>
        public static bool ApplyInverse(this Equation currentEq, Equation rootEq)
        {
            return false;
        }

        /// <summary>
        /// if x = y and y = z, then x = z
        /// if x = y, then x + a = y + a
        /// if x^2 = y^2, then x = y
        /// if x = y, then ax = ay
        /// ax = ay -> x=y 
        /// </summary>
        /// <param name="goal"></param>
        /// <param name="gGoal"></param>
        /// <returns></returns>
        public static Equation ApplyTransitive(this Equation currentEq, Equation rootEq, bool withEqRule)
        {
            Equation localEq = currentEq;
            object lhs = currentEq.Lhs;
            object rhs = currentEq.Rhs;

            if (withEqRule)
            {
                if (SatisfyTransitiveCondition1(lhs, rhs))
                {
                    #region Condition1
                    var cloneEq = currentEq.Clone();
                    var inverseRhs = new Term(Expression.Multiply, new List<object>() { -1, rhs });
                    var lhsTerm = cloneEq.Lhs as Term;
                    if (lhsTerm != null)
                    {
                        var cloneLst = lhsTerm.Args as List<object>;
                        Debug.Assert(cloneLst != null);
                        if (lhsTerm.Op.Method.Name.Equals("Add"))
                        {
                            cloneLst.Add(inverseRhs);
                        }
                        else
                        {
                            cloneEq.Lhs = new Term(Expression.Add, new List<object>() { lhs, inverseRhs });
                        }
                    }
                    else
                    {
                        cloneEq.Lhs = new Term(Expression.Add, new List<object>() { lhs, inverseRhs });
                    }
                    cloneEq.Rhs = new Term(Expression.Add, new List<object>() { rhs, inverseRhs });
                    string rule = EquationsRule.Rule(EquationsRule.EquationRuleType.Transitive);
                    string appliedRule = EquationsRule.Rule(
                              EquationsRule.EquationRuleType.Transitive,
                              localEq, null);
                    rootEq.GenerateTrace(localEq, cloneEq, rule, appliedRule);
                    localEq = cloneEq;
                    #endregion
                }                
            }

            if (SatisfyTransitiveCondition2(lhs, rhs))
            {
                #region Condition2

                var cloneEq = currentEq.Clone();

                var lhsTerm = cloneEq.Lhs as Term;
                Debug.Assert(lhsTerm != null);

                var cloneLst = lhsTerm.Args as List<object>;
                Debug.Assert(cloneLst != null);
                cloneEq.Lhs = cloneLst[0];
                cloneEq.Rhs = new Term(Expression.Power, new List<object>() { cloneEq.Rhs, 0.5 });
                string rule = EquationsRule.Rule(EquationsRule.EquationRuleType.Transitive);
                string appliedRule = EquationsRule.Rule(
                          EquationsRule.EquationRuleType.Transitive,
                          localEq, null);
                rootEq.GenerateTrace(localEq, cloneEq, rule, appliedRule);
                localEq = cloneEq;
              
                #endregion
            }

            return localEq;
        }

      private static bool SatisfyTransitiveCondition2(object lhs, object rhs)
      {
            bool rhsNumeric = LogicSharp.IsNumeric(rhs);
            var lhsTerm = lhs as Term;
            if (lhsTerm != null)
            {
                if (lhsTerm.Op.Method.Name.Equals("Power") && rhsNumeric)
                {
                    return true;
                }
            }           
            return false;
        }

        private static bool SatisfyTransitiveCondition1(object lhs, object rhs)
        {
            bool rhsNumeric = LogicSharp.IsNumeric(rhs);

            if (rhsNumeric && !0.0.Equals(rhs) && !0.Equals(rhs))
            {
                return true;
            }
            return false;
        }
    }
}
