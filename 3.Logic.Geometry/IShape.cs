using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpLogic;
using System.ComponentModel;

namespace CSharpLogic
{
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
            CachedSymbols = new HashSet<ShapeSymbol>();
            CachedGoals = new HashSet<KeyValuePair<object, EqGoal>>();
            Shape = _shape;
        }

        public abstract object RetrieveConcreteShapes();
        public abstract object GetOutputType();
        public abstract bool UnifyProperty(string label, out object obj);
    }
}
