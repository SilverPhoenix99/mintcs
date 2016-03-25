namespace Mint.Parse
{
    public interface AstVisitor<T, out TRet>
    {
        TRet Visit(Ast<T> node);
    }

    public interface AstVisitor<T>
    {
        void Visit(Ast<T> node);
    }
}
