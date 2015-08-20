using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CSharpLogic
{
    public static class ArithmeticEvalExtension
    {
        /// <summary>
        //with calculation functionality, it does not 
        /// take care of term rewriting.
        /// 
        /// Terms should follow the below rules:
        /// tp: "2+2",  "2+3-1", "2+2*2"
        /// </summary>
        /// <returns> can be term or value</returns>
        /// 
        public static object Arithmetic(this object obj, Term rootTerm)
        {
            var term = obj as Term;
            if (term == null) return obj;

            Term localTerm = term.DepthFirstSearch(rootTerm);
            var list = localTerm.Args as List<object>;

            //List<object> objs = localTerm.FindArithValues();
            if (list == null || list.Count < 2) return localTerm;
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

                    if (SatisfyCalcCondition(term.Op, list[i], list[i + 1], out obj1))
                    {
                        var cloneTerm = localTerm.Clone();
                        var cloneLst = cloneTerm.Args as List<object>;
                        Debug.Assert(cloneLst != null);
                        cloneLst[i] = obj1;
                        cloneLst.RemoveAt(i + 1);

                        string rule = ArithRule.CalcRule(term.Op.Method.Name);
                        string appliedrule = ArithRule.CalcRule(term.Op.Method.Name, list[i], list[i + 1], obj1);

                        rootTerm.GenerateTrace(localTerm, cloneTerm, rule, appliedrule);
                        localTerm = cloneTerm;
                        madeChanges = true;
                    }
                }
            } while (madeChanges);

/*
            if ()
            {
                var objCalc = Calculate(term.Op, objs[0], objs[1]);
                
                rootTerm.GenerateTrace(objs[0], objs[1], objCalc, rule);
                return localTerm.Substitute(objs[0], objs[1], objCalc);
            }
*/           
            //return localTerm;

            var lstArgs = localTerm.Args as List<object>;
            if (lstArgs.Count == 1) return lstArgs[0];
            return localTerm;
        }

        private static bool SatisfyCalcCondition(
                              Func<Expression, Expression, BinaryExpression> func,
                              object x, object y, out object output)
        {
            output = null;
            if (LogicSharp.IsNumeric(x) && LogicSharp.IsNumeric(y))
            {
                double xDoubleVal;
                double yDoubleVal;
                bool isXDouble = LogicSharp.IsDouble(x, out xDoubleVal);
                bool isYDouble = LogicSharp.IsDouble(y, out yDoubleVal);
                if (isXDouble || isYDouble)
                {
                    var xExpr = Expression.Constant(xDoubleVal);
                    var yExpr = Expression.Constant(yDoubleVal);
                    var rExpr = func(xExpr, yExpr);
                    output = Expression.Lambda<Func<double>>(rExpr).Compile().Invoke();
                    int iResult;
                    if (LogicSharp.IsInt(output, out iResult))
                    {
                        output = iResult;
                        return true;
                    }
                    return true;
                }                
            }
            return false;
        }
    }
}
