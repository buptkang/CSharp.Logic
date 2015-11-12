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
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    public partial class Term : DyLogicObject,
                IAlgebraLogic, IEval
    {
        #region Properties and Constructors

        public Func<Expression, Expression, BinaryExpression> Op { get; set; }
        public object Args { get; set; }

        public Term(Func<Expression, Expression, BinaryExpression> _op, object _args)
        {
            Op = _op;
            Args = _args;
        }

        #endregion

        #region Utilities

        public bool ContainsVar()
        {
            var lst = Args as List<object>;
            if (lst == null) return false;
            foreach (var obj in lst)
            {
                var variable = obj as Var;
                if (variable != null) return true;

                var term1 = obj as Term;
                if (term1 != null && term1.ContainsVar()) return true;
            }
            return false;
        }

        public bool ContainsVar(Var variable)
        {
            var tuple1 = Args as Tuple<object>;
            if (tuple1 != null)
            {
                var term1 = tuple1.Item1 as Term;
                if (term1 != null) return term1.ContainsVar(variable);
                var variable1 = tuple1.Item1 as Var;
                if (variable1 != null) return variable1.Equals(variable);
                return false; //constant
            }

            var lst = Args as List<object>;
            if (lst != null)
            {
                foreach (var obj in lst)
                {
                    var term1 = obj as Term;
                    if (term1 != null) return term1.ContainsVar(variable);
                    var variable1 = obj as Var;
                    if (variable1 != null)
                    {
                        bool tempresult = variable1.Equals(variable);
                        if (tempresult) return true;
                    }
                }
                return false;
            }

            var tuple2 = Args as Tuple<object, object>;
            if (tuple2 != null)
            {
                bool result;
                var term1 = tuple2.Item1 as Term;
                if (term1 != null)
                {
                    result = term1.ContainsVar(variable);
                    if (result) return true;
                }
                var variable1 = tuple2.Item1 as Var;
                if (variable1 != null)
                {
                    result = variable1.Equals(variable);
                    if (result) return true;
                }

                var term2 = tuple2.Item2 as Term;
                if (term2 != null)
                {
                    result = term2.ContainsVar(variable);
                    if (result) return true;
                }
                var variable2 = tuple2.Item2 as Var;
                if (variable2 != null)
                {
                    result = variable.Equals(variable2);
                    if (result) return true;
                }
                return false;
            }

            throw new Exception("Term.cs: Cannot reach here");
        }

        public bool ContainsVar(EqGoal goal)
        {
            Debug.Assert(goal != null);
            var variable = goal.Lhs as Var;
            Debug.Assert(variable != null);
            return ContainsVar(variable);
        }

        public Term Clone()
        {
            var term = (Term)this.MemberwiseClone();
            var newlst = new List<object>();

            var lst = Args as List<object>;
            Debug.Assert(lst != null);
            foreach (object obj in lst)
            {
                var variable = obj as Var;
                if (variable != null)
                {
                    newlst.Add(variable.Clone());
                    continue;
                }

                var localTerm = obj as Term;
                if (localTerm != null)
                {
                    newlst.Add(localTerm.Clone());
                    continue;
                }

                newlst.Add(obj);
            }
            term.Args = newlst;
            return term;
        }

        public object ReConstruct()
        {
            var lst = Args as List<object>;
            Debug.Assert(lst != null);
            if (lst.Count == 1) return lst[0];

            for (var i = 0; i < lst.Count; i++)
            {
                var localTerm = lst[i] as Term;
                if (localTerm != null)
                {
                    lst[i] = localTerm.ReConstruct();
                }
            }
            return this;
        }

        #endregion

        #region Override Functions

        public override bool Equals(object obj)
        {
            if (obj is Term)
            {
                var term = obj as Term;
                if (!Op.Equals(term.Op)) return false;

                var lst = Args as List<object>;
                var lst1 = term.Args as List<object>;
                Debug.Assert(lst != null);
                Debug.Assert(lst1 != null);
                if (lst.Count != lst1.Count) return false;

                for (int i = 0; i < lst.Count; i++)
                {
                    var curr1 = lst[i];
                    var curr2 = lst1[i];

                    bool isNum1 = LogicSharp.IsNumeric(curr1);
                    bool isNum2 = LogicSharp.IsNumeric(curr2);
                    bool result;
                    if (isNum1 && isNum2)
                    {
                        result = LogicSharp.NumericEqual(curr1, curr2);
                    }
                    else
                    {
                        result = curr1.Equals(curr2);
                    }
                    if (!result) return false;
                }
                return true;
                //return !lst.Where((t, i) => !t.Equals(lst1[i])).Any();
            }
            return false;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            var tuple = Args as Tuple<object, object>;
            if (tuple != null)
            {
                #region Tuple Format
                builder.Append('(');

                var lTerm = tuple.Item1;
                var rTerm = tuple.Item2;

                builder.Append(lTerm);

                if (Op.Method.Name.Equals("Add"))
                {
                    builder.Append('+');
                }
                else if (Op.Method.Name.Equals("Substract"))
                {
                    builder.Append('-');
                }
                else if (Op.Method.Name.Equals("Multiply"))
                {
                    builder.Append('*');
                }
                else if (Op.Method.Name.Equals("Divide"))
                {
                    builder.Append('/');
                }

                builder.Append(rTerm.ToString());
                builder.Append(')');
                #endregion
            }

            var lst = Args as List<object>;
            if (lst != null)
            {
                for (int i = 0; i < lst.Count; i++)
                {
                    var variable = lst[i] as Var;
                    bool number = LogicSharp.IsNumeric(lst[i]);
                    var localTerm = lst[i] as Term;

                    #region Var
                    if (variable != null)
                    {
                        if (Op.Method.Name.Equals("Add"))
                        {
                            if (i != 0)
                            {
                                builder.Append("+");
                            }
                        }

                        if (Op.Method.Name.Equals("Power"))
                        {
                            if (i != 0)
                            {
                                builder.Append("^");
                            }
                        }
                        builder.Append(variable);
                    }
                    #endregion

                    #region Numerics
                    if (number)
                    {
                        if (Op.Method.Name.Equals("Add"))
                        {
                            if (i != 0)
                            {
                                double dnum;
                                bool result = LogicSharp.IsDouble(lst[i], out dnum);
                                if (dnum < 0.0)
                                {
                                    builder.Append("-");
                                    double absNum = Math.Abs(dnum);
                                    builder.Append(absNum);
                                }
                                else
                                {
                                    builder.Append("+");
                                    builder.Append(lst[i].ToString());
                                }
                            }
                            else
                            {
                                builder.Append(lst[i].ToString());
                            }
                        }
                        else if (Op.Method.Name.Equals("Multiply"))
                        {
                            double dnum;
                            LogicSharp.IsDouble(lst[i], out dnum);
                            double absNum = Math.Abs(dnum) - 1.0;
                            if (absNum > 0.0001)
                            {
                                builder.Append(lst[i]);
                            }
                            else
                            {
                                if (dnum < 0.0d)
                                {
                                    builder.Append("-");
                                }
                            }
                        }
                        else if (Op.Method.Name.Equals("Divide"))
                        {
                            if (i != 0)
                            {
                                builder.Append("/");
                                builder.Append(lst[i].ToString());
                            }
                            else
                            {
                                builder.Append(lst[i].ToString());
                            }

                        }
                        else if (Op.Method.Name.Equals("Power"))
                        {
                            if (i != 0)
                            {
                                builder.Append("^");
                            }
                            builder.Append(lst[i].ToString());
                        }
                    }
                    #endregion

                    #region Term
                    if (localTerm != null)
                    {
                        //precatch
                        if (Op.Method.Name.Equals("Add"))
                        {
                            if (i != 0)
                            {
                                bool result = localTerm.InvertOp();
                                if (!result)
                                {
                                    builder.Append("+");
                                }
                                else
                                {
                                    builder.Append("-");
                                }
                            }
                            builder.Append(localTerm.ToString());
                        }

                        if (Op.Method.Name.Equals("Multiply") ||
                            Op.Method.Name.Equals("Power")
                            )
                        {
                            bool needBrace = false;
                            needBrace = localTerm.NeedBracket();
                            if (needBrace)
                            {
                                builder.Append("(");
                                builder.Append(localTerm.ToString());
                                builder.Append(")");
                            }
                            else
                            {
                                builder.Append(localTerm.ToString());
                            }
                        }

                        if (Op.Method.Name.Equals("Divide"))
                        {
                            if (i != 0)
                            {
                                builder.Append("/");
                            }
                            bool needBrace = false;
                            needBrace = localTerm.NeedBracket();
                            if (needBrace)
                            {
                                builder.Append("(");
                                builder.Append(localTerm.ToString());
                                builder.Append(")");
                            }
                            else
                            {
                                builder.Append(localTerm.ToString());
                            }
                        }
                    }
                    #endregion
                }
            }

            return builder.ToString();
        }

        private bool NeedBracket()
        {
            var lst = Args as List<object>;

            if (lst == null) return false;

            if (lst.Count > 1) return true;
            return false;
        }

        private bool InvertOp()
        {
            var lst = Args as List<object>;

            if (lst == null) return false;
            double dnum;
            bool result = LogicSharp.IsDouble(lst[0], out dnum);
            if (!result) return false;
            if (dnum < 0.0) return true;
            return false;
        }



        #endregion
    }
}
