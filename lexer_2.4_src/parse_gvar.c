static int
parse_gvar(struct parser_params *parser, const enum lex_state_e last_state)
{
    register int c;

    SET_LEX_STATE(EXPR_END);
    newtok();
    c = nextc();
    switch(c)
    {
        case '_':                /* $_: last read line string */
            c = nextc();
            if(parser_is_identchar())
            {
                tokadd('$');
                tokadd('_');
                break;
            }
            pushback(c);
            c = '_';
            /* fall through */
        case '~':                /* $~: match-data */
        case '*':                /* $*: argv */
        case '$':                /* $$: pid */
        case '?':                /* $?: last status */
        case '!':                /* $!: error string */
        case '@':                /* $@: error position */
        case '/':                /* $/: input record separator */
        case '\\':                /* $\: output record separator */
        case ';':                /* $;: field separator */
        case ',':                /* $,: output field separator */
        case '.':                /* $.: last read line number */
        case '=':                /* $=: ignorecase */
        case ':':                /* $:: load path */
        case '<':                /* $<: reading filename */
        case '>':                /* $>: default output handle */
        case '\"':                /* $": already loaded files */
            tokadd('$');
            tokadd(c);
            goto gvar;

        case '-':
            tokadd('$');
            tokadd(c);
            c = nextc();
            if(parser_is_identchar())
            {
                if(tokadd_mbchar(c) == -1) return 0;
            }
            else
            {
                pushback(c);
                pushback('-');
                return '$';
            }
gvar:
            set_yylval_name(TOK_INTERN());
            return tGVAR;

        case '&':                /* $&: last match */
        case '`':                /* $`: string before last match */
        case '\'':                /* $': string after last match */
        case '+':                /* $+: string matches last paren. */
            if(IS_lex_state_for(last_state, EXPR_FNAME))
            {
                tokadd('$');
                tokadd(c);
                goto gvar;
            }
            set_yylval_node(NEW_BACK_REF(c));
            return tBACK_REF;

        case '1': case '2': case '3':
        case '4': case '5': case '6':
        case '7': case '8': case '9':
            tokadd('$');
            do
            {
                tokadd(c);
                c = nextc();
            } while(c != -1 && ISDIGIT(c));
            pushback(c);

            if(IS_lex_state_for(last_state, EXPR_FNAME))
                goto gvar;

            tokfix();
            set_yylval_node(NEW_NTH_REF(parse_numvar(parser)));
            return tNTH_REF;

        default:
            if(!parser_is_identchar())
            {
                if(c == -1 || ISSPACE(c))
                {
                    compile_error(PARSER_ARG "`$' without identifiers is not allowed as a global variable name");
                }
                else
                {
                    pushback(c);
                    compile_error(PARSER_ARG "`$%c' is not allowed as a global variable name", c);
                }
                return 0;
            }

        case '0':
            tokadd('$');
    }

    if(tokadd_ident(parser, c))
        return 0;

    SET_LEX_STATE(EXPR_END);
    tokenize_ident(parser, last_state);
    return tGVAR;
}