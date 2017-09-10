namespace Mint.Parse
{
    public interface AstVisitor<out TRet>
    {
        TRet Visit(SyntaxNode node);
    }

    public interface AstVisitor
    {
        void Visit(SyntaxNode node);
    }
}
