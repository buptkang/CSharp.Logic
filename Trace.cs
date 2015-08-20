using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CSharpLogic
{
    public class TraceStep
    {
        public object Source { get; set; }

        public object Target { get; set; }

        public object Rule { get; set; }

        public object AppliedRule { get; set; }

        public TraceStep(object source, object target, object rule, object appliedRule)
        {
            Source = source;
            Target = target;
            Rule = rule;
            AppliedRule = appliedRule;
        }

        public TraceStep Clone()
        {
            return new TraceStep(Source, Target, Rule, AppliedRule);
        }
    }

    public static class TraceUtils
    {
/*        public static void MoveTraces(Term fromTerm, Term toTerm)
        {
            List<TraceStep> traces = fromTerm.Traces;
            if (traces.Count == 0) return;
            toTerm.Traces.AddRange(traces);
        }*/

        public static object Generate(this Term term, object obj1, object obj)
        {
            var originLst = term.Args as List<object>;
            Debug.Assert(originLst != null);
            if (originLst.Count == 1) return obj;

            int index = -1;
            for (int i = 0; i < originLst.Count; i++)
            {
                if (originLst[i].Equals(obj1))
                {
                    index = i;
                    break;
                }
            }
            Debug.Assert(index != -1);
            var lst = new List<object>();
            lst.AddRange(originLst.GetRange(0, index));
            lst.Add(obj);
            lst.AddRange(originLst.GetRange(index + 1, originLst.Count - index - 1));
            return new Term(term.Op, lst);
        }

        public static object Generate(this Term term, object obj1, object obj2, object obj)
        {
            var originLst = term.Args as List<object>;
            Debug.Assert(originLst != null);
            if (originLst.Count == 2) return obj;

            int index = -1;
            for (int i = 0; i < originLst.Count; i++)
            {
                if (originLst[i].Equals(obj1))
                {
                    index = i;
                    break;
                }
            }
            Debug.Assert(index != -1);
            var lst = new List<object>();
            lst.AddRange(originLst.GetRange(0, index));
            lst.Add(obj);
            lst.AddRange(originLst.GetRange(index + 2, originLst.Count - index - 2));
            return new Term(term.Op, lst);
        }

    }
}