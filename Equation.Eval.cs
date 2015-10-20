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

namespace CSharpLogic
{
    using System.Diagnostics;
    using System.Collections.Generic;

    public partial class Equation
    {
        //Cached symbols for non-concrete objects
        public HashSet<object> CachedEntities { get; set; }
        private HashSet<KeyValuePair<object, object>> CachedObjects;

        #region Evaluation

        public object Eval()
        {
           //verification purpose
           object outputEq;
           return Eval(out outputEq);
        }

        public void UnEval()
        {
            var lhsTerm = Lhs as Term;
            var rhsTerm = Rhs as Term;
            if (lhsTerm != null) lhsTerm.UnEval();
            if (rhsTerm != null) rhsTerm.UnEval();
            ClearTrace();
        }

        public bool? Eval(out object outputEq, bool withTransitive = true,
                            bool lineCheck = false)
        {
            bool? result = EvalEquation(out outputEq, withTransitive, lineCheck);

            if (result != null)
            {
                CachedEntities.Add(outputEq);
                return result;
            }
            var eq1 = outputEq as Equation;
            if (eq1 != null)
            {
                CachedEntities.Add(eq1);
            }
            var lst = outputEq as List<object>;
            if (lst != null)
            {
                foreach (var eq in lst)
                {
                    CachedEntities.Add(eq);
                }
            }
            return null;
        }

        public bool? EvalEquation(out object outputEq, bool withEqRule = true,
                        bool lineCheck = false)
        {
            bool? result = EquationLaws(this, withEqRule, out outputEq, lineCheck);

            var dyObj = outputEq as DyLogicObject;
            if (dyObj != null)
            {
                dyObj.ImportTrace(Traces);
            }
                        
            if (result != null) return result;

            var eqLst = outputEq as List<Equation>;
            if (eqLst == null) return null;

            var outputLst = new List<object>();
            object gOutput;

            foreach (var gEq in eqLst)
            {
                gEq.ClearTrace();
                bool? internalResult = gEq.EquationLaws(gEq, withEqRule, out gOutput, lineCheck);
                if (internalResult != null)
                {
                    return internalResult;
                }
                //transform rootEq trace to each output eq and
                var gEq1 = gOutput as DyLogicObject;
                if (gEq1 != null)
                {
                    var trace = new List<Tuple<object, object>>();

                    Debug.Assert(gEq.Traces.Count == 1);
                    Debug.Assert(this.Traces.Count == 1);

                    var lst1 = this.Traces[0].Item2 as List<TraceStep>;
                    var lst2 = gEq.Traces[0].Item2 as List<TraceStep>;

                    var lst = new List<TraceStep>();
                    if(lst1 != null) lst.AddRange(lst1);
                    if(lst2 != null) lst.AddRange(lst2);

                    var tuple = new Tuple<object, object>(this.Traces[0].Item1, lst);
                    trace.Add(tuple);
                    gEq1.ImportTrace(trace);
                }
                outputLst.Add(gOutput);
            }
            outputEq = outputLst;
            return null;
        }

        private Equation EvalTermInEquation(Equation rootEq, bool isLhs)
        {
            Equation localEq = this;
            object obj = isLhs ? Lhs : Rhs;

            var term = obj as Term;
            if (term == null) return localEq;
            object evalResult = term.Eval();
            if (evalResult.Equals(term)) return localEq;
            var cloneEq = Clone();
            if (isLhs)
            {
                cloneEq.Lhs = evalResult;
            }
            else
            {
                cloneEq.Rhs = evalResult;
            }

            Equation currentEq = FindCurrentEq(rootEq);

            #region Trace Transformation from term to Equation

            if (term.Traces.Count != 0)
            {
                localEq = currentEq;
                foreach (var trace in term.Traces)
                {
                    var strategy = trace.Item1 as string;
                    var lst = trace.Item2 as List<TraceStep>;
                    foreach (var ts in lst)
                    {
                        var cloneEq2 = Generate(currentEq, ts.Source, ts.Target, isLhs);
                        var eqTraceStep = new TraceStep(localEq, cloneEq2, ts.Rule, ts.AppliedRule);
                        rootEq._innerLoop.Add(eqTraceStep);
                        localEq = cloneEq2;
                    }
                    //GenerateATrace(strategy);
                }
            }

            if (term._innerLoop.Count != 0)
            {
                foreach (var ts in term._innerLoop)
                {
                    var cloneEq1 = Generate(currentEq, ts.Source, ts.Target, isLhs);
                    var eqTraceStep = new TraceStep(localEq, cloneEq1, ts.Rule, ts.AppliedRule);
                    rootEq._innerLoop.Add(eqTraceStep);
                    localEq = cloneEq1;
                }
            }
            #endregion

            return cloneEq;
        }

        private bool? EquationLaws(Equation rootEq, bool withEqRule, out object outputEq,
            bool lineCheck = false)
        {
/*            Equation currentEq;
            if (Traces.Count == 0)
            {
                currentEq = this;
            }
            else
            {
                var tuple = Traces[Traces.Count - 1];
                var lst = tuple.Item2 as List<TraceStep>;
                Debug.Assert(lst != null);
                currentEq = lst[lst.Count - 1].Target as Equation;
            }*/
            Equation currentEq = FindCurrentEq(rootEq);
            outputEq = currentEq;

            Debug.Assert(currentEq != null);
            bool hasChange;
            do
            {
                hasChange = false;
                Equation localEq00 = currentEq.EvalTermInEquation(rootEq, true);
                if (!localEq00.Equals(currentEq))
                {
                    hasChange = true;
                    currentEq = localEq00;
                }
                Equation localEq0 = currentEq.EvalTermInEquation(rootEq, false);
                if (!localEq0.Equals(currentEq))
                {
                    hasChange = true;
                    currentEq = localEq0;
                }

                bool? satisfiable = Satisfy(currentEq);
                if (satisfiable != null)
                {
                    if (rootEq._innerLoop.Count != 0)
                    {
                        rootEq.GenerateATrace(EquationsRule.EqStrategy);
                    }
                    outputEq = currentEq;
                    return satisfiable.Value;
                }

                var localObj1 = currentEq.ApplyTransitive(rootEq, withEqRule, lineCheck);

                var localEq1 = localObj1 as Equation;
                var localEqLst = localObj1 as List<Equation>;
                if (localEqLst != null)
                {
                    if (rootEq._innerLoop.Count != 0)
                    {
                        rootEq.GenerateATrace(EquationsRule.EqStrategy);
                    }
                    outputEq = localEqLst;
                    return null;
                }

                if (localEq1 != null && !localEq1.Equals(currentEq))
                {
                    hasChange = true;
                    currentEq = localEq1;
                }

                var localEq2 = localEq1.ApplySymmetric(rootEq);
                if (!localEq2.Equals(localEq1))
                {
                    hasChange = true;
                    currentEq = localEq2;
                }

            } while (hasChange);

            if (rootEq._innerLoop.Count != 0)
            {
                rootEq.GenerateATrace(EquationsRule.EqStrategy);
            }

            outputEq = currentEq;
            return null;
        }

        /// <summary>
        /// The Reflexive Property states that for every real number x, x = x.
        /// </summary>
        /// <param name="equation"></param>
        /// <returns>True: Satisfy, False: Un-satisfy, null: </returns>
        private bool? Satisfy(Equation equation)
        {
            bool lhsNumeric = LogicSharp.IsNumeric(equation.Lhs);
            bool rhsNumeric = LogicSharp.IsNumeric(equation.Rhs);
            if (lhsNumeric && rhsNumeric)
            {
                return equation.Lhs.Equals(equation.Rhs);
            }
            var leftVar = equation.Lhs as Var;
            var rightVar = equation.Rhs as Var;
            if (leftVar != null && rightVar != null)
            {
                bool result = leftVar.Equals(rightVar);
                if (result) return true;
                return null;
            }
            /*            var leftTerm  = equation.Lhs as Term;
                        var rightTerm = equation.Rhs as Term;
                        if (leftTerm != null && rightTerm != null)
                        {

                            return leftTerm.Equals(rightTerm);
                        }*/
            return null;
        }

        #endregion
    }
}
