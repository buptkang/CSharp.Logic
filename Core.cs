using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLogic
{
    public partial class LogicSharp
    {
        #region Goals

        public static Func<object,object,Func<Dictionary<object,object>, bool>> 
            Equal()
        {
            Func<object, object, Dictionary<object, object>, bool> functor = Unify;
            return Curry(functor);
        }

        public static bool Memberof(object x, List<object> coll)
        {
            return true;
        }

        #endregion

        #region Logical combination of goals

        /// <summary>
        /// mplus functor in minikanren
        /// </summary>
        /// <param name="goals"></param>
        /// <returns></returns>

        public static IEnumerable<KeyValuePair<object,object>> logic_Any
            (IEnumerable<Goal> g, Dictionary<object,object> substitutions)
        {
            List<Goal> goals = g.ToList();
            if (goals.Count() == 1)
            {
                var goal = goals[0];
                if (goal.Unify(substitutions))
                {
                    return substitutions;                    
                }
                else
                {
                    return null;
                }
            }
            else
            {
                var values = new HashSet<KeyValuePair<object, object>>();
                foreach (KeyValuePair<object, object> pair in substitutions)
                {
                    values.Add(pair);
                }

                foreach (Goal goal in goals)
                {
                    var clonedDict = LogicSharp.CloneDictionaryCloningValues(substitutions);
                    if (goal.Unify(clonedDict))
                    {
                        IEnumerable<KeyValuePair<object, object>> lst = LogicSharp.DiffTwoDictionary(clonedDict, substitutions);
                        foreach (var pair in lst)
                        {
                            if (!values.Contains(pair))
                            {
                                values.Add(pair);                                
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                return values;
            }
        }

        public static IEnumerable<KeyValuePair<object,object>> logic_All
            (IEnumerable<Goal> g, Dictionary<object, object> substitutions)
        {
            List<Goal> goals = g.ToList();
            if (goals.Count() == 1)
            {
                var goal = goals[0];
                if (goal.Unify(substitutions))
                {
                    return substitutions;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                var values = new Dictionary<object, object>();
                foreach (KeyValuePair<object, object> pair in substitutions)
                {
                    values.Add(pair.Key, pair.Value);
                }

                foreach (Goal goal in goals)
                {
                    var clonedDict = LogicSharp.CloneDictionaryCloningValues(substitutions);
                    if (goal.Unify(clonedDict))
                    {
                        IEnumerable<KeyValuePair<object, object>> lst = LogicSharp.DiffTwoDictionary(clonedDict, substitutions);
                        foreach (var pair in lst)
                        {
                            if (!values.ContainsKey(pair.Key))
                            {
                                values.Add(pair.Key, pair.Value);
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }            
                }
                return values;
            }
        }

        public static IEnumerable<KeyValuePair<object, object>> logic_Conde
            (IEnumerable<IEnumerable<Goal>> orAndGoals, Dictionary<object, object> substitutions)
        {
            var result = new HashSet<KeyValuePair<object, object>>();
            IEnumerable<KeyValuePair<object, object>> tempResult;
            IEnumerator<IEnumerable<Goal>> goalEnumerator = orAndGoals.GetEnumerator();
            while (goalEnumerator.MoveNext())
            {
                var clonedDict = CloneDictionaryCloningValues(substitutions);
                tempResult = logic_All(goalEnumerator.Current, clonedDict);
                if (tempResult != null)
                {
                    IEnumerable<KeyValuePair<object, object>> temp =
                        logic_Any(goalEnumerator.Current, tempResult as Dictionary<object, object>);
                    foreach (KeyValuePair<object, object> pair in temp)
                    {
                        if (!result.Contains(pair))
                        {
                            result.Add(pair);
                        }
                    }
                }
            } 
            
            return result;
        }

        #endregion

        #region User Execution

        public static object Run(Var variable, DyLogicObject obj)
        {
            if (obj.Properties.ContainsKey(variable))
            {
                return obj.Properties[variable];                
            }
            else
            {
                return null;
            }
        }

        public static object Run(Var variable, Goal goal, DyLogicObject obj)
        {
            if (goal.Unify(obj.Properties))
            {
                return obj.Properties[variable];
            }
            else
            {
                return null;
            }
        }

        public static object Run(Var variable, EqGoal goal)
        {
            if (variable.Equals(goal.Lhs)) return goal.Rhs;
            if (variable.Equals(goal.Rhs)) return goal.Lhs;
            return null;
        }

        public static object Run(Tuple<Var, Var> tuple, IEnumerable<Goal> goals)
        {
            var queue = new Queue<Goal>();

            var dict = new Dictionary<object, object>();
            foreach (Goal gl in goals)
            {
                bool unifiable = gl.Unify(dict);
                if (!unifiable)
                {
                    queue.Enqueue(gl);
                }
            }

            while (queue.Count != 0)
            {
                Goal tempGoal = queue.Dequeue();
                tempGoal.Reify(dict);
                tempGoal.Unify(dict);
            }

            return dict;
        }

        public static object Run(Var variable, List<Goal> goals)
        {
            return null;
        }

        public static object Run(Var variable, List<Goal> goals, DyLogicObject obj)
        {
            return null;
        }

        #endregion

        #region EarlyGoal Exception

        public class EarlyGoalException : Exception
        {
            public EarlyGoalException(string msg) : base()
            {
                
            }            
        }

        #endregion
    }
}
