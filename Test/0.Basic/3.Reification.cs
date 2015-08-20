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
    public class TestReification
    {
        [Test]
        public void TestReify()
        {
            var x = new Var();
            var y = new Var();
            var z = new Var();
            var dict = new Dictionary<object, object>();
            dict.Add(x, 1);
            dict.Add(y, 2);
            dict.Add(z, new Tuple<object, object>(x, y));

            object result = LogicSharp.Reify(x, dict);
            Assert.True(result.Equals(1));
            Assert.True(LogicSharp.Reify(10, dict).Equals(10));

            var t = new Var('t');
            result = LogicSharp.Reify(t, dict);
            Assert.True(t.Equals(result));

            var tuple = new Tuple<object, object>(1, y);
            Assert.True(LogicSharp.Reify(tuple, dict)
                        .Equals(new Tuple<object, object>(1, 2)));

            // assert reify((1, (x, (y, 2))), s) == (1, (1, (2, 2)))

            var tuple1 = new Tuple<object, object>(y, 2);
            var tuple2 = new Tuple<object, object>(x, tuple1);
            var tuple3 = new Tuple<object, object>(1, tuple2);

            var obj = LogicSharp.Reify(tuple3, dict);

            var tuple10 = new Tuple<object, object>(2, 2);
            var tuple20 = new Tuple<object, object>(1, tuple10);
            var tuple30 = new Tuple<object, object>(1, tuple20);
            Assert.True(obj.Equals(tuple30));

            // assert reify(z, s) == (1, 2)
            Assert.True(LogicSharp.Reify(z, dict).Equals(new Tuple<object, object>(1, 2)));
        }

        [Test]
        public void TestReify_Dict()
        {
            var x = new Var();
            var y = new Var();
            /*           
             *          s = {x: 2, y: 4}
                        e = {1: x, 3: {5: y}}
                        assert reify(e, s) == {1: 2, 3: {5: 4}}
             */
            var dict = new Dictionary<object, object>();
            dict.Add(x, 2);
            dict.Add(y, 4);

            var testDict = new Dictionary<object, object>();
            testDict.Add(1, x);
            var embedDict = new Dictionary<object, object>();
            embedDict.Add(5, y);
            testDict.Add(3, embedDict);

            var mockDict = new Dictionary<object, object>();
            mockDict.Add(1, 2);
            var mockEmbedDict = new Dictionary<object, object>();
            mockEmbedDict.Add(5, 4);
            mockDict.Add(3, mockEmbedDict);

            var obj = LogicSharp.Reify(testDict, dict) as Dictionary<object, object>;
            var test1 = obj[1];

            Assert.True(test1.Equals(mockDict[1]));

            var test2 = obj[3] as Dictionary<object, object>;
            var mocktest2 = mockDict[3] as Dictionary<object, object>;
            Assert.True(test2[5].Equals(mocktest2[5]));

        }

        [Test]
        public void TestReify_List()
        {
            /*          x, y = var(), var()
                        s = {x: 2, y: 4}
                        e = [1, [x, 3], y]
                        assert reify(e, s) == [1, [2, 3], 4]   */

            var x = new Var();
            var y = new Var();
            var dict = new Dictionary<object, object>();
            dict.Add(x, 2);
            dict.Add(y, 4);

            var lst = new List<object>();
            lst.Add(1);
            lst.Add(new List<object>() { x, 3 });
            lst.Add(y);

            var obj = LogicSharp.Reify(lst, dict) as List<object>;
            Assert.IsNotNull(obj);

            var mockList = new List<object>()
            {
                1,
                new List<object>() {2, 3},
                4
            };
            var test1 = obj[1] as List<object>;
            Assert.IsNotNull(test1);
            Assert.True(test1[0].Equals(2));
            var test2 = obj[2];
            Assert.IsNotNull(test2);
            Assert.True(test2.Equals(4));
        }

        [Test]
        public void TestReify_Complex()
        {
            /*            
             * x, y = var(), var()
            s = {x: 2, y: 4}
            e = {1: [x], 3: (y, 5)}

            assert reify(e, s) == {1: [2], 3: (4, 5)}
             */
            var x = new Var();
            var y = new Var();
            var dict = new Dictionary<object, object>();
            dict.Add(x, 2);
            dict.Add(y, 4);

            var input = new Dictionary<object, object>();
            input.Add(1, new List<object>() { x });
            input.Add(3, new Tuple<object, object>(y, 5));

            var obj = LogicSharp.Reify(input, dict) as Dictionary<object, object>;
            Assert.IsNotNull(obj);
            var test1 = obj[1] as List<object>;
            Assert.IsNotNull(test1);
            Assert.True(2.Equals(test1[0]));

            var test2 = obj[3] as Tuple<object, object>;
            Assert.IsNotNull(test2);
            Assert.True(4.Equals(test2.Item1));
        }
    }
}
