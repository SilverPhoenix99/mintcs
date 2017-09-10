using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;
using Mint.Lex;

namespace Mint.Compilation.Components
{
    internal class StringContentCompiler : CompilerComponentBase
    {
        private string Content => Dedent(Node.Token.Text);

        public StringContentCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile() => String.Expressions.New(Constant(Content)).Cast<iObject>();

        private string Dedent(string content)
        {
            var dedent = Dedentation();
            if(dedent <= 0)
            {
                return content;
            }

            var lines = content.Split('\n').Select(line => Dedent(line, dedent));
            return string.Join("\n", lines);
        }

        private int Dedentation() => Node.Token.Properties.TryGetValue("dedent", out var value) ? (int) value : 0;

        private string Dedent(string line, int dedent)
        {
            if(dedent < 1)
            {
                return line;
            }

            var width = 0;
            for(var i = 0; i < line.Length; i++)
            {
                if(width == dedent)
                {
                    return line.Substring(i);
                }

                switch(line[i])
                {
                    case ' ':
                        width++;
                        break;

                    case '\t':
                        width = (width / Lexer.TabWidth + 1) * Lexer.TabWidth;
                        if(width > dedent)
                        {
                            return line.Substring(i);
                        }
                        break;

                    default:
                        return line.Substring(i);
                }
            }

            return "";
        }
    }
}