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

    public partial class Equation
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

            if (term == null) throw new Exception("Cannot be null");
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
