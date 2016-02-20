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
    using System.Collections.Generic;
    using System.Dynamic;

    public class DyLogicObject : DynamicObject
    {
        #region Trace API

        public List<TraceStep> _innerLoop = new List<TraceStep>();

        public List<Tuple<object,object>> Traces = new List<Tuple<object, object>>();

        public List<Tuple<object, object>> CloneTrace()
        {
            return null;
        }

        public void GenerateATrace(string strategy)
        {
            var tuple = new Tuple<Object, object>(strategy, _innerLoop); 
            Traces.Add(tuple);
            _innerLoop = new List<TraceStep>();
        }

        public void ImportTrace(List<Tuple<object,object>> trace)
        {
            Traces = trace;
        }

        public void ImportTrace(DyLogicObject obj)
        {
            foreach (var tuple in obj.Traces)
            {
                Traces.Add(tuple);
            }
            foreach (var tempObj in obj._innerLoop)
            {
                _innerLoop.Add(tempObj);                
            }
        }

        public void ClearTrace()
        {
            _innerLoop.Clear();
           Traces.Clear();
        }

        #endregion

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
