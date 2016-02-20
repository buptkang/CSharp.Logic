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
    public enum RepresentationType
    {
        Explicit, Implicit, Parametric
    }

    public enum CoordinateSystemType
    {
        Cartesian, Polar
    }
 
    public enum ShapeType
    {
        Point = 0,
        Line = 1,
        LineSegment = 2,
        Circle = 3,
        PointLine = 4,
        TwoLines = 5,
        None = -1       
    }
}
