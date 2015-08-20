using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace CSharpLogic
{
    public class DyLogicObject : DynamicObject
    {
        public List<TraceStep> Traces = new List<TraceStep>();
        public int TraceCount { get { return Traces.Count; } }

        public List<TraceStep> CloneTrace()
        {
            return Traces.Select(ts => ts.Clone()).ToList();
        }

        public void ClearTrace()
        {
            Traces = new List<TraceStep>();
        }

        #region Dynamic Properties

        public readonly Dictionary<object, object> Properties 
                                    = new Dictionary<object, object>();

        public int Count
        {
            get { return Properties.Count; }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string name = binder.Name.ToLower();
            return Properties.TryGetValue(name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            // Converting the property name to lowercase 
            // so that property names become case-insensitive.
            Properties[binder.Name.ToLower()] = value;

            // You can always add a value to a dictionary, 
            // so this method always returns true. 
            return true;
        }

        #endregion
    }

    public static class DyLogicObjectExtension
    {
        public static void Reify(this DyLogicObject logicObj, Goal goal)
        {
            goal.Unify(logicObj.Properties);
        }

        public static void Reify(this DyLogicObject logicObj, IEnumerable<Goal> goals)
        {
            IEnumerable<KeyValuePair<object, object>> pairs =
                LogicSharp.logic_All(goals, logicObj.Properties);

            if (pairs == null)
            {
                return;
            }

            foreach (KeyValuePair<object, object> pair in pairs)
            {
                if (!logicObj.Properties.ContainsKey(pair.Key))
                {
                    logicObj.Properties.Add(pair.Key, pair.Value);
                }
            }
        }
    }
}
