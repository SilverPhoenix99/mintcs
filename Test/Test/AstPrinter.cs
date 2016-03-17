using Mint.Parser;
using System;
using System.IO;
using System.Linq;

namespace Test
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

        public void Visit(Ast<T> node)
        {
            if(node.List.Count == 0)
            {
                WriteLine(node.Value == null ? (object) "[]" : node.Value);
                return;
            }
           
            if(node.Value != null)
            {
                Write(node.Value);
                Write(" ");
            }

            WriteLine("[");
            Indent++;

            foreach(var child in node.List)
            {
                child.Accept(this);
            }

            Indent--;
            WriteLine("]");            
        }
        
        private void Write<V>(V obj)
        {
            WriteIndent();
            Writer.Write(obj);
        }

        private void WriteLine<V>(V obj)
        {
            WriteIndent();
            Writer.WriteLine(obj);
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
