using Mint.Lex.States.Delimiters;
using Mint.Parse;
using static Mint.Parse.TokenType;

namespace Mint.Lex.States
{
    internal partial class StringLiteral : Literal
    {
        private RegexpFlags regexpOptions = RegexpFlags.None;
        private bool emittedSpace;

        private Delimiter Delimiter { get; }

        private State EndState { get; }

        protected override char CurrentChar => Lexer.CurrentChar;

        private bool IsDelimiter => Lexer.CurrentChar == Delimiter.CloseDelimiter;

        public StringLiteral(Lexer lexer, int ts, int te, bool canLabel = false, State endState = null)
            : base(lexer, lexer.Position + 1)
        {
            var text = lexer.TextAt(ts, te);
            Delimiter = DelimiterFactory.CreateDelimiter(this, text);

            if(canLabel)
            {
                Delimiter.Features |= LiteralFeatures.Label;
            }

            EndState = endState ?? lexer.EndState;

            BeginToken = lexer.GenerateToken(Delimiter.BeginType, ts, te);
            BeginToken.Properties["has_interpolation"] = Delimiter.HasInterpolation;
        }

        protected override void EmitContent(int te)
        {

            if(contentStart == te)
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
                     : Delimiter.IsRegexp ? tREGEXP_END
                     : tSTRING_END;

            Lexer.EmitToken(type, ts, te);
            Lexer.PopLiteral();
            Lexer.CurrentState = EndState;
        }

        private void EmitSpace(int ts, int te)
        {
            if(!Delimiter.IsWords || emittedSpace)
            {
                return;
            }

            Lexer.EmitToken(tSPACE, ts, te);
            emittedSpace = true;
            contentStart = this.te;
        }
    }
}