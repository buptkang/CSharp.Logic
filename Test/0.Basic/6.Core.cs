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
    public class TestCore
    {
        [Test]
        public void TestWalk()
        {
            /*
             * s = {1: 2, 2: 3}
            assert walk(2, s) == 3
            assert walk(1, s) == 3
            assert walk(4, s) == 4 
            Utils.transitive_get() 
             */

            var dict = new Dictionary<object, object>();
            dict.Add(1,2);
            dict.Add(2,3);

            object obj = LogicSharp.transitive_get(2, dict);
            Assert.True(3.Equals(obj));

            obj = LogicSharp.transitive_get(1, dict);
            Assert.True(3.Equals(obj));

            obj = LogicSharp.transitive_get(4, dict);
            Assert.True(4.Equals(obj));
        }

        [Test]
        public void TestDeepWalk()
        {
            var dict = new Dictionary<object, object>();
            var z = new Var('z');
            var y = new Var('y');
            var x = new Var('x');
            dict.Add(z, 6);
            dict.Add(y, 5);
            var tuple = new Tuple<object, object>(y, z);
            dict.Add(x, tuple);

            object obj = LogicSharp.transitive_get(x, dict);
            Assert.True(obj.Equals(tuple));

            obj = LogicSharp.deep_transitive_get(x, dict);
            Assert.IsInstanceOf(typeof(Tuple<object,object>), obj);
            var result = obj as Tuple<object, object>;
            Assert.IsNotNull(result);
            Assert.True(5.Equals(result.Item1));
            Assert.True(6.Equals(result.Item2));
            //    Transitive get that propagates within tuples
            //    >>> d = {1: (2, 3), 2: 12, 3: 13}
            //    >>> transitive_get(1, d)
            //    (2, 3)
            //    >>> deep_transitive_get(1, d)
            //    (12, 13)

            dict = new Dictionary<object, object>();
            tuple = new Tuple<object, object>(2,3);
            dict.Add(1, tuple);
            dict.Add(2,12);
            dict.Add(3,13);
            obj = LogicSharp.deep_transitive_get(1, dict);
            Assert.IsInstanceOf(typeof(Tuple<object,object>), obj);
            result = obj as Tuple<object, object>;
            Assert.IsNotNull(result);
            Assert.True(12.Equals(result.Item1));
            Assert.True(13.Equals(result.Item2));
        }

        [Test]
        public void TestLogicAny()
        {
            var x = new Var('x');
            var goal1 = new EqGoal(x, 2);
            
            var lst = new List<Goal>();
            lst.Add(goal1);
            var dict = new Dictionary<object, object>();
            object result = LogicSharp.logic_Any(lst, dict);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(IEnumerable<KeyValuePair<object,object>>), result);
            Assert.IsInstanceOf(typeof(Dictionary<object, object>), result);
            var resultDict = result as Dictionary<object, object>;
            Assert.IsTrue(resultDict.Count == 1);
            Assert.True(dict.ContainsKey(x));

            //assert len(tuple(lany(eq(x, 2), eq(x, 3))({}))) == 2
            //assert len(tuple(lany((eq, x, 2), (eq, x, 3))({}))) == 2

            var goal2 = new EqGoal(x, 3);
            lst = new List<Goal>();
            lst.Add(goal1);
            lst.Add(goal2);
            dict = new Dictionary<object, object>();
            result = LogicSharp.logic_Any(lst, dict);
            Assert.IsInstanceOf(typeof(IEnumerable<KeyValuePair<object, object>>), result);
            Assert.IsInstanceOf(typeof(HashSet<KeyValuePair<object, object>>), result);
            var myHashSet = result as HashSet<KeyValuePair<object, object>>;
            Assert.NotNull(myHashSet);
            Assert.True(myHashSet.Count == 2);
     
            //assert len(tuple(lany(eq(x, 2), eq(x, 3))({x:2}))) == 1
            lst = new List<Goal>();
            lst.Add(goal1);
            lst.Add(goal2);
            dict = new Dictionary<object, object>();
            dict.Add(x,2);
            result = LogicSharp.logic_Any(lst, dict);
            Assert.IsNull(result);
        }

        [Test]
        public void TestLogicAll()
        {
/*            x = var('x')
            assert results(lall((eq, x, 2))) == ({x: 2},)
            assert results(lall((eq, x, 2), (eq, x, 3))) == ()            */

            var x = new Var('x');
            var goal1 = new EqGoal(x, 2);
            var goal2 = new EqGoal(x, 3);

            var lst = new List<Goal>();
            lst.Add(goal1);
            lst.Add(goal2);
            var dict = new Dictionary<object, object>();
            object result = LogicSharp.logic_All(lst, dict);
            Assert.Null(result);

            /*assert results(lall((eq, x, 2), (eq, y, 3))) == ({x:2, y:3}) */
            var y = new Var('y');
            var goal3 = new EqGoal(y, 4);
            lst = new List<Goal>();
            lst.Add(goal1);
            lst.Add(goal3);
            dict = new Dictionary<object, object>();
            result = LogicSharp.logic_All(lst, dict);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(Dictionary<object, object>), result);
            var resultDict = result as Dictionary<object, object>;
            Assert.IsTrue(resultDict.Count == 2);
        }

        [Test]
        public void TestLogicConde()
        {
            /*            
             * x = var('x')
            assert results(conde([eq(x, 2)], [eq(x, 3)])) == ({x: 2}, {x: 3})
            assert results(conde([eq(x, 2), eq(x, 3)])) == ()
            */
            var x = new Var('x');
            var goal1 = new EqGoal(x, 2);
            var goal2 = new EqGoal(x, 3);
            var lst = new List<Goal>();
            lst.Add(goal1);
            var lst2 = new List<Goal>();
            lst2.Add(goal2);
            var lslst = new List<List<Goal>>();
            lslst.Add(lst);
            lslst.Add(lst2);

            var dict = new Dictionary<object, object>();
            object result = LogicSharp.logic_Conde(lslst,dict);
            Assert.IsInstanceOf(typeof(IEnumerable<KeyValuePair<object, object>>), result);
            Assert.IsInstanceOf(typeof(HashSet<KeyValuePair<object, object>>), result);
            var myHashSet = result as HashSet<KeyValuePair<object, object>>;
            Assert.NotNull(myHashSet);
            Assert.True(myHashSet.Count == 2);

            lst = new List<Goal>();
            lst.Add(goal1);
            lst.Add(goal2);
            lslst = new List<List<Goal>>();
            lslst.Add(lst);
            dict = new Dictionary<object, object>();
            result = LogicSharp.logic_Conde(lslst, dict);
            Assert.IsInstanceOf(typeof(IEnumerable<KeyValuePair<object, object>>), result);
            Assert.IsInstanceOf(typeof(HashSet<KeyValuePair<object, object>>), result);
            myHashSet = result as HashSet<KeyValuePair<object, object>>;
            Assert.IsEmpty(myHashSet); 
        }

        #region User Execution 

        [Test]
        public void TestRun()
        {
            var x = new Var('x');
            var goal = new EqGoal(x, 3);

            object result = LogicSharp.Run(x, goal);
            Assert.NotNull(result);
            Assert.True(result.Equals(3));
        }

        #endregion 
    }
}
