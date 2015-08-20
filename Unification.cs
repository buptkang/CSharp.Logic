using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLogic
{
    public partial class LogicSharp
    {      
        private static bool UnifyImpl(Tuple<object, object> u, Tuple<object, object> v, Dictionary<object, object> s)
        {
            if (!Unify(u.Item1, v.Item1, s))
            {
                return false;
            }
            if (!Unify(u.Item2, v.Item2, s))
            {
                return false;
            }
            return true;
        }

        private static bool UnifyImpl(IEnumerable<object> u, IEnumerable<object> v, Dictionary<object, object> s)
        {
            var enumerable = u as IList<object> ?? u.ToList();
            var objects = v as IList<object> ?? v.ToList();

            if (enumerable.Count() != objects.Count()) return false;
            var pair = enumerable.Zip(objects, (first, second) 
                            => new Tuple<object, object>(first, second));
            return pair.All(item => Unify(item.Item1, item.Item2, s));
        }

        private static bool UnifyImpl(object u, object v, Dictionary<object, object> s)
        {
            return u.Equals(v);
        }

        private static bool UnifyImpl(Dictionary<object, object> u, 
            Dictionary<object, object> v,
            Dictionary<object, object> s)
        {
            if (v.Count != u.Count) return false;
            foreach (KeyValuePair<object, object> pair in u)
            {
                if (!v.ContainsKey(pair.Key))
                {
                    return false;
                }

                if (!Unify(pair.Value, v[pair.Key], s))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool UnifyImpl(Term u, Term v, Dictionary<object, object> s)
        {
            bool opUnifiable = Unify(u.Op, v.Op, s);

            if (opUnifiable)
            {
                return Unify(u.Args, v.Args, s);
            }
            return false;
        }

        public static bool Unify(object u, object v, Dictionary<object, object> s)
        {
            if (s == null)
            {
                s = new Dictionary<object, object>();
            }

            object tempU = LogicSharp.transitive_get(u, s);
            object tempV = LogicSharp.transitive_get(v, s);

            if (LogicSharp.equal_test(tempU, tempV))
            {
                return true;
            }

            if (Var.IsVar(tempU) && Var.IsVar(tempV))
            {
                return tempU.Equals(tempV);
            }

            if (Var.IsVar(tempU))
            {
                LogicSharp.Assoc(s, tempU, tempV);
                return true;
            }

            if (Var.IsVar(tempV))
            {
                LogicSharp.Assoc(s, tempV, tempU);
                return true;
            }

            dynamic a = tempU;
            dynamic b = tempV;

            return UnifyImpl(a, b, s);
        }

        public static object Unify_Object(DyLogicObject dy1, DyLogicObject dy2, Dictionary<object, object> s)
        {
            return Unify(dy1.Properties, dy2.Properties, s);
        }
    }
}
