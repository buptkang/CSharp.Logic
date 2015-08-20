using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace CSharpLogic
{
    public static class EquationsRule
    {
        public enum EquationRuleType
        {
            Transitive,
            Symmetric,
            Inverse
        }

        public static string Rule(EquationRuleType ruleType,
            object obj1, object obj2)
        {
            switch (ruleType)
            {
                case EquationRuleType.Inverse:
                    return "TODO";
                case EquationRuleType.Symmetric:
                    return string.Format("Apply Symmetric law on equation {0}", obj1);
                case EquationRuleType.Transitive:
                    return string.Format("Apply Transitive law on equation {0}", obj1);
            }
            return null;
        }

        public static string Rule(EquationRuleType ruleType)
        {
            switch (ruleType)
            {
                case EquationRuleType.Inverse:
                    return "TODO";
                case EquationRuleType.Symmetric:
                    return string.Format("Consider Symmetric law on equation x=y -> y=x");
                case EquationRuleType.Transitive:
                    return string.Format("Consider Transitive law on equation x^a=y^a->x=y");
            }
            return null;
        }
    }
}
