using mint.Compiler;
using System;
using System.IO;
using System.Linq;

namespace mint.test
{
    class AstPrinter<T> : AstVisitor<T>
    {
        public AstPrinter(Ast<T> ast, TextWriter writer = null, int indent_size = 2)
        {
            Ast = ast;
            Writer = Writer ?? Console.Out;
            IndentSize = indent_size;
        }

        public Ast<T> Ast { get; }
        public TextWriter Writer { get; }
        public int Indent { get; private set; }
        public int IndentSize { get; }

        public void Print()
        {
            Ast.Accept(this);
        }

        public void Visit(AstNode<T> node)
        {
            WriteIndent();
            Writer.WriteLine(node.Value);
        }

        public void Visit(AstList<T> list)
        {
            WriteIndent();

            if(list.List.Count == 0)
            {
                Writer.WriteLine("{}");
                return;
            }

            Writer.WriteLine("{");
            Indent++;
            foreach(var ast in list.List)
            {
                ast.Accept(this);
            }
            Indent--;
            WriteIndent();
            Writer.WriteLine("}");
        }

        private void WriteIndent()
        {
            Writer.Write(new string(' ', IndentSize * Indent));
        }

        public static void Print(Ast<T> ast, TextWriter writer = null, int indent_size = 2)
        {
            new AstPrinter<T>(ast, writer, indent_size).Print();
        }
    }
}
