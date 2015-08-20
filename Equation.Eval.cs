using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CSharpLogic
{
    public partial class Equation : DyLogicObject, IEquationLogic, IEval
    {
        public object Eval()
        {
            //verification purpose
            Equation outputEq;

            bool? result = Eval(out outputEq, false);
            if (result != null)
            {
                CachedEntities.Add(result);
            }

            return null;
        }

        public void UnEval()
        {
            var lhsTerm = Lhs as Term;
            var rhsTerm = Rhs as Term;
            if (lhsTerm != null) lhsTerm.UnEval();
            if (rhsTerm != null) rhsTerm.UnEval();
            ClearTrace();
        }

        public bool? Eval(out Equation outputEq, bool withTransitive = true)
        {
            #region Term Proprocessing
            var lhsTerm = Lhs as Term;
            if (lhsTerm != null)
            {
                object lhs = lhsTerm.Eval();
                if (!lhsTerm.Equals(lhs))
                {
                    TransformTermTrace(true);
                }
            }

            var rhsTerm = Rhs as Term;
            if (rhsTerm != null)
            {
                object rhs = rhsTerm.Eval();
                if (!rhsTerm.Equals(rhs))
                {
                    TransformTermTrace(false);
                }
            }
            #endregion

            return EvalEquation(out outputEq, withTransitive);
        }

        public bool? EvalEquation(out Equation outputEq, bool withEqRule = true)
        {
            return EquationLaws(this, withEqRule, out outputEq);
        }

        private Equation EvalLeftSide(Equation rootEq)
        {
            Equation localEq = this;
            var lhsTerm = Lhs as Term;
            if (lhsTerm != null)
            {
                object lhs = lhsTerm.Eval();
                if (!lhsTerm.Equals(lhs))
                {
                    var cloneEq = Clone();
                    cloneEq.Lhs = lhs;

                    #region Trace Generation
                    if (lhsTerm.Traces.Count != 0)
                    {
                        foreach (var ts in lhsTerm.Traces)
                        {
                            var cloneEq1 = Generate(localEq, ts.Source, ts.Target, true);
                            var eqTraceStep = new TraceStep(localEq, cloneEq1, ts.Rule, ts.AppliedRule);
                            rootEq.Traces.Add(eqTraceStep);
                            localEq = cloneEq1;
                        }
                    }
                    #endregion

                    localEq = cloneEq;
                }
            }
            return localEq;
        }

        private Equation EvalRightSide(Equation rootEq)
        {
            Equation localEq = this;
            var rhsTerm = Rhs as Term;
            if (rhsTerm != null)
            {
                object rhs = rhsTerm.Eval();
                if (!rhsTerm.Equals(rhs))
                {
                    var cloneEq = Clone();
                    cloneEq.Rhs = rhs;
                    #region Trace Generation
                    if (rhsTerm.Traces.Count != 0)
                    {
                        foreach (var ts in rhsTerm.Traces)
                        {
                            var cloneEq1 = Generate(localEq, ts.Source, ts.Target, false);
                            var eqTraceStep = new TraceStep(localEq, cloneEq1, ts.Rule, ts.AppliedRule);
                            rootEq.Traces.Add(eqTraceStep);
                            localEq = cloneEq1;
                        }
                    }
                    #endregion

                    localEq = cloneEq;
                }
            }
            return localEq;
        }

        private bool? EquationLaws(Equation rootEq, bool withEqRule, out Equation outputEq)
        {
            Equation currentEq;
            if (Traces.Count == 0)
            {
                currentEq = this;
            }
            else
            {
                currentEq = Traces[Traces.Count - 1].Target as Equation;
            }
            outputEq = currentEq;

            Debug.Assert(currentEq != null);
            bool hasChange;
            do
            {
                hasChange = false;
                bool? satisfiable = Satisfy(currentEq);
                if (satisfiable != null)
                {
                    outputEq = currentEq;
                    return satisfiable.Value;
                }
                Equation localEq00 = currentEq.EvalLeftSide(rootEq);
                if (!localEq00.Equals(currentEq))
                {
                    hasChange = true;
                    currentEq = localEq00;
                }
                Equation localEq0 = currentEq.EvalRightSide(rootEq);
                if (!localEq0.Equals(currentEq))
                {
                    hasChange = true;
                    currentEq = localEq0;
                }

                var localEq1 = currentEq.ApplyTransitive(rootEq, withEqRule);
                if (!localEq1.Equals(currentEq))
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
    }
}
