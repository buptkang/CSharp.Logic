using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpLogic
{
    public class ArithRule
    {
        public static string CalcRule(object method, object left,
            object right, object result)
        {
            return string.Format("Make the calculation: {1} {0} {2}",
                method.ToString(), left.ToString(), right.ToString());
        }

        public static string CalcRule(object method)
        {
            return string.Format("Think about Calculation: {0}", method.ToString());
        }

    }
}
