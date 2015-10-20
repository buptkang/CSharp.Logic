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
    using System.Collections.Generic;
    using System.Linq;

    public abstract partial class ShapeSymbol
    {
        public void GenerateATrace(string strategy)
        {
            var tuple = new Tuple<Object, object>(strategy, _innerLoop); 
            Traces.Add(tuple);
            _innerLoop = new List<TraceStep>();
        }

        public List<TraceStep> _innerLoop = new List<TraceStep>();

        public List<Tuple<object, object>> Traces = new List<Tuple<object, object>>();

        public void CloneTrace()
        {
        }

        public void ClearTrace()
        {
            _innerLoop.Clear();
           Traces.Clear();
        }

        public void ImportTrace(DyLogicObject obj)
        {
            if (obj.Traces.Count == 0) return;
            foreach (var tuple in obj.Traces)
            {
                Traces.Add(tuple);
            }
        }
    }
}
