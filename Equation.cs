using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpLogic
{
    public partial class Equation : DyLogicObject, IEquationLogic, IEval
    {
        #region Properties and Constructors

        public string EqLabel { get; set; }
        public object Lhs { get; set; }
        public object Rhs { get; set; }
        public bool IsGenerated { get; set; }

        public Equation() 
        {
            CachedEntities = new HashSet<object>();
            CachedObjects = new HashSet<KeyValuePair<object, object>>();
        }

        public Equation(string label, object lhs,
                        object rhs, bool generated = false)
        {
            EqLabel = label;
            Lhs = lhs;
            Rhs = rhs;
            IsGenerated = generated;
            CachedEntities = new HashSet<object>();
            CachedObjects = new HashSet<KeyValuePair<object, object>>();
        }

        public Equation(object lhs, object rhs)
            : this(null, lhs, rhs, false)
        { }

        public Equation(object lhs)
            : this(null, lhs, null, false)
        { }

        public Equation(object lhs, object rhs, bool generated)
            : this(null, lhs, rhs, generated)
        { }

        //copy constructor
        public Equation(Equation eq)
        {
            EqLabel = eq.EqLabel;
            var lhsVar  = eq.Lhs as Var;
            var lhsTerm = eq.Lhs as Term;
            if (lhsVar != null)
            {
                Lhs = lhsVar.Clone();
            }
            else if (lhsTerm != null)
            {
                Lhs = lhsTerm.Clone();
            }
            else
            {
                Lhs = eq.Lhs;
            }

            var rhsVar  = eq.Rhs as Var;
            var rhsTerm = eq.Rhs as Term;
            if (rhsVar != null)
            {
                Rhs = rhsVar.Clone();
            }
            else if (rhsTerm != null)
            {
                Rhs = rhsTerm.Clone();
            }
            else
            {
                Rhs = eq.Rhs;
            }
            IsGenerated = eq.IsGenerated;
        }

        #endregion

        #region Utility Functions

        public bool ContainsVar()
        {
            var lhsVar = Lhs as Var;
            var rhsVar = Rhs as Var;

            if (lhsVar != null || rhsVar != null) return true;

            var lhsTerm = Lhs as Term;
            var rhsTerm = Rhs as Term;

            if (lhsTerm != null && lhsTerm.ContainsVar()) return true;
            if (rhsTerm != null && rhsTerm.ContainsVar()) return true;
            return false;
        }

        public bool ContainsVar(Var variable)
        {
            var lhsVar = Lhs as Var;
            bool result;
            if (lhsVar != null)
            {
                result = lhsVar.Equals(variable);
                if (result) return true;
            }

            var rhsVar = Rhs as Var;
            if (rhsVar != null)
            {
                result = Rhs.Equals(variable);
                if (result) return true;
            }
            return false;
        }

        public Equation Clone()
        {
            var equation = (Equation)this.MemberwiseClone();

            var lhs = Lhs as Term;
            if (lhs != null)
            {
                equation.Lhs = lhs.Clone();
            }

            var rhs = Rhs as Term;
            if (rhs != null)
            {
                equation.Rhs = rhs.Clone();
            }

            return equation;
        }

        #endregion

        #region Object Override functions

        public override bool Equals(object obj)
        {
            var eq = obj as Equation;
            if (eq != null)
            {
                return Lhs.Equals(eq.Lhs) && Rhs.Equals(eq.Rhs);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Lhs.GetHashCode() ^ this.Rhs.GetHashCode();
        }

        public override string ToString()
        {
            if (Rhs != null)
            {
                return string.Format("{0}={1}", Lhs.ToString(), Rhs.ToString());                
            }
            else
            {
                return Lhs.ToString();
            }

        }

        #endregion
    }
}
