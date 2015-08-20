using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpLogic
{
    public class Var
    {
        #region Properties and Constructors

        private static dynamic _id = 1;

        private object _token;

        public object Token { get { return _token; } set { _token = value; } }

        public Var()
        {
            _token = _id;
            _id += 1;
        }

        public Var(object id)
        {
            _token = id;
        }

        #endregion

        #region Basic Override 

        public Var Clone()
        {
            var variable = (Var)MemberwiseClone();
            variable._token = _token.ToString();
            return variable;
        }

        public override string ToString()
        {
            return this._token.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is Var)
            {
                var mVar = obj as Var;
                return mVar.ToString().Equals(this.ToString());
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return Token.GetHashCode();
        }

        #endregion

        #region Var Checking

        public static bool IsVar(Var obj)
        {
            return true;
        }

        public static bool IsVar(object obj)
        {
            if (obj is Var)
            {
                //dynamic a = obj;
                //IsVar(a);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsNotVar(object obj)
        {
            return !IsVar(obj);
        }

        public static bool ContainsVar(object obj)
        {
            if (obj is Var)
            {
                return true;
            }
            else if (obj is List<object>)
            {
                var lst = obj as List<object>;
                return lst.Select(ContainsVar).Any(result => result);
            }
            else if(obj is Term)
            {
                var term = obj as Term;
                return ContainsVar(term.Args);
            }
            else if (obj is Tuple<object>)
            {
                var tuple = obj as Tuple<object>;
                return ContainsVar(tuple.Item1);
            }
            else if (obj is Tuple<object, object>)
            {
                var tuple = obj as Tuple<object, object>;
                return ContainsVar(tuple.Item1) || ContainsVar(tuple.Item2);
            }
            else if (obj is IEnumerable<object>)
            {
                var enumerator = obj as IEnumerable<object>;
                foreach (var tempObj in enumerator)
                {
                    if (ContainsVar(tempObj))
                        return true;
                }
                return false;
            }
            else if (obj is IDictionary<object, object>)
            {
                var dict = obj as IDictionary<object, object>;
                foreach (KeyValuePair<object, object> pair in dict)
                {
                    if (ContainsVar(pair.Key) || ContainsVar(pair.Value))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        public static Var GetVar(object obj)
        {
            while (true)
            {
                var variable = obj as Var;
                if (variable != null) return variable;

                var tuple = obj as Tuple<object>;
                if (tuple != null)
                {
                    obj = tuple.Item1;
                    continue;
                }

                var tuple2 = obj as Tuple<object, object>;
                if (tuple2 != null)
                {
                    var result = GetVar(tuple2.Item1);
                    if (result != null) return result;
                    else
                    {
                        obj = tuple2.Item2;
                        continue;
                    }
                }

                return null;              
            }
        }

        #endregion
    }
}