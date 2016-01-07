using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mint.Compiler
{
    interface iLiteral
    {
        uint BraceCount { get; set; }
        int ContentStart { get; set; }
        uint State { get; }
    }
}
