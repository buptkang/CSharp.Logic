using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using CSharpLogic;

namespace CSharpLogic
{
    public abstract partial class ShapeSymbol
    {
        //Cached symbols for non-concrete objects
        public HashSet<ShapeSymbol> CachedSymbols { get; set; }
        public HashSet<KeyValuePair<object, EqGoal>> CachedGoals { get; set; }

        public bool ContainGoal(EqGoal goal)
        {
            return CachedGoals.Any(pair => pair.Value.Equals(goal));
        }

        public void RemoveGoal(EqGoal goal)
        {
            CachedGoals.RemoveWhere(pair => pair.Value.Equals(goal));
        }

        public List<EqGoal> RetrieveGoals()
        {
            return CachedGoals.Select(pair => pair.Value).ToList();
        }

        public virtual void UndoGoal(EqGoal goal, object parent) { }

        public object EvalGoal(object field, EqGoal goal)
        {
            var substitute = goal.ToDict();
            object result = null;
            if (Var.ContainsVar(field))
            {
                result = LogicSharp.Reify(field, substitute);
            }
            else
            {
                result = field;
            }
            return result; 
        }    
    }
}
