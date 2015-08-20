using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpLogic
{
    public partial class Equation : DyLogicObject, IEquationLogic,IEval
    {
        /// <summary>
        /// Transform Term trace -> Equation trace
        /// </summary>
        public void TransformTermTrace(bool isLhs)
        {
            Term term;
            if (isLhs)
            {
                term = Lhs as Term;
            }
            else
            {
                term = Rhs as Term;
            }
            Equation currentEq;
            if (Traces.Count == 0)
            {
                currentEq = this;
            }
            else
            {
                currentEq = Traces[Traces.Count - 1].Target as Equation;
                if (currentEq == null) throw new Exception("Must be equation here");
            }
        
            if(term == null) throw new Exception("Cannot be null");
            if (term.Traces.Count != 0)
            {
                Equation localEq = currentEq;
                foreach (var ts in term.Traces)
                {
                    var cloneEq = Generate(localEq, ts.Source, ts.Target, isLhs);
                    var eqTraceStep = new TraceStep(localEq, cloneEq, ts.Rule, ts.AppliedRule);
                    Traces.Add(eqTraceStep);
                    localEq = cloneEq;
                }
            }
        }

        public Equation Generate(Equation currentEq, object source, object target, bool isLhs)
        {
            if (currentEq.Traces.Count != 0)
            {
                currentEq = Traces[Traces.Count - 1].Target as Equation;
                if (currentEq == null) throw new Exception("Must be equation here");
            }

            Equation cloneEq = currentEq.Clone();
            if (isLhs)
            {
                if (!source.Equals(currentEq.Lhs))
                {
                    //throw new Exception("Equation.Trace.cs 1: Must be equal.");
                }
                cloneEq.Lhs = target;
            }
            else
            {
                if (!source.Equals(currentEq.Rhs))
                {
                    throw new Exception("Equation.Trace.cs 2: Must be equal.");
                }
                cloneEq.Rhs = target;
            }
            return cloneEq;
        }

        public void GenerateTrace(Equation currentEq, Equation cloneEq, string rule, string appliedRule)
        {
            var ts = new TraceStep(currentEq, cloneEq, rule, appliedRule);
            Traces.Add(ts);
        }
    }
}
