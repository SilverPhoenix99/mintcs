﻿namespace Mint.Parser
{
    public interface AstVisitor<T, TRet>
    {
        TRet Visit(Ast<T> node);
    }

    public interface AstVisitor<T>
    {
        void Visit(Ast<T> node);
    }
}