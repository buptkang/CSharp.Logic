using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpLogic
{
    public class SubstitutionRule
    {
        //Reify and Unify trace
        public static string ApplySubstitute(object source, object term)
        {
            return string.Format("Substitute Term {1} into Object {0}",
               source.ToString(), term.ToString());
        }

        public static string ApplySubstitute()
        {
            return string.Format("Consider substitute given knowledge to question");
        }
    }
}
