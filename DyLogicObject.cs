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
    using System.Dynamic;
    using System.Linq;

    public class DyLogicObject : DynamicObject
    {
        public List<TraceStep> Traces = new List<TraceStep>();
        public int TraceCount { get { return Traces.Count; } }

        //Tutoring Space
        public List<string> StrategyTraces = new List<string>();
        public bool StrategyShowed = false;

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

        #region Dynamic Properties

        public readonly Dictionary<object, object> Properties 
                                    = new Dictionary<object, object>();

        public int Count
        {
            get { return Properties.Count; }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string name = binder.Name.ToLower();
            return Properties.TryGetValue(name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            // Converting the property name to lowercase 
            // so that property names become case-insensitive.
            Properties[binder.Name.ToLower()] = value;

            // You can always add a value to a dictionary, 
            // so this method always returns true. 
            return true;
        }

        #endregion
    }

    public static class DyLogicObjectExtension
    {
        public static void Reify(this DyLogicObject logicObj, Goal goal)
        {
            goal.Unify(logicObj.Properties);
        }

        public static void Reify(this DyLogicObject logicObj, IEnumerable<Goal> goals)
        {
            IEnumerable<KeyValuePair<object, object>> pairs =
                LogicSharp.logic_All(goals, logicObj.Properties);

            if (pairs == null)
            {
                return;
            }

            foreach (KeyValuePair<object, object> pair in pairs)
            {
                if (!logicObj.Properties.ContainsKey(pair.Key))
                {
                    logicObj.Properties.Add(pair.Key, pair.Value);
                }
            }
        }
    }
}
