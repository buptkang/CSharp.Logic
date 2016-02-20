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
    using System.Diagnostics;

    public partial class Term
    {
        public void GenerateTrace(object source, object target, object concept,
                                  string metaScaffold, string scaffold)
        {
            Term currentTerm;
            if (_innerLoop.Count == 0)
            {
                currentTerm = this;
            }
            else
            {
                currentTerm = _innerLoop[_innerLoop.Count - 1].Target as Term;
                if (currentTerm == null)
                {
                    return;
                }
            }
            Term cloneTerm = currentTerm.Clone();
            List<object> lst;
            bool isFound = cloneTerm.SearchList(source, out lst);
            if (isFound)
            {
                Debug.Assert(lst != null);
                var index = lst.FindIndex(x => x.Equals(source));
                lst[index] = target;
                object objj = cloneTerm.ReConstruct();
                var ts = new TraceStep(currentTerm, objj, concept, metaScaffold, scaffold);
                _innerLoop.Add(ts);
            }
            else
            {
                var targetTerm = target as Term;
                if (targetTerm != null)
                {
                    object objj = targetTerm.ReConstruct();
                    var ts = new TraceStep(currentTerm, objj, concept, metaScaffold, scaffold);
                    _innerLoop.Add(ts);
                }
                else
                {
                    var ts = new TraceStep(currentTerm, target, concept, metaScaffold, scaffold);
                    _innerLoop.Add(ts);
                }
            }
        }

        private bool SearchArithList(object obj1, object obj2, out List<object> output)
        {
            output = null;
            var lst = Args as List<object>;
            Debug.Assert(lst != null);

            for (int i = 0; i < lst.Count - 1; i++)
            {
                var localTerm = lst[i] as Term;
                if (localTerm != null)
                {
                    bool result = localTerm.SearchArithList(obj1, obj2, out output);
                    if (result) return true;
                }

                localTerm = lst[i + 1] as Term;
                if (localTerm != null)
                {
                    bool result = localTerm.SearchArithList(obj1, obj2, out output);
                    if (result) return true;
                }

                if (lst[i].Equals(obj1) && lst[i + 1].Equals(obj2))
                {
                    output = lst;
                    return true;
                }
            }
            return false;
        }

        private bool SearchList(object obj, out List<object> returnLst)
        {
            returnLst = null;
            var lst = Args as List<object>;
            Debug.Assert(lst != null);
            foreach (var tempObj in lst)
            {
                if (tempObj.Equals(obj))
                {
                    returnLst = lst;
                    return true;
                };
                var localTerm = tempObj as Term;
                if (localTerm != null)
                {
                    bool result = localTerm.SearchList(obj, out returnLst);
                    if (result)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
