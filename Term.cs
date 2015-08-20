using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using NUnit.Framework;

namespace CSharpLogic
{
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
            Debug.Assert(lst != null);
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
                    if (variable1 != null) return variable1.Equals(variable);
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
                return !lst.Where((t, i) => !t.Equals(lst1[i])).Any();
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

                builder.Append(lTerm.ToString());

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
                #region List Format
                if (lst.Count == 1)
                {
                    return lst[0].ToString();
                }

                //builder.Append('(');
                int innerVisualElement = 0;
                int index = 0;
                do
                {
                    bool noOp = false;
                    if (lst[index].Equals(1) && Op.Method.Name.Equals("Multiply"))
                    {
                        index++;
                        if (index < lst.Count) continue;
                        else break;
                    }
                    builder.Append(lst[index]);
                    innerVisualElement++;
                    if (Op.Method.Name.Equals("Add"))
                    {
                        #region Add Format
                        if (index + 1 < lst.Count)
                        {
                            bool isNumeric = LogicSharp.IsNumeric(lst[index + 1]);
                            if (isNumeric)
                            {
                                double dd = double.Parse(lst[index + 1].ToString());
                                if (dd < 0.0)
                                {
                                    noOp = true;
                                    builder.Append(lst[index + 1]);
                                    index += 1;
                                    innerVisualElement++;
                                }
                            }
                        }

                        if (!noOp && index != lst.Count - 1)
                        {
                            builder.Append('+');
                        }
                        #endregion
                    }
                    else if (Op.Method.Name.Equals("Multiply"))
                    {
                        if (index != lst.Count - 1)
                        {
                            builder.Append('*');
                        }
                    }
                    else if (Op.Method.Name.Equals("Divide"))
                    {
                        if (index != lst.Count - 1)
                        {
                            builder.Append('/');
                        }
                    }
                    index++;
                } while (index < lst.Count);

                //builder.Append(')');
                if (innerVisualElement > 1)
                {
                    var str = builder.ToString();
                    var bb = new StringBuilder();
                    bb.Append('(').Append(str).Append(')');
                    return bb.ToString();
                }
                #endregion
            }


            return builder.ToString();
        }

        #endregion

        #region Reification

        public object Reify(EqGoal eqGoal)
        {
            Dictionary<object,object> dict = eqGoal.ToDict();
            return Reify(dict);
        }

        public Term Reify(Dictionary<object, object> s)
        {
            var gArgs = LogicSharp.Reify(Args, s);
            return new Term(Op, gArgs);
        }

        #endregion
    }
}
