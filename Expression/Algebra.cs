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

using System.Data.SqlClient;

namespace CSharpLogic
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
   
    public interface IAlgebraLogic
    {
        /// <summary>
        // 1.1 Distributive law a(b+c) <=> ab+ac
        // 1.2 Associative  law 
        // 1.3 Commutative  law
        // 1.4 Identity     law x*1 =>1
        // 1.5 zero         law x * 0 => 0, 0 * x => 0, x + 0 => x
        /// </summary>
        /// <returns>value of term</returns>
        object EvalAlgebra();
    }

    public static class AlgebraEvalExtension
    {
        #region Commutative Law

        /// <summary>
        /// Commutative Law 
        /// Algorithm: Bubble Sort
        /// </summary>
        /// <param name="term"></param>
        /// <param name="obj"></param>
        /// <param name="rootTerm"></param>
        public static object ApplyCommutative(this object obj, Term rootTerm)
        {
            var term = obj as Term;
            if (term == null) return obj;

            //3+x -> x+3
            //x*3 -> 3*x
            //x+a -> x+a
            //x*a -> x*a
            //3*a*3 -> a*3*3
            //(x+1)*a -> a*(x+1)
            //1+(1+a) -> (a+1)+1

            Term localTerm = term.DepthFirstSearch(rootTerm);

            #region Apply Commutative Law: Bubble Sort
            bool madeChanges;
            do
            {
                var list = localTerm.Args as List<object>;
                if (list == null) throw new Exception("Cannot be null");
                int itemCount = list.Count;
                madeChanges = false;
                itemCount--;
                for (var i = 0; i < itemCount; i++)
                {
                    list = localTerm.Args as List<object>;
                    Debug.Assert(list != null);
                    if (SatisfyCommutativeCondition(localTerm, list[i], list[i + 1]))
                    {
                        var cloneTerm = localTerm.Clone();
                        var cloneLst = cloneTerm.Args as List<object>;
                        Debug.Assert(cloneLst != null);
                        object tempObj = cloneLst[i];
                        cloneLst[i] = cloneLst[i + 1];
                        cloneLst[i + 1] = tempObj;
                        madeChanges = true;

                        //generate trace rule
                        string kc = AlgebraRule.RuleConcept(AlgebraRule.AlgebraRuleType.Commutative);

                        string rule = AlgebraRule.Rule(AlgebraRule.AlgebraRuleType.Commutative);
                        string appliedRule = AlgebraRule.Rule(
                            AlgebraRule.AlgebraRuleType.Commutative,
                            list[i], list[i + 1]);

                        rootTerm.GenerateTrace(localTerm, cloneTerm, kc, rule, appliedRule);
                        localTerm = cloneTerm;
                    }
                }
            } while (madeChanges);

            #endregion

            return localTerm;
        }

        //commutative law
        private static bool SatisfyCommutativeCondition(Term inTerm, object obj1, object obj2)
        {
            if (inTerm.Op.Method.Name.Equals("Add"))
            {
                //e.g 3+x => x+3 
                if (LogicSharp.IsNumeric(obj1))
                {
                    var variable = obj2 as Var;
                    if (variable != null) return true;
                    var term = obj2 as Term;
                    if (term != null && term.ContainsVar()) return true;
                }

                //e.g (1+1)+(x+1)=>(x+1)+(1+1)
                var term1 = obj1 as Term;
                if (term1 != null && !term1.ContainsVar())
                {
                    var variable = obj2 as Var;
                    if (variable != null) return true;

                    var term = obj2 as Term;
                    if (term != null && term.ContainsVar()) return true;
                }

                var variable11 = obj1 as Var;
                var term2 = obj2 as Term;

                if (variable11 != null && term2.QuadraticTerm()) return true;

                //(2*y)+(2*x) -> (2*x)+(2*y)
                // x + x^2    -> x^2 + x
                if (term1 != null && term2 != null)
                {
                    var term1Lst = term1.Args as List<object>;
                    Debug.Assert(term1Lst != null);
                    var term1Var = term1Lst[1] as Var;

                    var term2Lst = term2.Args as List<object>;
                    Debug.Assert(term2Lst != null);
                    var term2Var = term2Lst[1] as Var;
                    if (term1Var != null && term2Var != null)
                    {
                        bool condition1 = term1Var.ToString().Equals("Y") ||
                                          term1Var.ToString().Equals("y");
                        bool condition2 = term2Var.ToString().Equals("X") ||
                                          term2Var.ToString().Equals("x");
                        if (condition1 && condition2) return true;
                    }
                    if (!term1.QuadraticTerm() && term2.QuadraticTerm())
                    {
                        return true;
                    }
                }
            }
            else if (inTerm.Op.Method.Name.Equals("Multiply"))
            {
                var term1 = obj1 as Term;
                var term2 = obj2 as Term;

                /*                if (term1 != null && term2 == null)
                                {
                                    return true;
                                }*/

                //e.g x*3 -> 3*x
                //e.g 3*x*3 -> x*3*3
                if (LogicSharp.IsNumeric(obj2))
                {
                    var variable = obj1 as Var;
                    if (variable != null) return true;
                    var term = obj1 as Term;
                    if (term != null && term.ContainsVar()) return true;
                }
            }
            return false;
        }

        #endregion

        #region Identity Law

        /// <summary>
        /// true positive: y*1 -> y, y/1 -> y
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="rootTerm"></param>
        /// <returns></returns>
        public static object ApplyIdentity(this object obj, Term rootTerm)
        {
            //x     ->1*x
            //x+3   ->1*x+3
            //x+x   ->1*x+1*x
            //y+2*y ->1*y+2*y
            //x*y   ->1*x*y
            var variable = obj as Var;
            if (variable != null) return new Term(Expression.Multiply, new List<object>() { 1, variable });

            var term = obj as Term;
            if (term == null) return obj;

            Term localTerm = term.DepthFirstSearch(rootTerm);

            #region Apply Identity Law
            var list = localTerm.Args as List<object>;
            Debug.Assert(list != null);

            var lst = localTerm.Args as List<object>;
            Debug.Assert(lst != null);
            object gobj;
            for (int i = 0; i < lst.Count; i++)
            {
                if (SatisfyIdentityCondition1(localTerm, lst[i], out gobj))
                {
                    var cloneTerm = localTerm.Clone();
                    var cloneLst = cloneTerm.Args as List<object>;
                    Debug.Assert(cloneLst != null);

                    cloneLst[i] = gobj;
                    //generate trace rule
                    /*
                    string rule = AlgebraRule.Rule(
                        AlgebraRule.AlgebraRuleType.Identity,
                        list[i], null);

                    rootTerm.GenerateTrace(localTerm, cloneTerm, rule);
                     */ 
                    localTerm = cloneTerm;
                }                
            }

            if (lst.Count >= 2)
            {
                if (SatisfyIdentityCondition2(localTerm, lst[0], lst[1], out gobj))
                {
                    var cloneTerm = localTerm.Clone();
                    var cloneLst = cloneTerm.Args as List<object>;
                    Debug.Assert(cloneLst != null);
                    cloneLst[0] = gobj;
                    cloneLst.Remove(lst[1]);
                    //generate trace rule
                    /*
                    string rule = AlgebraRule.Rule(
                        AlgebraRule.AlgebraRuleType.Identity,
                        list[0], null);

                    rootTerm.GenerateTrace(localTerm, cloneTerm, rule);
                     */ 
                    localTerm = cloneTerm;
                }
            }

            #endregion
            return localTerm;           
        }

        private static bool SatisfyIdentityCondition2(Term inTerm, object obj1, object obj2,
            out object output)
        {
            output = null;
            if (inTerm.Op.Method.Name.Equals("Multiply"))
            {
                if (obj1.Equals(1) && NotSpecialVariables(obj2))
                {
                    output = obj2;
                    return true;
                }

                if (obj2.Equals(1) && NotSpecialVariables(obj1))
                {
                    output = obj1;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// x -> 1*x
        /// </summary>
        /// <param name="inTerm"></param>
        /// <param name="obj"></param>
        /// <param name="obj1"></param>
        /// <returns></returns>
        private static bool SatisfyIdentityCondition1(Term inTerm, object obj, out object obj1)
        {
            obj1 = null;
            if (inTerm.Op.Method.Name.Equals("Add"))
            {
                var variable = obj as Var;
                if (variable != null)
                {
                    if (variable.ToString().Equals("X") ||
                        variable.ToString().Equals("x") ||
                        variable.ToString().Equals("Y") ||
                        variable.ToString().Equals("y"))
                    {
                        obj1 = new Term(Expression.Multiply, new List<object>() { 1, variable });
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

        #region Zero Law

        /// <summary>
        /// x+0 = x, x-0 = x, x*0=0
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="rootTerm"></param>
        /// <returns></returns>
        public static object ApplyZero(this object obj, Term rootTerm)
        {
            var term = obj as Term;
            if (term == null) return obj;

            Term localTerm = term.DepthFirstSearch(rootTerm);

            #region Apply Zero Law

            var list = localTerm.Args as List<object>;
            if (ContainZero(list))
            {
                if (localTerm.Op.Method.Name.Equals("Multiply"))
                {
                    string kc = AlgebraRule.RuleConcept(AlgebraRule.AlgebraRuleType.Identity);

                    string rule = AlgebraRule.Rule(
                        AlgebraRule.AlgebraRuleType.Identity);
                    String appliedRule = AlgebraRule.Rule(
                           AlgebraRule.AlgebraRuleType.Identity,
                           localTerm, null);

                    rootTerm.GenerateTrace(localTerm, 0, kc, rule, appliedRule);
                    return 0;
                }
                if (localTerm.Op.Method.Name.Equals("Add"))
                {
                    string kc = AlgebraRule.RuleConcept(AlgebraRule.AlgebraRuleType.Identity);

                    var cloneTerm = localTerm.Clone();
                    var cloneLst = cloneTerm.Args as List<object>;
                    Debug.Assert(cloneLst != null);
                    RemoveZero(ref cloneLst);
                    //generate trace rule
                    string rule = AlgebraRule.Rule(
                        AlgebraRule.AlgebraRuleType.Identity);
                    string appliedRule = AlgebraRule.Rule(
                        AlgebraRule.AlgebraRuleType.Identity,
                        localTerm, null);

                    rootTerm.GenerateTrace(localTerm, cloneTerm, kc, rule, appliedRule);
                    localTerm = cloneTerm;
                }
            }
            #endregion

            return localTerm;
        }

        private static void RemoveZero(ref List<object> lst)
        {
            for (var i = 0; i < lst.Count; i++)
            {
                if (lst[i].Equals(0.0) || lst[i].Equals(0))
                {
                    lst.RemoveAt(i);
                }
            }
        }

        private static bool ContainZero(IEnumerable<object> lst)
        {
            return lst.Where(LogicSharp.IsNumeric).Any(obj => 0.0.Equals(obj) || 0.Equals(obj));
        }

        #endregion

        #region Distribute Law

        public static object ApplyDistributive(this object obj, Term rootTerm)
        {
            //1*y+1*y         -> (1+1)*y
            //1*y+2*y         -> (1+2)*y  
            //y+y+y           -> (1+1+1)*y
            //a*y+y+x           -> (a+1)*y+x

            //1*y^2+2*y^2     -> (1+2)*y^2
            //x^2+x^2+x^2     -> (1+1+1)*x^2
            //2*x^2+x^2+y     -> (2+1)x^2+y

            //3*(x+1)         -> 3x+3
            //a*(x+1)         -> ax+a
            //x*(a+1)         -> xa + x 
            //(a+1)*(x+1)     -> TODO

            var term = obj as Term;
            if (term == null) return obj;

            Term localTerm = term.DepthFirstSearch(rootTerm);

            var list = localTerm.Args as List<object>;
            if (list == null) throw new Exception("Cannot be null");
            if (list.Count < 2) return localTerm;

            bool madeChanges;
            do
            {
                list = localTerm.Args as List<object>;
                if (list == null) throw new Exception("Cannot be null");
                int itemCount = list.Count;
                madeChanges = false;
                itemCount--;
                for (var i = 0; i < itemCount; i++)
                {
                    list = localTerm.Args as List<object>;
                    Debug.Assert(list != null);
                    itemCount = list.Count;
                    object obj1;
                    if (i + 1 >= list.Count) break;
                    if (SatisfyDistributiveCondition(term, list[i], list[i+1], out obj1))
                    {
                        var cloneTerm = localTerm.Clone();
                        var cloneLst = cloneTerm.Args as List<object>;
                        Debug.Assert(cloneLst != null);
                        cloneLst[i] = obj1;
                        cloneLst.RemoveAt(i+1);

                        string kc = AlgebraRule.RuleConcept(AlgebraRule.AlgebraRuleType.Distributive);

                        //generate trace rule
                        string rule = AlgebraRule.Rule(
                            AlgebraRule.AlgebraRuleType.Distributive);
                        string appliedRule = AlgebraRule.Rule(
                            AlgebraRule.AlgebraRuleType.Distributive,
                            list[i], list[i+1]);

                        if (cloneLst.Count == 1)
                        {
                            cloneTerm = cloneLst[0] as Term;
                        }

                        rootTerm.GenerateTrace(localTerm, cloneTerm, kc, rule, appliedRule);
                        localTerm = cloneTerm;
                        madeChanges = true;
                    }
                }
            } while (madeChanges);
            return localTerm;
        }

        private static bool SatisfyDistributiveCondition(Term inTerm,
            object obj1, object obj2, out object outputObj)
        {
            outputObj = null;
            if (inTerm.Op.Method.Name.Equals("Add"))
            {
                var term1 = obj1 as Term;
                var term2 = obj2 as Term;

                if (term1 != null && term2 != null)
                {
                    var lst1 = term1.Args as List<object>;
                    var lst2 = term2.Args as List<object>;
                    Debug.Assert(lst1 != null);
                    Debug.Assert(lst2 != null);

                    #region Quadratic Term

                    if (term1.QuadraticTerm() && term2.QuadraticTerm())
                    {
                        if (lst1.Count != 2 || lst2.Count != 2) return false;
                        var quadratic1 = lst1[1] as Term;
                        var quadratic2 = lst2[1] as Term;

                        bool cond1 = term1.IsQuadraticTerm();
                        bool cond2 = term2.IsQuadraticTerm();

                        bool quadrCond1 = cond1 && cond2;
                        bool quadrCond2 = cond1 && !cond2 && (quadratic2 != null);
                        bool quadrCond3 = !cond1 && (quadratic1 != null) && cond2;
                        bool quadrCond4 = !cond1 && (quadratic1 != null) && !cond2 && (quadratic2 != null);

                        if (quadrCond1)
                        {
                            if (!term1.MatchQuadraticTerm(term2)) return false;
                            var newList1 = new List<object>() { 1, 1 };
                            var gTerm1 = new Term(Expression.Add, newList1);
                            outputObj = new Term(Expression.Multiply, new List<object>() { gTerm1, term1 });
                            return true;
                        }
                        if (quadrCond2)
                        {
                            if (!term1.MatchQuadraticTerm(quadratic2)) return false;
                            var newList2 = new List<object>() { 1, lst2[0] };
                            var gTerm2 = new Term(Expression.Add, newList2);
                            outputObj = new Term(Expression.Multiply, new List<object>() { gTerm2, term1 });
                            return true;
                        }
                        if (quadrCond3)
                        {
                            if (!quadratic1.MatchQuadraticTerm(term2)) return false;
                            var newList3 = new List<object>() {lst1[0], 1};
                            var gTerm3 = new Term(Expression.Add, newList3);
                            outputObj = new Term(Expression.Multiply, new List<object>() { gTerm3, term2 });
                            return true;
                        }
                        if (quadrCond4)
                        {
                            if (!quadratic1.MatchQuadraticTerm(quadratic2)) return false;
                            var newList4 = new List<object>() {lst1[0], lst2[0]};
                            var gTerm4 = new Term(Expression.Add, newList4);
                            outputObj = new Term(Expression.Multiply, new List<object>() { gTerm4, quadratic2 });
                            return true;
                        }
                    }

                    #endregion

                    //Linear
                    if (term1.Op.Method.Name.Equals("Power") ||
                        term2.Op.Method.Name.Equals("Power"))
                    {
                        return false;
                    }

                    if (lst1.Count == 2 && lst2.Count == 2 && lst1[1].Equals(lst2[1]))
                    {
                        if (term1.Op.Method.Name.Equals("Multiply") &&
                            term2.Op.Method.Name.Equals("Multiply"))
                        {
                            var newList = new List<object>() { lst1[0], lst2[0] };
                            var gTerm = new Term(Expression.Add, newList);
                            outputObj = new Term(Expression.Multiply, new List<object>() { gTerm, lst1[1] });
                            return true;
                        }
                    }
                } 
            }
            else if (inTerm.Op.Method.Name.Equals("Multiply"))
            {
                var term1 = obj1 as Term;
                var term2 = obj2 as Term;
                if (term1 != null && term2 != null) return false; //TODO

                if (term1 == null && term2 != null)
                {
                    if (!term2.Op.Method.Name.Equals("Add")) return false;
                    var lst2 = term2.Args as List<object>;
                    Debug.Assert(lst2 != null);
                    var newList = lst2.Select(obj => new Term(Expression.Multiply, new List<object>() { obj1, obj })).Cast<object>().ToList();
                    outputObj = new Term(Expression.Add, newList);
                    return true;
                }
            }

            return SatisfyDistributiveCondition2(inTerm, obj1, obj2, out outputObj);
        }


        private static bool SatisfyDistributiveCondition2(Term inTerm,
            object obj1, object obj2, out object outputObj)
        {
            // x/3+9 -> (x+27)/3
            //(2*x)/3+2 -> (2*x+6)/3

            outputObj = null;

            if (inTerm.Op.Method.Name.Equals("Add"))
            {
                var term1 = obj1 as Term;                
                if (term1 != null && term1.Op.Method.Name.Equals("Divide"))
                {
                    var lst = term1.Args as List<object>;
                    if (lst == null) return false;
                    var numerator   = lst[0];
                    var denominator = lst[1];

                    var gTerm = new Term(Expression.Multiply, new List<object> {denominator, obj2});
                    var newNumerator = new Term(Expression.Add, new List<object> {numerator, gTerm});
                    outputObj = new Term(Expression.Divide, new List<object> {newNumerator, denominator});
                    return true;
                }
                var term2 = obj2 as Term;
                if (term2 != null && term2.Op.Method.Name.Equals("Divide"))
                {
                    var lst = term2.Args as List<object>;
                    if (lst == null) return false;
                    var numerator = lst[0];
                    var denominator = lst[1];
                    var gTerm = new Term(Expression.Multiply, new List<object> { obj1, denominator});
                    var newNumerator = new Term(Expression.Add, new List<object> { gTerm, numerator});
                    outputObj = new Term(Expression.Divide, new List<object> { newNumerator, denominator });
                    return true;
                }
            }
            return false;            
        }

        #endregion

        #region Associative Law

        /// <summary>
        /// x+(y+z) == (x+y)+zm, x(yz) == (xy)z
        /// </summary>
        /// <param name="term"></param>
        /// <param name="gTerm"></param>
        /// <returns></returns>
        public static object ApplyAssociative(this object obj, Term rootTerm)
        {
            //(a+1)+1 -> a+(1+1)
            //a*3*3 -> a*(3*3)
            var term = obj as Term;
            if (term == null) return obj;

            Term localTerm = term.DepthFirstSearch(rootTerm);
            var list = localTerm.Args as List<object>;
            if (list == null) throw new Exception("Cannot be null");
            if (list.Count < 2) return localTerm;

            object obj1, obj2;
            if (SatisfyAssociativeCondition(term, list[0], list[1], out obj1, out obj2))
            {
                var cloneTerm = localTerm.Clone();
                var cloneLst = cloneTerm.Args as List<object>;
                Debug.Assert(cloneLst != null);
                cloneLst[0] = obj1;
                cloneLst[1] = obj2;

                //generate trace rule

                string kc = AlgebraRule.RuleConcept(AlgebraRule.AlgebraRuleType.Associative);

                string rule = AlgebraRule.Rule(
                    AlgebraRule.AlgebraRuleType.Associative);
                    
                String appliedRule = AlgebraRule.Rule(
                    AlgebraRule.AlgebraRuleType.Associative,
                    list[0], list[1]);

                rootTerm.GenerateTrace(localTerm, cloneTerm, kc, rule, appliedRule);
                localTerm = cloneTerm;
            }

            Term gTerm;
            if (SatisfyAssociativeCondition2(localTerm, out gTerm))
            {

                //generate trace rule

                string kc = AlgebraRule.RuleConcept(AlgebraRule.AlgebraRuleType.Associative);

                string rule = AlgebraRule.Rule(
                    AlgebraRule.AlgebraRuleType.Associative);

                String appliedRule = AlgebraRule.Rule(
                    AlgebraRule.AlgebraRuleType.Associative,
                    list[0], list[1]);

                rootTerm.GenerateTrace(localTerm, gTerm, kc, rule, appliedRule);
                localTerm = gTerm;
            }

            return localTerm;
        }

        private static bool SatisfyAssociativeCondition(Term inTerm,
            object obj1, object obj2, out object output1, out object output2)
        {
            output1 = null;
            output2 = null;
            if (inTerm.Op.Method.Name.Equals("Add"))
            {
                var term1 = obj1 as Term;
                var term2 = obj2 as Term;

                if (term1 != null && term2 != null)
                {
                    if (term1.Op.Method.Name.Equals("Add") && 
                        term2.Op.Method.Name.Equals("Add"))
                    {
                        var term1Args = term1.Args as List<object>;
                        var term2Args = term2.Args as List<object>;

                        if (term1Args != null && term2Args != null)
                        {
                            var term1Arg2 = term1Args[1];
                            var term2Arg2 = term2Args[1];
                            if (LogicSharp.IsNumeric(term1Arg2) &&
                                LogicSharp.IsNumeric(term2Arg2))
                            {
                                output1 = new Term(Expression.Add, new List<object> {term1Args[0], term2Args[0]});
                                output2 = new Term(Expression.Add, new List<object> {term1Arg2, term2Arg2});
                                return true;
                            }                            
                        }
                    }

/*                    if (term1.Op.Method.Name.Equals("Add") && term2.Op.Method.Name.Equals("Multiply"))
                    {
                        var lst = term1.Args as List<object>;
                        object obj = lst[lst.Count-1];
                        if (LogicSharp.IsNumeric(obj) || NotSpecialVariables(obj))
                        {
                            var newLst = new List<object>();
                            for (var i = 0; i < lst.Count - 1; i++)
                            {
                                newLst.Add(lst[i]);
                            }
                            output1 = newLst.Count == 1 ? newLst[0] : new Term(Expression.Add, newLst);
                            output2 = new Term(Expression.Add, new List<object>() { obj, obj2 });
                            return true;
                        }
                    }*/
                    if (term1.Op.Method.Name.Equals("Multiply") && term2.Op.Method.Name.Equals("Add"))
                    {
                        var lst = term2.Args as List<object>;
                        object obj = lst[lst.Count - 1];
                        if (LogicSharp.IsNumeric(obj) || NotSpecialVariables(obj))
                        {
                            var newLst = new List<object>();
                            for (var i = 0; i < lst.Count - 1; i++)
                            {
                                newLst.Add(lst[i]);
                            }
                            newLst.Insert(0,obj1);
                            output1 = new Term(Expression.Add, newLst);
                            output2 = obj;
                            return true;
                        }
                    }
                }

                if (term1 != null && term2 == null)
                {
                    if (!term1.Op.Method.Name.Equals("Add")) return false;
                    var lst = term1.Args as List<object>;
                    Debug.Assert(lst != null);
                    object obj = lst[lst.Count-1];
                    if (LogicSharp.IsNumeric(obj) || NotSpecialVariables(obj))
                    {
                        var newLst = new List<object>();
                        for (var i = 0; i < lst.Count-1; i++)
                        {
                            newLst.Add(lst[i]);
                        }
                        output1 = newLst.Count == 1 ? newLst[0] : new Term(Expression.Add, newLst);
                        output2 = new Term(Expression.Add, new List<object>() { obj, obj2 });
                        return true;
                    }
                    return false;
                }
            }
            else if (inTerm.Op.Method.Name.Equals("Multiply"))
            {
                var term1 = obj1 as Term;
                var term2 = obj2 as Term;
                if (term1 == null && term2 != null)
                {
                    if (term2.Op.Method.Name.Equals("Multiply"))
                    {
                        var lst = term2.Args as List<object>;
                        Debug.Assert(lst != null);
                        object obj = lst[0];
                        if (LogicSharp.IsNumeric(obj) || NotSpecialVariables(obj))
                        {
                            output1 = new Term(Expression.Multiply, new List<object>() { obj1, obj });
                            var newLst = new List<object>();
                            for (var i = 1; i < lst.Count; i++)
                            {
                                newLst.Add(lst[i]);
                            }
                            output2 = newLst.Count == 1 ? newLst[0] : new Term(Expression.Multiply, newLst);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private static bool SatisfyAssociativeCondition2(Term inTerm, out Term gTerm)
        {
            gTerm = null;
            if (!inTerm.Op.Method.Name.Equals("Multiply")) return false;
            var lst = inTerm.Args as List<object>;

            var term1 = lst[0] as Term;
            var term2 = lst[1] as Term;

            if (term1 != null && term1.Op.Method.Name.Equals("Divide"))
            {
                var arg1 = term1.Args as List<object>;
                var numerator = new Term(Expression.Multiply, new List<object> { arg1[0], lst[1] });
                var denominator = arg1[1];
                gTerm = new Term(Expression.Divide, new List<object> {numerator, denominator});
                return true;
            }

            if (term2 != null && term2.Op.Method.Name.Equals("Divide"))
            {
                var arg2 = term2.Args as List<object>;
                var numerator = new Term(Expression.Multiply, new List<object> {lst[0], arg2[0]});
                var denominator = arg2[1];
                gTerm = new Term(Expression.Divide, new List<object> {numerator, denominator});
                return true;
            }

            return false;
        }


        private static bool NotSpecialVariables(object obj)
        {
            var variable = obj as Var;
            if (variable != null)
            {
                if (variable.ToString().Equals("X") ||
                    variable.ToString().Equals("x") ||
                    variable.ToString().Equals("Y") ||
                    variable.ToString().Equals("y"))
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        #endregion
    }
}