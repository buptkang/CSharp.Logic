﻿/*******************************************************************************
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

        public static string SubstituteKC()
        {
            return "algebraic variable substitution";
        }

        public static string SubstitutionStrategy = "Substitute existing knowledge into the question knowledge pattern.";
    }
}
