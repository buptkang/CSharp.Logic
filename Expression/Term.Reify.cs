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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    public partial class Term
    {
        #region Reification

        public object Reify(EqGoal eqGoal)
        {
            Dictionary<object, object> dict = eqGoal.ToDict();
            return Reify(dict);
        }

        public Term Reify(Dictionary<object, object> s)
        {
            var gArgs = LogicSharp.Reify(Args, s);
            return new Term(Op, gArgs);
        }

        #endregion
    }
}
