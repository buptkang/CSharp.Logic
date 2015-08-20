using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace CSharpLogic
{
    public partial class LogicSharp
    {
        public static Dictionary<TKey, TValue> CloneDictionaryCloningValues<TKey, TValue>
        (Dictionary<TKey, TValue> original)
        {
            var ret = new Dictionary<TKey, TValue>(original.Count,
                                                                    original.Comparer);
            foreach (KeyValuePair<TKey, TValue> entry in original)
            {
                ret.Add(entry.Key, (TValue)entry.Value);
            }
            return ret;
        }

        public static IEnumerable<KeyValuePair<object, object>> DiffTwoDictionary(Dictionary<object, object> dicOne,
            Dictionary<object, object> dicTwo)
        {
            return dicOne.Except(dicTwo).Concat(dicTwo.Except(dicOne));
        }

        public static IEnumerable<KeyValuePair<object, object>> Interleave(List<Goal> objs)
        {
            return null;
        }

        public static object transitive_get(object key, Dictionary<object, object> d)
        {
            if (d == null || d.Count == 0) return key;
            while (d.ContainsKey(key))
            {
                key = d[key];
            }
            return (object)key;
        }

        public static object deep_transitive_get(object key, Dictionary<object, object> d)
        {
            object key1 = transitive_get(key, d);
            if (key1 is Tuple<object>)
            {
                var mKey = key1 as Tuple<object>;
                return Tuple.Create(deep_transitive_get(mKey.Item1, d) as object);
            }
            else if (key1 is Tuple<object, object>)
            {
                var mKey = key1 as Tuple<object, object>;
                return Tuple.Create(deep_transitive_get(mKey.Item1, d),
                             deep_transitive_get(mKey.Item2, d)
                            );
            }
            else if (key1 is Tuple<object, object, object>)
            {
                var mKey = key1 as Tuple<object, object, object>;
                return Tuple.Create(deep_transitive_get(mKey.Item1, d),
                             deep_transitive_get(mKey.Item2, d),
                             deep_transitive_get(mKey.Item3, d)
                            );
            }
            return key1;
        }

        public static bool equal_test(object obj1, object obj2)
        {
            if (obj1 is List<object> && obj2 is List<object>)
            {
                var lst1 = obj1 as List<object>;
                var lst2 = obj2 as List<object>;
                if (lst1.Count != lst2.Count) return false;

                for (var i = 0; i < lst1.Count; i++)
                {
                    bool result = lst1[i].Equals(lst2[i]);
                    if (!result) return false;
                }
                return true;
            }
            else if (obj1 is Tuple<object> && obj2 is Tuple<object>)
            {
                var tuple1 = obj1 as Tuple<object>;
                var tuple2 = obj2 as Tuple<object>;
                return tuple1.Equals(tuple2);
            }
            else if (obj1 is Tuple<object, object> && obj2 is Tuple<object, object>)
            {
                var tuple1 = obj1 as Tuple<object, object>;
                var tuple2 = obj2 as Tuple<object, object>;

                return tuple1.Item1.Equals(tuple2.Item1)
                       && tuple1.Item2.Equals(tuple2.Item2);
            }
            else if (obj1 is Dictionary<object, object>
                && obj2 is Dictionary<object, object>)
            {
                var dict1 = obj1 as Dictionary<object, object>;
                var dict2 = obj2 as Dictionary<object, object>;

                if (dict1.Count != dict2.Count) return false;
                if (dict1.Keys.Except(dict2.Keys).Any()) return false;
                if (dict2.Keys.Except(dict1.Keys).Any()) return false;
                return dict1.All(pair => equal_test(pair.Value, dict2[pair.Key]));
            }
            else if (obj1 is Term && obj2 is Term)
            {
                var term1 = obj1 as Term;
                var term2 = obj2 as Term;
                if (!term1.Op.Equals(term2.Op)) return false;
                return equal_test(term1.Args, term2.Args);
            }
            else
            {
                return obj1.Equals(obj2);
            }
        }

        /*
         *      >>> assoc({'x': 1}, 'x', 2)
                {'x': 2}
                >>> assoc({'x': 1}, 'y', 3)   
                {'x': 1, 'y': 3}
        */
        public static void Assoc(Dictionary<object, object> dict, object key, object value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }
        }


        public static Func<T1, T2, Func<T3, TResult>> Curry<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> function)
        {
            return (a, b) => c => function(a, b, c);
        }


        public static string PrintTuple(object obj)
        {
            if (obj is Tuple<object>)
            {
                var term = obj as Tuple<object>;
                return term.ToString();
            }
            else if (obj is Tuple<object, object>)
            {
                var term = obj as Tuple<object, object>;
                var builder = new StringBuilder();
                builder.Append(term.Item1.ToString()).Append(",").Append(term.Item2.ToString());
                return builder.ToString();
            }
            else if (obj is Tuple<object, object, object>)
            {
                var term = obj as Tuple<object, object, object>;
                var builder = new StringBuilder();
                builder.Append(term.Item1.ToString()).Append(",").Append(term.Item2.ToString()).Append(",").Append(term.Item3.ToString());
                return builder.ToString();
            }
            else if (obj is Tuple<object, object, object, object>)
            {
                var term = obj as Tuple<object, object, object, object>;
                var builder = new StringBuilder();
                builder.Append(term.Item1.ToString()).Append(",").Append(term.Item2.ToString()).Append(",").Append(term.Item3.ToString()).Append(",").Append(term.Item4.ToString());
                return builder.ToString();
            }
            else
            {
                return null;
            }
        }


        public static object Calculate(Func<Expression, Expression, BinaryExpression> func,
            object x, object y)
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
                var result = Expression.Lambda<Func<double>>(rExpr).Compile().Invoke();

                int iResult;
                if (LogicSharp.IsInt(result, out iResult))
                {
                    return iResult;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                return null;
            }
        }

        public static bool IsInt(object expression, out int number)
        {
            if (expression == null)
            {
                number = 0;
                return false;
            }

            return Int32.TryParse(Convert.ToString(expression,
                System.Globalization.CultureInfo.InvariantCulture),
                System.Globalization.NumberStyles.Any,
                System.Globalization.NumberFormatInfo.InvariantInfo,
                out number);
        }

        public static bool IsDouble(object expression, out double number)
        {
            if (expression == null)
            {
                number = 0.0;
                return false;
            }

            return Double.TryParse(Convert.ToString(expression,
                System.Globalization.CultureInfo.InvariantCulture),
                System.Globalization.NumberStyles.Any,
                System.Globalization.NumberFormatInfo.InvariantInfo,
                out number);
        }

        public static bool IsNumeric(object obj)
        {
            bool result;
            try
            {
                int inum;
                result = IsInt(obj, out inum);
                if (result)
                {
                    return true;
                }
            }
            catch (Exception)
            {                
            }

            try
            {
                double dnum;
                result = IsDouble(obj, out dnum);
                if (result)
                {
                    return true;
                }
            }
            catch (Exception)
            {
            }

            return false;
        }

    }
}
