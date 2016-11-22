﻿using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mint.Compilation.Scopes
{
    public interface Scope
    {
        Compiler Compiler { get; }

        Scope Parent { get; }

        CompilerClosure Closure { get; }

        Expression Nesting { get; }
    }
}
