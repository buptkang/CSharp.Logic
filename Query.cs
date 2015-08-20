using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NUnit.Framework.Constraints;

namespace CSharpLogic
{
    public partial class Query : Equation, INotifyPropertyChanged
    {
        public Guid QueryQuid { get; set; }

        private object _constraint1;
        public object Constraint1 //can be term or string
        {
            get{ return _constraint1;}
            set
            {
                _constraint1 = value;
                NotifyPropertyChanged("Constraint1");               
            } 
        }
        private ShapeType? _constraint2;
        public ShapeType? Constraint2
        {
            get { return _constraint2; }
            set
            {
                _constraint2 = value;
                NotifyPropertyChanged("Constraint2");               
            }
        }

        public bool Success { get; set; }
        public object FeedBack { get; set; } //when query failed, give feedback to user
        public string Instruction { get; set; }


        public Query(object constraint1, ShapeType? constraint2 = null)
        {
            Constraint1 = constraint1;
            Constraint2 = constraint2;
            QueryQuid = Guid.NewGuid();
        }

        public Query(ShapeType? st) : this(null, st) {}

        public Query(Equation eq) : base(eq)
        {
        }

        #region IPropertyChanged Event

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

        #region Override functions

        public override string ToString()
        {
            return "TODO";
        }

        public override bool Equals(object obj)
        {
            var query = obj as Query;
            if (query == null) return false;

            if (Constraint1 != null && Constraint2 != null)
            {
                return Constraint1.Equals(query.Constraint1) &&
                       Constraint2.Equals(query.Constraint2);
            }

            if (Constraint1 == null) return Constraint2.Equals(query.Constraint2);
            if (Constraint2 == null) return Constraint1.Equals(query.Constraint1);

            throw new Exception("Cannot reach here");
        }

        public override int GetHashCode()
        {
            if (Constraint1 != null && Constraint2 != null)
            {
                return Constraint1.GetHashCode() ^ Constraint2.GetHashCode();
            }

            if (Constraint1 == null) return Constraint2.GetHashCode();
            if (Constraint2 == null) return Constraint1.GetHashCode();

            throw new Exception("Cannot reach here");
        }

        #endregion
    }
}
