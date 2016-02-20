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

    public partial class LogicSharp
    {
        public static bool IsInt(object expression, out int number)
        {
            if (expression == null)
            {
                number = 0;
                return false;
            }

            return Int32.TryParse(Convert.ToString(expression,
                System.Globalization.CultureInfo.InvariantCulture),
                System.Globalization.NumberStyles.Any,
                System.Globalization.NumberFormatInfo.InvariantInfo,
                out number);
        }

        public static bool IsDouble(object expression, out double number)
        {
            if (expression == null)
            {
                number = 0.0;
                return false;
            }

            return Double.TryParse(Convert.ToString(expression,
                System.Globalization.CultureInfo.InvariantCulture),
                System.Globalization.NumberStyles.Any,
                System.Globalization.NumberFormatInfo.InvariantInfo,
                out number);
        }

        public static bool IsNumeric(object obj)
        {
            var term = obj as Term;
            if (term != null) return false;

            bool result;
            try
            {
                int inum;
                result = IsInt(obj, out inum);
                if (result)
                {
                    return true;
                }
            }
            catch (Exception)
            {
            }

            try
            {
                double dnum;
                result = IsDouble(obj, out dnum);
                if (result)
                {
                    return true;
                }
            }
            catch (Exception)
            {
            }

            return false;
        }

        public static bool NumericApproximateEqual(object obj1, object obj2)
        {
            bool result = NumericEqual(obj1, obj2);
            if (result) return true;

            double dNum1, dNum2;
            bool bool1 = IsDouble(obj1, out dNum1);
            bool bool2 = IsDouble(obj2, out dNum2);

            if (bool1 && bool2)
            {
                dNum1 = Math.Round(dNum1, 1);
                dNum2 = Math.Round(dNum2, 1);
                double gap = Math.Abs(dNum1 - dNum2);
                if (gap < 0.00001) return true;
            }
            return false;
        }

        public static bool NumericEqual(object obj1, object obj2)
        {
            double dNum1, dNum2;
            bool bool1 = IsDouble(obj1, out dNum1);
            bool bool2 = IsDouble(obj2, out dNum2);

            if (bool1 && bool2)
            {
                double gap = Math.Abs(dNum1 - dNum2);
                if (gap < 0.00001) return true;
            }
            return false;
        }

        public static object CheckObj(object obj)
        {
            int iNum;
            bool bool1 = IsInt(obj, out iNum);
            if (bool1) return iNum;
            return obj;
        }
    }
}
