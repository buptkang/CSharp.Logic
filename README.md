# CSharp.Logic
 
This project provides an embedded logic reasoning core for the .NET environment. It supports recursive substitution based on the logic rules. Its core derives from [miniKanren](https://github.com/miniKanren/miniKanren), a Scheme library for relational programming on which this library is based. In comparison to Prolog syntax, this project demonstrates a practical build approach to achieve First-Order-Logic which is based on the [Horn Clause](https://en.wikipedia.org/wiki/Horn_clause) knowledge representation. 

The core feature of this project is

1. Compute logic variables recursively through user-defined data structure.

2. Trace the computing path to direct the computation. 

## Code Examples

1. For the math arithmetic problems such as 1+2+3=?, 1-2*3=?, could the system both tell users the result and internal steps to derive that result: check out unit test code [ArithTest.cs](https://github.com/buptkang/CSharp.Logic/tree/master/Test/0.Logic.Arithmetic)

2. For the math algebraic problems, such as simplifying 2x+3y-2x+2z=?, could the system tell users what rules have been applied to derive the simplied version of the given expression: check out unit test code [AlgebraTest.cs](https://github.com/buptkang/CSharp.Logic/tree/master/Test/1.Logic.Algebra)
 
## Build upon CSharp.Logic

For geometry-based math problems, [Relation.Logic](https://github.com/buptkang/Relation.Logic) gives an example to use this project as the logical substitution core to solve higher-level knowledge reasoning problems.

## License

Copyright (c) 2015 Bo Kang

Licensed under the Apache License, Version 2.0 (the "License")
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
