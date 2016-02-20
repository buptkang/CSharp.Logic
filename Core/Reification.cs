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
    using System.Linq;

    public partial class LogicSharp
    {
        private static object ReifyImpl(Object obj, Dictionary<object, object> dict)
        {
            return obj;
        }

        private static object ReifyImpl(Dictionary<object, object> dic, Dictionary<object, object> dict)
        {
            return dic.ToDictionary(pair => pair.Key, pair => Reify(pair.Value, dict));
        }

        private static object ReifyImpl(List<object> list, Dictionary<object, object> dict)
        {
            return ReifyImpl((IEnumerable<object>)list, dict);
        }

        private static object ReifyImpl(Tuple<object, object> tuple, Dictionary<object, object> dict)
        {
            return new Tuple<object, object>(Reify(tuple.Item1, dict), Reify(tuple.Item2, dict));
        }

        private static object ReifyImpl(IEnumerable<object> iter, Dictionary<object, object> dict)
        {
            return iter.Select(obj => Reify(obj, dict)).ToList();
        }

        public static object Reify(object e, Dictionary<object, object> s)
        {
            if (Var.IsVar(e))
            {
                var tempVar = (Var)e;
                return s.ContainsKey(tempVar) ? Reify(s[tempVar], s) : e;
            }

            var term = e as Term;
            if (term != null)
            {
                var gArgs = Reify(term.Args, s);
                if (gArgs.Equals(term.Args))
                {
                    return e;
                }
                else
                {
                    return new Term(term.Op, gArgs);
                }
            }
            dynamic a = e;
            return ReifyImpl(a, s);
        }

        public static object Reify_Object(DyLogicObject logicObj, Dictionary<object, object> s)
        {
            var obj = Reify(logicObj.Properties, s) as Dictionary<object, object>;
            if (LogicSharp.equal_test(obj, logicObj.Properties))
            {
                return logicObj;
            }
            else
            {
                //Initialize a new dynamic object
                var newObj = new DyLogicObject();
                if (obj != null)
                {
                    foreach (var pair in obj)
                    {
                        newObj.Properties.Add(pair.Key, pair.Value);
                    }
                }
                return newObj;
            }
        }
    }
}
