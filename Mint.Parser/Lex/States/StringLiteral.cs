using Mint.Lex.States.Delimiters;
using Mint.Parse;
using static Mint.Parse.TokenType;

namespace Mint.Lex.States
{
    internal partial class StringLiteral : Literal
    {
        private readonly Delimiter delimiter;
        private RegexpFlags regexpOptions = RegexpFlags.None;
        private bool emittedSpace;
        private readonly State endState;


        public StringLiteral(Lexer lexer, int ts, int te, bool canLabel = false, State endState = null)
            : base(lexer, lexer.Position + 1)
        {
            var text = lexer.TextAt(ts, te);
            delimiter = DelimiterFactory.CreateDelimiter(this, text);

            if(canLabel)
            {
                delimiter.Features |= LiteralFeatures.Label;
            }

            this.endState = endState ?? lexer.EndState;

            BeginToken = lexer.GenerateToken(delimiter.BeginType, ts, te);
            BeginToken.Properties["has_interpolation"] = delimiter.HasInterpolation;
        }


        protected override char CurrentChar => Lexer.CurrentChar;
        private bool IsDelimiter => Lexer.CurrentChar == delimiter.CloseDelimiter;


        protected override void EmitContent(int te)
        {
            if(delimiter.IsWords && contentStart == te)
            {
                return;
            }

            base.EmitContent(te);
            emittedSpace = false;
        }


        protected override void EmitDBeg()
        {
            base.EmitDBeg();
            emittedSpace = false;
        }


        protected override void EmitDVar(TokenType type)
        {
            base.EmitDVar(type);
            emittedSpace = false;
        }


        private void EmitEndToken(int ts, int te)
        {
            EmitSpace(ts, ts);

            var type = Lexer.Data[te - 1] == ':' ? tLABEL_END
                     : delimiter.IsRegexp ? tREGEXP_END
                     : tSTRING_END;

            Lexer.EmitToken(type, ts, te);
            Lexer.PopLiteral();
            Lexer.CurrentState = endState;
        }


        private void EmitSpace(int ts, int te)
        {
            if(!delimiter.IsWords || emittedSpace)
            {
                return;
            }

            Lexer.EmitToken(tSPACE, ts, te);
            emittedSpace = true;
            contentStart = this.te;
        }
    }
}