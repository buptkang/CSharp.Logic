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

        public static string EqStrategy = "Manipulate Equation by using algebraic and equational rules.";
    }
}
