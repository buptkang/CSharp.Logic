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

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace CSharpLogic
{
    using System;

    public partial class Equation
    {
        public Equation FindCurrentEq(Equation rootEq)
        {
            Equation currentEq;
            if (rootEq._innerLoop.Count != 0)
            {
                currentEq = rootEq._innerLoop[rootEq._innerLoop.Count - 1].Target as Equation;
                if (currentEq == null) throw new Exception("Must be equation here");
            }
            else
            {
                if (rootEq.Traces.Count == 0)
                {
                    currentEq = rootEq;
                }
                else
                {
                    var lastTrace = rootEq.Traces[rootEq.Traces.Count - 1].Item2 as List<TraceStep>;
                    Debug.Assert(lastTrace != null);
                    currentEq = lastTrace[lastTrace.Count - 1].Target as Equation;
                }
            }
            return currentEq;
        }

        public Equation Generate(Equation currentEq, object source, object target, bool isLhs)
        {
            Equation cloneEq = currentEq.Clone();
            if (isLhs)
            {
                if (!source.Equals(cloneEq.Lhs))
                {
                    //throw new Exception("Equation.Trace.cs 1: Must be equal.");
                }
                cloneEq.Lhs = target;
            }
            else
            {
                if (!source.Equals(cloneEq.Rhs))
                {
                    //throw new Exception("Equation.Trace.cs 2: Must be equal.");
                }
                cloneEq.Rhs = target;
            }
            return cloneEq;
        }
    }
}