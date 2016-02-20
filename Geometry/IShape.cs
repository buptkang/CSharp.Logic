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

using System.Collections.ObjectModel;

namespace CSharpLogic
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public abstract partial class Shape : Equation,
        IEquatable<Shape>, INotifyPropertyChanged
    {
        public string Label { get; set; }
        public ShapeType ShapeType { get; set; }
        public CoordinateSystemType Coordinate { get; set; }
        public RepresentationType Repr { get; set; }

        #region Interaction Purpuse
        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property. 
        // The CallerMemberName attribute that is applied to the optional propertyName 
        // parameter causes the property name of the caller to be substituted as an argument. 
        protected void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        protected Shape(ShapeType shapeType, string label)
            : this()
        {
            Label = label;
            ShapeType = shapeType;
        }

        protected Shape()
        {
            Coordinate = CoordinateSystemType.Cartesian;
            Repr = RepresentationType.Explicit;
        }

        protected Shape(Equation equation) : base(equation)
        {
            Coordinate = CoordinateSystemType.Cartesian;
            Repr = RepresentationType.Explicit;
        }

        public virtual bool Concrete
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual List<Var> GetVars()
        {
            return null;
        }

        #region IEquatable

        public virtual bool Equals(Shape other)
        {
            if (this.Label != null)
            {
                return this.Label.Equals(other.Label)
                   && this.ShapeType.Equals(other.ShapeType)
                   && this.Coordinate.Equals(other.Coordinate);
            }
            else
            {
                return this.ShapeType.Equals(other.ShapeType)
                   && this.Coordinate.Equals(other.Coordinate);
            }
        }

        public override int GetHashCode()
        {
            if (Label != null)
            {
                return Label.GetHashCode() ^ Coordinate.GetHashCode() ^ ShapeType.GetHashCode();
            }
            else
            {
                return Coordinate.GetHashCode() ^ ShapeType.GetHashCode();
            }
        }

        #endregion

        public abstract object GetInputType();
    }

    public abstract partial class ShapeSymbol
    {
        public Shape Shape { get; set; }

        protected ShapeSymbol(Shape _shape)
        {
            CachedSymbols = new ObservableCollection<ShapeSymbol>();
            CachedGoals = new HashSet<KeyValuePair<object, EqGoal>>();
            Shape = _shape;
        }

        public abstract object RetrieveConcreteShapes();

        public abstract object GetOutputType();

        //forward-solving (Query Search)
        public abstract bool UnifyProperty(string label, out object obj);

        public abstract bool UnifyExplicitProperty(EqGoal goal);
        
        //backward-solving (Goal Search)
        public abstract bool UnifyProperty(EqGoal goal, out object obj);

        public abstract bool UnifyShape(ShapeSymbol ss);
       
        public abstract bool ApproximateMatch(object obj);
    }
}