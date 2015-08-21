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

namespace CSharpLogic
{
    using System;

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
