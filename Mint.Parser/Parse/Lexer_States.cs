namespace Mint.Parse
{
	public partial class Lexer
	{
		public enum States
		{
			BOF                  = Lex_en_BOF,
			EXPR_ARG             = Lex_en_EXPR_ARG,
			EXPR_BEG             = Lex_en_EXPR_BEG,
			EXPR_CLASS           = Lex_en_EXPR_CLASS,
			EXPR_CMDARG          = Lex_en_EXPR_CMDARG,
			EXPR_DOT             = Lex_en_EXPR_DOT,
			EXPR_END             = Lex_en_EXPR_END,
			EXPR_ENDARG          = Lex_en_EXPR_ENDARG,
			EXPR_ENDFN           = Lex_en_EXPR_ENDFN,
			EXPR_FNAME           = Lex_en_EXPR_FNAME,
			EXPR_LABELARG        = Lex_en_EXPR_LABELARG,
			EXPR_MID             = Lex_en_EXPR_MID,
			COMMON_EXPR          = Lex_en_COMMON_EXPR,
			HEREDOC_DELIMITER    = Lex_en_HEREDOC_DELIMITER,
			HEREDOC_CONTENT      = Lex_en_HEREDOC_CONTENT,
			STRING_LITERAL       = Lex_en_STRING_LITERAL,
            STRING_INTERPOLATION = Lex_en_STRING_INTERPOLATION,
			REGEXP_END           = Lex_en_REGEXP_END,
			CHAR                 = Lex_en_CHAR
		};
	}
}
