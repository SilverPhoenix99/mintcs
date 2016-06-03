/*

All possible state combinations:

EXPR_ARG
EXPR_ARG | EXPR_LABELED
EXPR_BEG
EXPR_BEG | EXPR_LABEL
EXPR_CLASS
EXPR_CMDARG
EXPR_DOT
EXPR_END
EXPR_END | EXPR_LABEL
EXPR_ENDARG
EXPR_ENDFN
EXPR_ENDFN | EXPR_LABEL
EXPR_FNAME
EXPR_FNAME | EXPR_FITEM
EXPR_MID

*/


# define BITSTACK_PUSH(stack, n) ((stack) = ((stack)<<1)|((n)&1))
# define BITSTACK_POP(stack)     ((stack) = (stack) >> 1)
# define BITSTACK_LEXPOP(stack)  ((stack) = ((stack) >> 1) | ((stack) & 1))
# define BITSTACK_SET_P(stack)   ((stack)&1)
# define BITSTACK_SET(stack, n)  ((stack)=(n))

#define COND_PUSH(n)  BITSTACK_PUSH(cond_stack, (n))
#define COND_POP()    BITSTACK_POP(cond_stack)
#define COND_LEXPOP() BITSTACK_LEXPOP(cond_stack)
#define COND_P()      BITSTACK_SET_P(cond_stack)
#define COND_SET(n)   BITSTACK_SET(cond_stack, (n))

#define CMDARG_PUSH(n)  BITSTACK_PUSH(cmdarg_stack, (n))
#define CMDARG_POP()    BITSTACK_POP(cmdarg_stack)
#define CMDARG_LEXPOP() BITSTACK_LEXPOP(cmdarg_stack)
#define CMDARG_P()      BITSTACK_SET_P(cmdarg_stack)
#define CMDARG_SET(n)   BITSTACK_SET(cmdarg_stack, (n))

#define IS_lex_state_for(x, ls)     ((x) & (ls))
#define IS_lex_state_all_for(x, ls) (((x) & (ls)) == (ls))
#define IS_lex_state(ls)            IS_lex_state_for(lex_state, (ls))
#define IS_lex_state_all(ls)        IS_lex_state_all_for(lex_state, (ls))

// #define IS_ARG() IS_lex_state(EXPR_ARG | EXPR_CMDARG)
// #define IS_END() IS_lex_state(EXPR_END | EXPR_ENDARG | EXPR_ENDFN)
// #define IS_BEG() (IS_lex_state(EXPR_BEG | EXPR_MID | EXPR_CLASS) || IS_lex_state_all(EXPR_ARG|EXPR_LABELED))
// #define IS_SPCARG(c) (IS_lex_state(EXPR_ARG | EXPR_CMDARG) && space_seen && !ISSPACE(c))
// #define IS_LABEL_POSSIBLE() \
//     ((IS_lex_state(EXPR_LABEL | EXPR_ENDFN) && !cmd_state) || IS_lex_state(EXPR_ARG | EXPR_CMDARG))
// #define IS_AFTER_OPERATOR() IS_lex_state(EXPR_FNAME | EXPR_DOT)

// [ \t\n\v\f\r] ( includes '\n' )
#define ISSPACE(c) \
    (c == ' ' || ('\t' <= c && c <= '\r'))

// end_with? /:[^:]/
#define IS_LABEL_SUFFIX(n) \
    (peek_n(':',(n)) && !peek_n(':', (n)+1))

#define is_identchar(p,e,enc) \
    (rb_enc_isalnum((unsigned char)(*(p)),(enc)) || (*(p)) == '_' || !ISASCII(*(p)))

#define parser_is_identchar() \
    (!parser->eofp && is_identchar((lex_p-1),lex_pend,current_enc))

static int
parser_yylex(struct parser_params *parser)
{
    register int c;
    int space_seen = 0;
    int cmd_state;
    int label;
    enum lex_state_e last_state;
    int fallthru = FALSE;
    int token_seen = parser->token_seen;

    if(lex_strterm)
    {
        int token;
        if(nd_type(lex_strterm) == NODE_HEREDOC)
        {
            token = parser_here_document(parser, lex_strterm);
            if(token == tSTRING_END)
            {
                lex_strterm = 0;
                SET_LEX_STATE(EXPR_END);
            }
        }
        else
        {
            token = parse_string(lex_strterm);
            if((token == tSTRING_END) && (lex_strterm->nd_func & STR_FUNC_LABEL))
            {
                if(((IS_lex_state(EXPR_BEG | EXPR_ENDFN) && !COND_P()) || IS_lex_state(EXPR_ARG | EXPR_CMDARG)) &&
                    IS_LABEL_SUFFIX(0))
                {
                    nextc();
                    token = tLABEL_END;
                }
            }
            if(token == tSTRING_END || token == tREGEXP_END || token == tLABEL_END)
            {
                rb_gc_force_recycle((VALUE)lex_strterm);
                lex_strterm = 0;
                SET_LEX_STATE(token == tLABEL_END ? EXPR_BEG|EXPR_LABEL : EXPR_END);
            }
        }
        return token;
    }
    cmd_state = parser->command_start;
    parser->command_start = FALSE;
    parser->token_seen = TRUE;
retry:
    last_state = lex_state;
    switch (c = nextc())
    {
        case '\0': case '\004': case '\032': case -1:
        {
            return 0;
        }

        /* white spaces */
        case ' ': case '\t': case '\f': case '\r':
        case '\13': /* '\v' */
        {
            space_seen = 1;
            goto retry;
        }

        case '#':                /* it's a comment */
        {
            parser->token_seen = token_seen;
            /* no magic_comment in shebang line */
            if(!parser_magic_comment(parser, lex_p, lex_pend - lex_p)
            && comment_at_top(parser))
            {
                set_file_encoding(parser, lex_p, lex_pend);
            }
            lex_p = lex_pend;
            /* fall through */
        }

        case '\n': // done
        {
            parser->token_seen = token_seen;
            c = (IS_lex_state(EXPR_BEG|EXPR_CLASS|EXPR_FNAME|EXPR_DOT) && !IS_lex_state(EXPR_LABELED));
            if(c || IS_lex_state_all(EXPR_ARG|EXPR_LABELED))
            {
                if(!c && parser->in_kwarg)
                {
                    parser->command_start = TRUE;
                    SET_LEX_STATE(EXPR_BEG);
                    return '\n';
                }
                goto retry;
            }
            while ((c = nextc()))
            {
                switch (c)
                {
                    case ' ': case '\t': case '\f': case '\r':
                    case '\13': /* '\v' */
                        space_seen = 1;
                        break;
                    case '&':
                    case '.':
                    {
                        if(peek('.') == (c == '&')) // '&.' | '.' [^.]
                        {
                            pushback(c);
                            goto retry;
                        }
                    }
                    default:
                        --ruby_sourceline;
                        lex_nextline = lex_lastline;

                    case -1:                /* EOF no decrement*/
                        lex_goto_eol(parser);
                        parser->command_start = TRUE;
                        SET_LEX_STATE(EXPR_BEG);
                        return '\n';
                }
            }

            parser->command_start = TRUE;
            SET_LEX_STATE(EXPR_BEG);
            return '\n';
        }

        case '*':  // done
        {
            if((c = nextc()) == '*')
            {
                if((c = nextc()) == '=')
                {
                    set_yylval_id(tPOW);
                    SET_LEX_STATE(EXPR_BEG);
                    return tOP_ASGN;
                }
                pushback(c);
                if(IS_lex_state(EXPR_ARG | EXPR_CMDARG) && space_seen && !ISSPACE(c))
                {
                    c = tDSTAR;
                }
                else if(IS_lex_state(EXPR_BEG | EXPR_MID | EXPR_CLASS) || IS_lex_state_all(EXPR_ARG|EXPR_LABELED))
                {
                    c = tDSTAR;
                }
                else
                {
                    c = tPOW;
                }
            }
            else
            {
                if(c == '=')
                {
                    set_yylval_id('*');
                    SET_LEX_STATE(EXPR_BEG);
                    return tOP_ASGN;
                }
                pushback(c);
                if(IS_lex_state(EXPR_ARG | EXPR_CMDARG) && space_seen && !ISSPACE(c))
                {
                    c = tSTAR;
                }
                else if(IS_lex_state(EXPR_BEG | EXPR_MID | EXPR_CLASS) || IS_lex_state_all(EXPR_ARG|EXPR_LABELED))
                {
                    c = tSTAR;
                }
                else
                {
                    c = '*';
                }
            }
            SET_LEX_STATE(IS_lex_state(EXPR_FNAME | EXPR_DOT) ? EXPR_ARG : EXPR_BEG);
            return c;
        }

        case '!':  // done
        {
            c = nextc();
            if(IS_lex_state(EXPR_FNAME | EXPR_DOT))
            {
                SET_LEX_STATE(EXPR_ARG);
                if(c == '@')
                {
                    return '!';
                }
            }
            else
            {
                SET_LEX_STATE(EXPR_BEG);
            }
            if(c == '=')
            {
                return tNEQ;
            }
            if(c == '~')
            {
                return tNMATCH;
            }
            pushback(c);
            return '!';
        }

        case '=':  // done
        {
            if(was_bol())
            {
                /* skip embedded rd document */
                if(strncmp(lex_p, "begin", 5) == 0 && ISSPACE(lex_p[5]))
                {
                    int first_p = TRUE;

                    lex_goto_eol(parser);
                    for (;;)
                    {
                        lex_goto_eol(parser);
                        first_p = FALSE;
                        c = nextc();
                        if(c == -1)
                        {
                            compile_error(PARSER_ARG "embedded document meets end of file");
                            return 0;
                        }
                        if(c != '=') continue;
                        if(c == '=' && strncmp(lex_p, "end", 3) == 0 &&
                            (lex_p + 3 == lex_pend || ISSPACE(lex_p[3])))
                        {
                            break;
                        }
                    }
                    lex_goto_eol(parser);
                    goto retry;
                }
            }

            SET_LEX_STATE(IS_lex_state(EXPR_FNAME | EXPR_DOT) ? EXPR_ARG : EXPR_BEG);
            if((c = nextc()) == '=')
            {
                if((c = nextc()) == '=')
                {
                    return tEQQ;
                }
                pushback(c);
                return tEQ;
            }
            if(c == '~')
            {
                return tMATCH;
            }
            else if(c == '>')
            {
                return tASSOC;
            }
            pushback(c);
            return '=';
        }

        case '<':
        {
            last_state = lex_state;
            c = nextc();
            if(c == '<' &&
                !IS_lex_state(EXPR_DOT | EXPR_CLASS) &&
                !IS_lex_state(EXPR_END | EXPR_ENDARG | EXPR_ENDFN) &&
                (!IS_lex_state(EXPR_ARG | EXPR_CMDARG) || IS_lex_state(EXPR_LABELED) || space_seen))
            {
                int token = heredoc_identifier();
                if(token) return token;
            }
            if(IS_lex_state(EXPR_FNAME | EXPR_DOT))
            {
                SET_LEX_STATE(EXPR_ARG);
            }
            else
            {
                if(IS_lex_state(EXPR_CLASS))
                    parser->command_start = TRUE;
                SET_LEX_STATE(EXPR_BEG);
            }
            if(c == '=')
            {
                if((c = nextc()) == '>')
                {
                    return tCMP;
                }
                pushback(c);
                return tLEQ;
            }
            if(c == '<')
            {
                if((c = nextc()) == '=')
                {
                    set_yylval_id(tLSHFT);
                    SET_LEX_STATE(EXPR_BEG);
                    return tOP_ASGN;
                }
                pushback(c);
                return tLSHFT;
            }
            pushback(c);
            return '<';
        }

        case '>':
        {
            SET_LEX_STATE(IS_lex_state(EXPR_FNAME | EXPR_DOT) ? EXPR_ARG : EXPR_BEG);
            if((c = nextc()) == '=')
            {
                return tGEQ;
            }
            if(c == '>')
            {
                if((c = nextc()) == '=')
                {
                    set_yylval_id(tRSHFT);
                    SET_LEX_STATE(EXPR_BEG);
                    return tOP_ASGN;
                }
                pushback(c);
                return tRSHFT;
            }
            pushback(c);
            return '>';
        }

        case '"':
        {
            label =
                ((IS_lex_state(EXPR_LABEL | EXPR_ENDFN) && !cmd_state) || IS_lex_state(EXPR_ARG | EXPR_CMDARG))
                ? str_label
                : 0;
            lex_strterm = NEW_STRTERM(str_dquote | label, '"', 0);
            return tSTRING_BEG;
        }

        case '`':
        {
            if(IS_lex_state(EXPR_FNAME))
            {
                SET_LEX_STATE(EXPR_ENDFN);
                return c;
            }
            if(IS_lex_state(EXPR_DOT))
            {
                if(cmd_state)
                    SET_LEX_STATE(EXPR_CMDARG);
                else
                    SET_LEX_STATE(EXPR_ARG);
                return c;
            }
            lex_strterm = NEW_STRTERM(str_xquote, '`', 0);
            return tXSTRING_BEG;
        }

        case '\'':
        {
            label =
                ((IS_lex_state(EXPR_LABEL | EXPR_ENDFN) && !cmd_state) || IS_lex_state(EXPR_ARG | EXPR_CMDARG))
                ? str_label
                : 0
            ;
            lex_strterm = NEW_STRTERM(str_squote | label, '\'', 0);
            return tSTRING_BEG;
        }

        case '?':
            return parse_qmark(parser);

        case '&':
        {
            if((c = nextc()) == '&')
            {
                SET_LEX_STATE(EXPR_BEG);
                if((c = nextc()) == '=')
                {
                    set_yylval_id(tANDOP);
                    SET_LEX_STATE(EXPR_BEG);
                    return tOP_ASGN;
                }
                pushback(c);
                return tANDOP;
            }
            else if(c == '=')
            {
                set_yylval_id('&');
                SET_LEX_STATE(EXPR_BEG);
                return tOP_ASGN;
            }
            else if(c == '.')
            {
                SET_LEX_STATE(EXPR_DOT);
                return tANDDOT;
            }
            pushback(c);
            if(IS_lex_state(EXPR_ARG | EXPR_CMDARG) && space_seen && !ISSPACE(c))
            {
                c = tAMPER;
            }
            else if(IS_lex_state(EXPR_BEG | EXPR_MID | EXPR_CLASS) || IS_lex_state_all(EXPR_ARG|EXPR_LABELED))
            {
                c = tAMPER;
            }
            else
            {
                c = '&';
            }
            SET_LEX_STATE(IS_lex_state(EXPR_FNAME | EXPR_DOT) ? EXPR_ARG : EXPR_BEG);
            return c;
        }

        case '|':
        {
            if((c = nextc()) == '|')
            {
                SET_LEX_STATE(EXPR_BEG);
                if((c = nextc()) == '=')
                {
                    set_yylval_id(tOROP);
                    SET_LEX_STATE(EXPR_BEG);
                    return tOP_ASGN;
                }
                pushback(c);
                return tOROP;
            }
            if(c == '=')
            {
                set_yylval_id('|');
                SET_LEX_STATE(EXPR_BEG);
                return tOP_ASGN;
            }
            SET_LEX_STATE(IS_lex_state(EXPR_FNAME | EXPR_DOT) ? EXPR_ARG : EXPR_BEG|EXPR_LABEL);
            pushback(c);
            return '|';
        }

        case '+':
        {
            c = nextc();
            if(IS_lex_state(EXPR_FNAME | EXPR_DOT))
            {
                SET_LEX_STATE(EXPR_ARG);
                if(c == '@')
                {
                    return tUPLUS;
                }
                pushback(c);
                return '+';
            }
            if(c == '=')
            {
                set_yylval_id('+');
                SET_LEX_STATE(EXPR_BEG);
                return tOP_ASGN;
            }
            if((IS_lex_state(EXPR_BEG | EXPR_MID | EXPR_CLASS) || IS_lex_state_all(EXPR_ARG|EXPR_LABELED))
            || (IS_lex_state(EXPR_ARG | EXPR_CMDARG) && space_seen && !ISSPACE(c)))
            {
                SET_LEX_STATE(EXPR_BEG);
                pushback(c);
                if(c != -1 && ISDIGIT(c))
                {
                    return parse_numeric(parser, '+');
                }
                return tUPLUS;
            }
            SET_LEX_STATE(EXPR_BEG);
            pushback(c);
            return '+';
        }

        case '-':
        {
            c = nextc();
            if(IS_lex_state(EXPR_FNAME | EXPR_DOT))
            {
                SET_LEX_STATE(EXPR_ARG);
                if(c == '@')
                {
                    return tUMINUS;
                }
                pushback(c);
                return '-';
            }
            if(c == '=')
            {
                set_yylval_id('-');
                SET_LEX_STATE(EXPR_BEG);
                return tOP_ASGN;
            }
            if(c == '>')
            {
                SET_LEX_STATE(EXPR_ENDFN);
                token_info_push("->");
                return tLAMBDA;
            }
            if((IS_lex_state(EXPR_BEG | EXPR_MID | EXPR_CLASS) || IS_lex_state_all(EXPR_ARG|EXPR_LABELED))
            || (IS_lex_state(EXPR_ARG | EXPR_CMDARG) && space_seen && !ISSPACE(c)))
            {
                SET_LEX_STATE(EXPR_BEG);
                pushback(c);
                if(c != -1 && ISDIGIT(c))
                {
                    return tUMINUS_NUM;
                }
                return tUMINUS;
            }
            SET_LEX_STATE(EXPR_BEG);
            pushback(c);
            return '-';
        }

        case '.':
        {
            SET_LEX_STATE(EXPR_BEG);
            if((c = nextc()) == '.')
            {
                if((c = nextc()) == '.')
                {
                    return tDOT3;
                }
                pushback(c);
                return tDOT2;
            }
            pushback(c);
            if(c != -1 && ISDIGIT(c))
            {
                yyerror("no .<digit> floating literal anymore; put 0 before dot");
            }
            SET_LEX_STATE(EXPR_DOT);
            return '.';
        }

        case '0': case '1': case '2': case '3': case '4':
        case '5': case '6': case '7': case '8': case '9':
            return parse_numeric(parser, c);

        case ')':
        case ']':
            paren_nest--;
        case '}':
        {
            COND_LEXPOP();
            CMDARG_LEXPOP();
            if(c == ')')
                SET_LEX_STATE(EXPR_ENDFN);
            else
                SET_LEX_STATE(EXPR_ENDARG);
            if(c == '}')
            {
                if(!brace_nest--) c = tSTRING_DEND;
            }
            return c;
        }

        case ':':
        {
            c = nextc();
            if(c == ':')
            {
                if((IS_lex_state(EXPR_BEG | EXPR_MID | EXPR_CLASS) || IS_lex_state_all(EXPR_ARG|EXPR_LABELED))
                || IS_lex_state(EXPR_CLASS)
                || (IS_lex_state(EXPR_ARG | EXPR_CMDARG) && space_seen && !ISSPACE(-1)))
                {
                    SET_LEX_STATE(EXPR_BEG);
                    return tCOLON3;
                }
                SET_LEX_STATE(EXPR_DOT);
                return tCOLON2;
            }
            if(IS_lex_state(EXPR_END | EXPR_ENDARG | EXPR_ENDFN) || ISSPACE(c) || c == '#')
            {
                pushback(c);
                SET_LEX_STATE(EXPR_BEG);
                return ':';
            }
            switch (c)
            {
              case '\'':
                lex_strterm = NEW_STRTERM(str_ssym, c, 0);
                break;
              case '"':
                lex_strterm = NEW_STRTERM(str_dsym, c, 0);
                break;
              default:
                pushback(c);
                break;
            }
            SET_LEX_STATE(EXPR_FNAME);
            return tSYMBEG;
        }

        case '/':
        {
            if(IS_lex_state(EXPR_BEG | EXPR_MID | EXPR_CLASS) || IS_lex_state_all(EXPR_ARG|EXPR_LABELED))
            {
                lex_strterm = NEW_STRTERM(str_regexp, '/', 0);
                return tREGEXP_BEG;
            }
            if((c = nextc()) == '=')
            {
                set_yylval_id('/');
                SET_LEX_STATE(EXPR_BEG);
                return tOP_ASGN;
            }
            pushback(c);
            if(IS_lex_state(EXPR_ARG | EXPR_CMDARG) && space_seen && !ISSPACE(c))
            {
                lex_strterm = NEW_STRTERM(str_regexp, '/', 0);
                return tREGEXP_BEG;
            }
            SET_LEX_STATE(IS_lex_state(EXPR_FNAME | EXPR_DOT) ? EXPR_ARG : EXPR_BEG);
            return '/';
        }

        case '^':
        {
            if((c = nextc()) == '=')
            {
                set_yylval_id('^');
                SET_LEX_STATE(EXPR_BEG);
                return tOP_ASGN;
            }
            SET_LEX_STATE(IS_lex_state(EXPR_FNAME | EXPR_DOT) ? EXPR_ARG : EXPR_BEG);
            pushback(c);
            return '^';
        }

        case ';':
        {
            SET_LEX_STATE(EXPR_BEG);
            parser->command_start = TRUE;
            return ';';
        }

        case ',':
        {
            SET_LEX_STATE(EXPR_BEG|EXPR_LABEL);
            return ',';
        }

        case '~':
        {
            if(IS_lex_state(EXPR_FNAME | EXPR_DOT))
            {
                if((c = nextc()) != '@')
            {
                    pushback(c);
                }
                SET_LEX_STATE(EXPR_ARG);
            }
            else
            {
                SET_LEX_STATE(EXPR_BEG);
            }
            return '~';
        }

        case '(':
        {
            if(IS_lex_state(EXPR_BEG | EXPR_MID | EXPR_CLASS) || IS_lex_state_all(EXPR_ARG|EXPR_LABELED))
            {
                c = tLPAREN;
            }
            else if(IS_lex_state(EXPR_ARG | EXPR_CMDARG) && space_seen && !ISSPACE(-1))
            {
                c = tLPAREN_ARG;
            }
            paren_nest++;
            COND_PUSH(0);
            CMDARG_PUSH(0);
            SET_LEX_STATE(EXPR_BEG|EXPR_LABEL);
            return c;
        }

        case '[':
        {
            paren_nest++;
            if(IS_lex_state(EXPR_FNAME | EXPR_DOT))
            {
                SET_LEX_STATE(EXPR_ARG);
                if((c = nextc()) == ']')
                {
                    if((c = nextc()) == '=')
                    {
                        return tASET;
                    }
                    pushback(c);
                    return tAREF;
                }
                pushback(c);
                lex_state |= EXPR_LABEL;
                return '[';
            }
            else if(IS_lex_state(EXPR_BEG | EXPR_MID | EXPR_CLASS) || IS_lex_state_all(EXPR_ARG|EXPR_LABELED))
            {
                c = tLBRACK;
            }
            else if(IS_lex_state(EXPR_ARG | EXPR_CMDARG) && (space_seen || IS_lex_state(EXPR_LABELED)))
            {
                c = tLBRACK;
            }
            SET_LEX_STATE(EXPR_BEG|EXPR_LABEL);
            COND_PUSH(0);
            CMDARG_PUSH(0);
            return c;
        }

        case '{':
        {
            ++brace_nest;
            if(lpar_beg && lpar_beg == paren_nest)
            {
                SET_LEX_STATE(EXPR_BEG);
                lpar_beg = 0;
                --paren_nest;
                COND_PUSH(0);
                CMDARG_PUSH(0);
                return tLAMBEG;
            }
            if(IS_lex_state(EXPR_LABELED))
                c = tLBRACE;      /* hash */
            else if(IS_lex_state(EXPR_ARG | EXPR_CMDARG | EXPR_END | EXPR_ENDFN))
                c = '{';          /* block (primary) */
            else if(IS_lex_state(EXPR_ENDARG))
                c = tLBRACE_ARG;  /* block (expr) */
            else
                c = tLBRACE;      /* hash */
            COND_PUSH(0);
            CMDARG_PUSH(0);
            SET_LEX_STATE(EXPR_BEG);
            if(c != tLBRACE_ARG)
                lex_state |= EXPR_LABEL;
            if(c != tLBRACE)
                parser->command_start = TRUE;
            return c;
        }

        case '\\':
        {
            c = nextc();
            if(c == '\n')
            {
                space_seen = 1;
                goto retry; /* skip \\n */
            }
            pushback(c);
            return '\\';
        }

        case '%':
            return parse_percent(parser, space_seen, last_state);

        case '$':
            return parse_gvar(parser, last_state);

        case '@':
            return parse_atmark(parser, last_state);

        case '_':
        {
            if(was_bol() && whole_match_p("__END__", 7, 0))
            {
                ruby__end__seen = 1;
                parser->eofp = 1;
                return -1;
            }
            newtok();
            break;
        }

        default:
        {
            if(!parser_is_identchar())
            {
                compile_error(PARSER_ARG  "Invalid char `\\x%02X' in expression", c);
                goto retry;
            }

            newtok();
            break;
        }
    }

    return parse_ident(parser, c, cmd_state);
}
