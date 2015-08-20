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
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;

    [TestFixture]
    public class TestVariable
    {
        [Test]
        public void Test_Clone()
        {
            var variable = new Var('a'); 
            var variable1 = variable.Clone();
            variable1.Token = "b";
            Assert.True(variable.Token.Equals('a'));
            Assert.True(variable.ToString().Equals("a"));
        }

        [Test]
        public void Test_IsVar()
        {
            Assert.IsTrue(Var.IsVar(new Var(3)));
            Assert.IsFalse(Var.IsVar(3));

            Assert.IsTrue((new Var(1)).Equals(new Var(1)));
            Assert.IsFalse((new Var()).Equals(new Var()));
        }

        [Test]
        public void Test_IsVar_2()
        {
            //var itself
            object ob2 = new Var('1');
            Assert.True(Var.ContainsVar(ob2));

            //Tuple
            object ob1 = new Tuple<object, object>(new Var('x'), 1);
            object ob0 = new Tuple<object>(1);
            Assert.True(Var.ContainsVar(ob1));
            Assert.False(Var.ContainsVar(ob0));

            //IEnumerable
            object ob3 = new List<object>() {1, 1, 1};
            object ob4 = new List<object>() { new Var('x'), 1 };
            Assert.False(Var.ContainsVar(ob3));
            Assert.True(Var.ContainsVar(ob4));

            //Dictionary
            var ob5 = new Dictionary<object, object>();
            ob5.Add(new Var('y'), 1);
            var ob6 = new Dictionary<object, object>();
            ob6.Add(2,3);
            Assert.True(Var.ContainsVar(ob5));
            Assert.False(Var.ContainsVar(ob6));
        }
    }
}
