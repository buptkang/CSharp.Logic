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

using System;

namespace CSharpLogic
{
    using System.Collections.ObjectModel;
    using System.Collections.Generic;
    using System.Linq;

    public delegate void ReifyUpdateHandler(object sender, EventArgs args);

    public class ReifyEventArgs : EventArgs
    {
        public ShapeSymbol CurrentShapeSymbol { get; set; }

        public ReifyEventArgs(ShapeSymbol ss)
        {
            CurrentShapeSymbol = ss;
        }
    }

    public abstract partial class ShapeSymbol
    {
        public event ReifyUpdateHandler ReifyShapeUpdated;

        protected virtual void RaiseReify(EventArgs e)
        {
            if (ReifyShapeUpdated != null)
            {
                ReifyShapeUpdated(this, e);
            }
        }

        //Cached symbols for non-concrete objects
        public ObservableCollection<ShapeSymbol> CachedSymbols { get; set; }
      
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
