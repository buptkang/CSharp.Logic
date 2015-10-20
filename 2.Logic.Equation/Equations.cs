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

using System;
using System.Diagnostics.SymbolStore;

namespace CSharpLogic
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq.Expressions;

    public interface IEquationLogic
    {
        bool? EvalEquation(out object outputEq, bool withEqRules = true, bool lineCheck = false);
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

                var ts = new TraceStep(localEq, cloneEq, rule, appliedRule);
                rootEq._innerLoop.Add(ts);
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
        public static object ApplyTransitive(this Equation currentEq, Equation rootEq, bool withEqRule,
               bool lineCheck = false)
        {
            Equation localEq = currentEq;
            object lhs = currentEq.Lhs;
            object rhs = currentEq.Rhs;

            if (!withEqRule) return localEq;

            //Power Inverse
            if (SatisfyTransitiveCondition2(lhs, rhs))
            {
                #region Condition2

                var cloneEq = currentEq.Clone();
                var cloneEq2 = currentEq.Clone();

                var lhsTerm = cloneEq.Lhs as Term;
                Debug.Assert(lhsTerm != null);
                var cloneLst = lhsTerm.Args as List<object>;
                Debug.Assert(cloneLst != null);
                cloneEq.Lhs = cloneLst[0];
                cloneEq.Rhs = new Term(Expression.Power, new List<object>() { cloneEq.Rhs, 0.5 });

                var lhsTerm2 = cloneEq2.Lhs as Term;
                Debug.Assert(lhsTerm2 != null);
                var cloneLst2 = lhsTerm2.Args as List<object>;
                Debug.Assert(cloneLst2 != null);
                cloneEq2.Lhs = cloneLst2[0];
                var internal1 = new Term(Expression.Power, new List<object>() { cloneEq2.Rhs, 0.5 });
                cloneEq2.Rhs = new Term(Expression.Multiply, new List<object>() { -1, internal1 });

                string rule = EquationsRule.Rule(EquationsRule.EquationRuleType.Transitive);
                string appliedRule = EquationsRule.Rule(
                          EquationsRule.EquationRuleType.Transitive,
                          localEq, null);
                var ts = new TraceStep(localEq, cloneEq, rule, appliedRule);
                rootEq._innerLoop.Add(ts);
                //localEq = cloneEq;

                var lst = new List<Equation>();
                lst.Add(cloneEq);
                lst.Add(cloneEq2);
                return lst;
                #endregion
            }

            if (!lineCheck)
            {
                //Add Inverse
                if (SatifyTransitiveCondition0(lhs, rhs))
                {
                    #region condition0

                    var cloneEq = currentEq.Clone();

                    var rhsTerm = new Term(Expression.Add, new List<object>() {cloneEq.Rhs});

                    var lhsTerm = cloneEq.Lhs as Term;
                    Debug.Assert(lhsTerm != null);

                    var lst = lhsTerm.Args as List<object>;
                    Debug.Assert(lst != null);

                    for (int i = 0; i < lst.Count; i++)
                    {
                        var temp = lst[i];
                        bool isNumber = LogicSharp.IsNumeric(temp);
                        if (isNumber)
                        {
                            var inverseRhs = new Term(Expression.Multiply, new List<object>() {-1, temp});
                            lst.Remove(temp);
                            var rhsArgLst = rhsTerm.Args as List<object>;
                            Debug.Assert(rhsArgLst != null);
                            rhsArgLst.Add(inverseRhs);
                            break;
                        }

                        var term = temp as Term;
                        if (term != null && !term.ContainsVar())
                        {
                            var inverseRhs = new Term(Expression.Multiply, new List<object>() {-1, temp});
                            lst.Remove(i);
                            var rhsArgLst = rhsTerm.Args as List<object>;
                            Debug.Assert(rhsArgLst != null);
                            rhsArgLst.Add(inverseRhs);
                            break;
                        }
                    }

                    cloneEq.Rhs = rhsTerm;
                    if (lst.Count == 1)
                    {
                        cloneEq.Lhs = lst[0];
                    }

                    string rule = EquationsRule.Rule(EquationsRule.EquationRuleType.Transitive);
                    string appliedRule = EquationsRule.Rule(
                        EquationsRule.EquationRuleType.Transitive,
                        localEq, null);

                    var traceStep = new TraceStep(localEq, cloneEq, rule, appliedRule);
                    rootEq._innerLoop.Add(traceStep);
                    localEq = cloneEq;
                    return localEq;

                    #endregion
                }
            }
            else
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
                    cloneEq.Rhs = 0;
                    string rule = EquationsRule.Rule(EquationsRule.EquationRuleType.Transitive);
                    string appliedRule = EquationsRule.Rule(
                              EquationsRule.EquationRuleType.Transitive,
                              localEq, null);

                    var traceStep = new TraceStep(localEq, cloneEq, rule, appliedRule);
                    rootEq._innerLoop.Add(traceStep);
                    localEq = cloneEq;
                    #endregion
                }                
            }

            //Mutliply Inverse
            if (SatisfyTransitiveCondition3(lhs, rhs))
            {
                #region condition3

                var cloneEq = currentEq.Clone();

                var rhsTerm = new Term(Expression.Multiply, new List<object>() { cloneEq.Rhs });

                var lhsTerm = cloneEq.Lhs as Term;
                Debug.Assert(lhsTerm != null);

                var lst = lhsTerm.Args as List<object>;
                Debug.Assert(lst != null);

                for (int i = 0; i < lst.Count; i++)
                {
                    var temp = lst[i];
                    bool isNumber = LogicSharp.IsNumeric(temp);
                    if (isNumber)
                    {
                        var inverseRhs = new Term(Expression.Divide, new List<object>() { 1, temp });
                        lst.Remove(temp);
                        var rhsArgLst = rhsTerm.Args as List<object>;
                        Debug.Assert(rhsArgLst != null);
                        rhsArgLst.Add(inverseRhs);
                        break;
                    }

                    var term = temp as Term;
                    if (term != null && !term.ContainsVar())
                    {
                        var inverseRhs = new Term(Expression.Divide, new List<object>() { 1, temp });
                        lst.Remove(i);
                        var rhsArgLst = rhsTerm.Args as List<object>;
                        Debug.Assert(rhsArgLst != null);
                        rhsArgLst.Add(inverseRhs);
                        break;
                    }
                }

                cloneEq.Rhs = rhsTerm;
                if (lst.Count == 1)
                {
                    cloneEq.Lhs = lst[0];
                }

                string rule = EquationsRule.Rule(EquationsRule.EquationRuleType.Transitive);
                string appliedRule = EquationsRule.Rule(
                          EquationsRule.EquationRuleType.Transitive,
                          localEq, null);

                var traceStep = new TraceStep(localEq, cloneEq, rule, appliedRule);
                rootEq._innerLoop.Add(traceStep);
                localEq = cloneEq;
                return localEq;

                #endregion
            }

            //Divide Inverse
            if (SatisfyTransitiveCondition4(lhs, rhs))
            {
                #region condition4
                var cloneEq = currentEq.Clone();

                var lhsTerm = cloneEq.Lhs as Term;
                Debug.Assert(lhsTerm != null);

                var lst = lhsTerm.Args as List<object>;
                Debug.Assert(lst != null);
                Debug.Assert(lst.Count == 2);

                bool numerator = LogicSharp.IsNumeric(lst[0]);
                bool deNumerator = LogicSharp.IsNumeric(lst[1]);

                if (deNumerator)
                {
                    var rhsTerm = new Term(Expression.Multiply, new List<object>() { lst[1], cloneEq.Rhs });
                    var newEq = new Equation(lst[0], rhsTerm);

                    string rule = EquationsRule.Rule(EquationsRule.EquationRuleType.Transitive);
                    string appliedRule = EquationsRule.Rule(
                              EquationsRule.EquationRuleType.Transitive,
                              localEq, newEq);

                    var traceStep = new TraceStep(localEq, newEq, rule, appliedRule);
                    rootEq._innerLoop.Add(traceStep);
                    localEq = newEq;
                    return localEq;
                }


                if (numerator)
                {
                    var rhsTerm = new Term(Expression.Divide, new List<object>() { lst[0], cloneEq.Rhs });
                    var newEq = new Equation(lst[1], rhsTerm);
                    string rule = EquationsRule.Rule(EquationsRule.EquationRuleType.Transitive);
                    string appliedRule = EquationsRule.Rule(
                              EquationsRule.EquationRuleType.Transitive,
                              localEq, newEq);

                    var traceStep = new TraceStep(localEq, newEq, rule, appliedRule);
                    rootEq._innerLoop.Add(traceStep);
                    localEq = newEq;
                    return localEq;
                }

                #endregion
            }

      

            return localEq;
        }

        /*
         * Move sqare root head to the rhs
         * E.g  x^2=4 -> x=4^(0.5)
         * 
         */
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

        /*
         * E.g 2/y=4, x/2=4
         */
        private static bool SatisfyTransitiveCondition4(object lhs, object rhs)
        {
            bool rhsNumeric = LogicSharp.IsNumeric(rhs);
            if (!rhsNumeric) return false;

            var lhsTerm = lhs as Term;
            if (lhsTerm == null) return false;
            if (!lhsTerm.Op.Method.Name.Equals("Divide")) return false;
            var lst = lhsTerm.Args as List<object>;
            Debug.Assert(lst != null);
            if (lst.Count != 2) return false;
            foreach (var temp in lst)
            {
                bool isNumber = LogicSharp.IsNumeric(temp);
                if (isNumber) return true;
            }
            return false;
        }

        /*
         * E.g -1*x=2
         */
        private static bool SatisfyTransitiveCondition3(object lhs, object rhs)
        {
            bool rhsNumeric = LogicSharp.IsNumeric(rhs);
            if (!rhsNumeric) return false;

            var lhsTerm = lhs as Term;
            if (lhsTerm == null) return false;
            if (!lhsTerm.Op.Method.Name.Equals("Multiply")) return false;
            var lst = lhsTerm.Args as List<object>;
            Debug.Assert(lst != null);
            foreach (var temp in lst)
            {
                bool isNumber = LogicSharp.IsNumeric(temp);
                if (isNumber) return true;
                var term = temp as Term;
                if (term != null && !term.ContainsVar())
                {
                    return true;
                }
            }
            return false;
        }

        /*
         *  Move lhs numeral terms to the rhs.
         *  E.g x+2=4  => x = 4-2 
         */
        private static bool SatifyTransitiveCondition0(object lhs, object rhs)
        {
            bool rhsNumeric = LogicSharp.IsNumeric(rhs);
            if (!rhsNumeric) return false;

            var lhsTerm = lhs as Term;
            if (lhsTerm == null) return false;
            if (!lhsTerm.Op.Method.Name.Equals("Add")) return false;
            var lst = lhsTerm.Args as List<object>;
            Debug.Assert(lst != null);
            foreach (var temp in lst)
            {
                bool isNumber = LogicSharp.IsNumeric(temp);
                if (isNumber) return true;
                var term = temp as Term;
                if (term != null && !term.ContainsVar())
                {
                    return true;
                }
            }
            return false;
        }

        private static bool SatisfyTransitiveCondition1(object lhs, object rhs)
        {
              if (!0.0.Equals(rhs) && !0.Equals(rhs))
              {
                  return true;
              }
            return false;
        }
    }
}
