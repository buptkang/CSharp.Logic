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
    using System.Linq;

    public abstract partial class ShapeSymbol
    {
        public List<TraceStep> Traces = new List<TraceStep>();
        public List<string> StrategyTraces = new List<string>();

        public void CloneTrace(out List<TraceStep> steps, out List<string> strategy)
        {
            steps = Traces.Select(ts => ts.Clone()).ToList();
            strategy = StrategyTraces;
        }

        public void ClearTrace()
        {
            Traces = new List<TraceStep>();
            StrategyTraces = new List<string>();
        }
    }
}
