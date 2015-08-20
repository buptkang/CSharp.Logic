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
    using System.Collections.Generic;

    [TestFixture]
    public class TestDyLogicObject
    {
        [Test]
        public void test_unify_dyna_object()
        {
            /*                
                        assert unify_object(Foo(1, 2), Foo(1, 2), {}) == {}
                        assert unify_object(Foo(1, 2), Foo(1, 3), {}) == False
                        assert unify_object(Foo(1, 2), Foo(1, var(3)), {}) == {var(3): 2}
             */
            dynamic foo = new DyLogicObject();
            foo.a = 1;
            foo.b = 2;

            dynamic foo2 = new DyLogicObject();
            foo2.a = 1;
            foo2.b = 2;

            var dict = new Dictionary<object, object>();
            bool result = LogicSharp.Unify_Object(foo, foo2, dict);
            Assert.True(result);
            Assert.True(dict.Count == 0);

            dynamic foo3 = new DyLogicObject();
            foo3.a = 1;
            foo3.b = 3;
            result = LogicSharp.Unify_Object(foo, foo3, dict);
            Assert.False(result);

            dynamic foo4 = new DyLogicObject();
            foo4.a = 1;
            var variable = new Var(3);
            foo4.b = variable;
            result = LogicSharp.Unify_Object(foo, foo4, dict);
            Assert.True(result);
            Assert.True(dict.Count == 1);
            Assert.True(dict.ContainsKey(variable));
            Assert.True(dict[variable].Equals(2));
        }

        [Test]
        public void test_reify_object_dynamic()
        {
            //dynamic approach
            /*            obj = reify_object(Foo(1, var(3)), {var(3): 4})
                        assert obj.a == 1
                        assert obj.b == 4

                        f = Foo(1, 2)
                        assert reify_object(f, {}) is f
             */

            dynamic foo = new DyLogicObject();
            foo.a = 1;
            var variable = new Var(3);
            foo.b = variable;

            var dict = new Dictionary<object, object>();
            dict.Add(variable, 4);

            dynamic obj = LogicSharp.Reify_Object(foo, dict);
            Assert.NotNull(obj);
            Assert.True(1.Equals(obj.a));
            Assert.True(4.Equals(obj.b));

            dynamic f = new DyLogicObject();
            f.a = 1;
            f.b = 2;

            dict = new Dictionary<object, object>();
            obj = LogicSharp.Reify_Object(f, dict);
            Assert.True(obj == f);
        }
    }
}
