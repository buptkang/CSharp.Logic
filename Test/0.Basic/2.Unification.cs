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
    public class TestUnification
    {
        [Test]
        public void TestUnify()
        {
            /*
                        assert unify(1, 1, {}) == {}
                        assert unify(1, 2, {}) == False
                        assert unify(var(1), 2, {}) == {var(1): 2}
                        assert unify(2, var(1), {}) == {var(1): 2}
                        assert unify(2, var(1), {var(1):3}) = {}
                        assert unify(3, var(2), {var(1):3}) = {}
            */
            var dict = new Dictionary<object, object>();
            bool result = LogicSharp.Unify(1, 1, dict);
            Assert.True(result);
            dict = new Dictionary<object, object>();
            result = LogicSharp.Unify(1, 2, dict);
            Assert.False(result);

            dict = new Dictionary<object, object>();
            var variable = new Var(1);
            result = LogicSharp.Unify(variable, 2, dict);
            Assert.True(result);
            Assert.True(dict.Count == 1);
            Assert.True(dict.ContainsKey(variable));
            Assert.True(dict[variable].Equals(2));

            dict = new Dictionary<object, object>();
            result = LogicSharp.Unify(2, variable, dict);
            Assert.True(result);
            Assert.True(dict.Count == 1);
            Assert.True(dict.ContainsKey(variable));
            Assert.True(dict[variable].Equals(2));

            dict = new Dictionary<object, object>();
            dict.Add(variable, 3);
            result = LogicSharp.Unify(2, variable, dict);
            Assert.False(result);

            var variable2 = new Var(2);
            dict = new Dictionary<object, object>();
            dict.Add(variable, 3);
            result = LogicSharp.Unify(3, variable2, dict);
            Assert.True(result);
        }

        [Test]
        public void TestUnify2()
        {
            //            assert unify((1, 2), (1, 2), {}) == {}
            //            assert unify([1, 2], [1, 2], {}) == {}
            //            assert unify((1, 2), (1, 2, 3), {}) == False
            //            assert unify((1, var(1)), (1, 2), {}) == {var(1): 2}
            //            assert unify((1, var(1)), (1, 2), {var(1): 3}) == False
            var tuple1 = new Tuple<object, object>(1, 2);
            var tuple2 = new Tuple<object, object>(1, 2);
            var dict = new Dictionary<object, object>();
            bool result = LogicSharp.Unify(tuple1, tuple2, dict);
            Assert.True(dict.Count == 0);
            Assert.True(result);

            var lst1 = new List<object>() { 1, 2 };
            var lst2 = new List<object>() { 1, 2 };
            dict = new Dictionary<object, object>();
            result = LogicSharp.Unify(lst1, lst2, dict);
            Assert.True(dict.Count == 0);
            Assert.True(result);

            var variable = new Var(1);
            var tuple3 = new Tuple<object, object>(1, variable);
            dict = new Dictionary<object, object>();
            result = LogicSharp.Unify(tuple1, tuple3, dict);
            Assert.True(dict.Count == 1);
            Assert.True(result);
            Assert.True(dict.ContainsKey(variable));
            Assert.True(dict[variable].Equals(2));

            dict = new Dictionary<object, object>();
            dict.Add(variable, 3);
            result = LogicSharp.Unify(tuple1, tuple3, dict);
            Assert.False(result);
        }

        [Test]
        public void TestUnify3()
        {
            /*
            assert unify({1: 2}, {1: 2}, {}) == {}
            assert unify({1: 2}, {1: 3}, {}) == False
            assert unify({2: 2}, {1: 2}, {}) == False
            assert unify({1: var(5)}, {1: 2}, {}) == {var(5): 2}
            assert unify({1: var(5)}, {2: var(4)}, {}) == {}
             */
            var dict1 = new Dictionary<object, object>();
            dict1.Add(1, 2);
            var dict2 = new Dictionary<object, object>();
            dict2.Add(1, 2);
            var dict = new Dictionary<object, object>();

            bool result = LogicSharp.Unify(dict1, dict2, dict);
            Assert.True(result);

            var dict3 = new Dictionary<object, object>();
            dict3.Add(1, 3);
            dict = new Dictionary<object, object>();
            result = LogicSharp.Unify(dict1, dict3, dict);
            Assert.False(result);

            var dict4 = new Dictionary<object, object>();
            dict4.Add(2, 2);
            dict = new Dictionary<object, object>();
            result = LogicSharp.Unify(dict1, dict4, dict);
            Assert.False(result);

            var variable = new Var(5);
            var dict5 = new Dictionary<object, object>();
            dict5.Add(1, variable);
            dict = new Dictionary<object, object>();
            result = LogicSharp.Unify(dict1, dict5, dict);
            Assert.True(result);
            Assert.True(dict.Count == 1);
            Assert.True(dict.ContainsKey(variable));
            Assert.True(dict[variable].Equals(2));

            dict5 = new Dictionary<object, object>();
            dict5.Add(1, variable);
            dict = new Dictionary<object, object>();
            dict1 = new Dictionary<object, object>();
            var variable2 = new Var(4);
            dict1.Add(2, variable2);
            result = LogicSharp.Unify(dict1, dict5, dict);
            Assert.False(result);
        }

        [Test]
        public void TestUnifyComplex()
        {
            //    assert unify((1, {2: 3}), (1, {2: 3}), {}) == {}
            //    assert unify((1, {2: 3}), (1, {2: 4}), {}) == False
            //    assert unify((1, {2: var(5)}), (1, {2: 4}), {}) == {var(5): 4}
            //
            //    assert unify({1: (2, 3)}, {1: (2, var(5))}, {}) == {var(5): 3}
            //    assert unify({1: [2, 3]}, {1: [2, var(5)]}, {}) == {var(5): 3}

            var dict = new Dictionary<object, object>();

            var dict1 = new Dictionary<object, object>();
            dict1.Add(2, 3);

            var tuple1 = new Tuple<object, object>(1, dict1);
            var tuple2 = new Tuple<object, object>(1, dict1);

            bool result = LogicSharp.Unify(tuple1, tuple2, dict);
            Assert.True(result);
            Assert.True(dict.Count == 0);

            var dict2 = new Dictionary<object, object>();
            dict2.Add(2, 4);
            var tuple3 = new Tuple<object, object>(1, dict2);
            result = LogicSharp.Unify(tuple1, tuple3, dict);
            Assert.False(result);

            var variable = new Var(5);
            var dict3 = new Dictionary<object, object>();
            dict3.Add(2, variable);
            var tuple4 = new Tuple<object, object>(1, dict3);
            result = LogicSharp.Unify(tuple3, tuple4, dict);
            Assert.True(result);
            Assert.True(dict.Count == 1);
            Assert.True(dict.ContainsKey(variable));
            Assert.True(dict[variable].Equals(4));
        }
    }
}
