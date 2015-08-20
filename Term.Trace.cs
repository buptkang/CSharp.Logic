using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CSharpLogic
{
    /// <summary>
    /// Substitution or term
    /// </summary>
    public partial class Term : DyLogicObject
        , IAlgebraLogic, IEval
    {
        public void GenerateTrace(object source, object target, 
                                  string rule, string appliedRule)
        {
            Term currentTerm;
            if (Traces.Count == 0)
            {
                currentTerm = this;
            }
            else
            {
                currentTerm = Traces[Traces.Count - 1].Target as Term;
                if (currentTerm == null) throw new Exception("Must be term here");
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
                var ts = new TraceStep(currentTerm, objj, rule, appliedRule);
                Traces.Add(ts);
            }
            else
            {
                var targetTerm = target as Term;
                if (targetTerm != null)
                {
                    object objj = targetTerm.ReConstruct();
                    var ts = new TraceStep(currentTerm, objj, rule, appliedRule);
                    Traces.Add(ts);
                }
                else
                {
                    var ts = new TraceStep(currentTerm, target, rule, appliedRule);
                    Traces.Add(ts);                    
                }
            }
        }

        private bool SearchArithList(object obj1, object obj2, out List<object> output)
        {
            output = null;
            var lst = Args as List<object>;
            Debug.Assert(lst != null);

            for (int i = 0; i < lst.Count-1; i++)
            {
                var localTerm = lst[i] as Term;
                if (localTerm != null)
                {
                    bool result = localTerm.SearchArithList(obj1, obj2, out output);
                    if (result) return true;
                }

                localTerm = lst[i+1] as Term;
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
            Debug.Assert(lst!= null);
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
