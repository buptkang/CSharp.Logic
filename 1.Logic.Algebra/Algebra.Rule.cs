using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace CSharpLogic
{
    public static class AlgebraRule
    {
        public enum AlgebraRuleType
        {
            Distributive,
            Associative,
            Commutative,
            Identity,
            Inverse
        }

        public static string Rule(AlgebraRuleType ruleType, 
                                object obj1, object obj2)
        {
            switch (ruleType)
            {
               case AlgebraRuleType.Distributive:
                    return string.Format("Apply distributive law between {0} and {1}", obj1, obj2);
               case AlgebraRuleType.Associative:
                    return string.Format("Apply associative law between {0} and {1}", obj1, obj2);
               case AlgebraRuleType.Commutative:
                    return string.Format("Apply commutative law between {0} and {1}", obj1, obj2);
               case AlgebraRuleType.Identity:
                    return string.Format("Apply identity law on {0}", obj1);
               case AlgebraRuleType.Inverse:
                    return string.Format("Apply inverse law between {0} and {1}", obj1, obj2);
               default:
                    break;
            }
            return null;
        }

        public static String Rule(AlgebraRuleType ruleType)
        {
            switch (ruleType)
            {
                case AlgebraRuleType.Distributive:
                    return string.Format("Consider distributive law ");
                case AlgebraRuleType.Associative:
                    return string.Format("Consider associative law ");
                case AlgebraRuleType.Commutative:
                    return string.Format("Consider commutative law ");
                case AlgebraRuleType.Identity:
                    return string.Format("Consdier identity law ");
                case AlgebraRuleType.Inverse:
                    return string.Format("Consider inverse law");
                default:
                    break;
            }
            return null;
        }

    }
}
