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
using System.Linq;
using System.Linq.Expressions;
using System.Xml;
using NUnit.Framework;

namespace CSharpLogic
{
    public static class TermExtension
    {
        #region Quadratic Term Pattern Match

        public static bool IsQuadraticTerm(this Term term)
        {
            if (term == null) return false;
            if (!term.ContainsVar()) return false;
            var cond1 = term.Op == Expression.Power;
            var lst = term.Args as List<object>;
            if (lst == null || lst.Count != 2) return false;
            var isNum = LogicSharp.IsNumeric(lst[1]);
            if (!isNum) return false;
            double number;
            LogicSharp.IsDouble(lst[1], out number);
            var cond2 = number.Equals(2.0);
            return cond1 && cond2;
        }

        public static bool QuadraticTerm(this Term term)
        {
            if (term.IsQuadraticTerm()) return true;
            if (term == null) return false;
            var objs = term.Args as List<object>;
            if (objs == null) return false;
            return objs.OfType<Term>().Any(tt => tt.QuadraticTerm());
        }

        public static bool MatchQuadraticTerm(this Term currTerm, Term matchTerm)
        {
            if (currTerm == null || matchTerm == null) return false;
            if (!currTerm.Op.Method.Name.Equals(matchTerm.Op.Method.Name)) return false;

            var lst1 = currTerm.Args as List<object>;
            var lst2 = matchTerm.Args as List<object>;

            if (lst1.Count != lst2.Count) return false;
            if (!lst1[1].Equals(lst2[1])) return false;

            var var1 = lst1[0] as Var;
            var var2 = lst2[0] as Var;
            if (var1 != null && var2 != null)
            {
                if (var1.ToString().Equals(var2.ToString())) return true;
            }

            var t1 = lst1[0] as Term;
            var t2 = lst2[0] as Term;
            if (t1 != null && t2 != null && t1.Equals(t2)) return true;

            return false;
        }

        #endregion

        #region Flattern Term

        public static Term FlatTerm(this Term currTerm)
        {
            if (!currTerm.Op.Method.Name.Equals("Add")) return currTerm;
            var lst = currTerm.Args as List<object>;
            var newLst = new List<object>();
            foreach (object obj in lst)
            {
                var term = obj as Term;
                if (term != null && term.Op.Method.Name.Equals("Add"))
                {
                    var ll = term.Args as List<object>;
                    if (ll != null) newLst.AddRange(ll);
                }
                else
                {
                    newLst.Add(obj);
                }
            }
            return new Term(Expression.Add, newLst);
        }

        #endregion

    }
}
