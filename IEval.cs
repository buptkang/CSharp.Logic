using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpLogic
{
    public interface IEval
    {
        object Eval();

        void UnEval();
    }
}
