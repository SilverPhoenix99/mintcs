static int
parse_percent(struct parser_params *parser, const int space_seen, const enum lex_state_e last_state)
{
    register int c;

    if(IS_lex_state(EXPR_BEG | EXPR_MID | EXPR_CLASS) || IS_lex_state_all(EXPR_ARG|EXPR_LABELED))
    {
        int term;
        int paren;

        c = nextc();
quotation:
        if(c == -1 || !ISALNUM(c))
        {
            term = c;
            c = 'Q';
        }
        else
        {
            term = nextc();
            if(rb_enc_isalnum(term, current_enc) || !parser_isascii())
            {
                yyerror("unknown type of %string");
                return 0;
            }
        }
        if(c == -1 || term == -1)
        {
            compile_error(PARSER_ARG "unterminated quoted string meets end of file");
            return 0;
        }
        paren = term;
        if(term == '(')
            term = ')';
        else if(term == '[')
            term = ']';
        else if(term == '{')
            term = '}';
        else if(term == '<')
            term = '>';
        else
            paren = 0;

        switch (c)
        {
            case 'Q':
                lex_strterm = NEW_STRTERM(str_dquote, term, paren);
                return tSTRING_BEG;

            case 'q':
                lex_strterm = NEW_STRTERM(str_squote, term, paren);
                return tSTRING_BEG;

            case 'W':
                lex_strterm = NEW_STRTERM(str_dword, term, paren);
                do
                {
                    c = nextc();
                } while(ISSPACE(c));
                pushback(c);
                return tWORDS_BEG;

            case 'w':
                lex_strterm = NEW_STRTERM(str_sword, term, paren);
                do
                {
                    c = nextc();
                } while(ISSPACE(c));
                pushback(c);
                return tQWORDS_BEG;

            case 'I':
                lex_strterm = NEW_STRTERM(str_dword, term, paren);
                do
                {
                    c = nextc();
                } while(ISSPACE(c));
                pushback(c);
                return tSYMBOLS_BEG;

            case 'i':
                lex_strterm = NEW_STRTERM(str_sword, term, paren);
                do
                {
                    c = nextc();
                } while(ISSPACE(c));
                pushback(c);
                return tQSYMBOLS_BEG;

            case 'x':
                lex_strterm = NEW_STRTERM(str_xquote, term, paren);
                return tXSTRING_BEG;

            case 'r':
                lex_strterm = NEW_STRTERM(str_regexp, term, paren);
                return tREGEXP_BEG;

            case 's':
                lex_strterm = NEW_STRTERM(str_ssym, term, paren);
                SET_LEX_STATE(EXPR_FNAME|EXPR_FITEM);
                return tSYMBEG;

            default:
                yyerror("unknown type of %string");
                return 0;
        }
    }
    if((c = nextc()) == '=')
    {
        set_yylval_id('%');
        SET_LEX_STATE(EXPR_BEG);
        return tOP_ASGN;
    }

    if((IS_lex_state(EXPR_ARG | EXPR_CMDARG) && space_seen && !ISSPACE(c))
    || (IS_lex_state(EXPR_FITEM) && c == 's'))
    {
        goto quotation;
    }

    SET_LEX_STATE(IS_lex_state(EXPR_FNAME | EXPR_DOT) ? EXPR_ARG : EXPR_BEG);
    pushback(c);
    warn_balanced("%%", "string literal");
    return '%';
}