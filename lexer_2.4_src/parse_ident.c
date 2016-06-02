static int
parse_ident(struct parser_params *parser, int c, int cmd_state)
{
    int result = 0;
    int mb = ENC_CODERANGE_7BIT;
    const enum lex_state_e last_state = lex_state;
    ID ident;

    do
    {
        if(!ISASCII(c))
            mb = ENC_CODERANGE_UNKNOWN;
        if(tokadd_mbchar(c) == -1)
            return 0;
        c = nextc();
    } while(parser_is_identchar());
    if((c == '!' || c == '?') && !peek('='))
    {
        tokadd(c);
    }
    else
    {
        pushback(c);
    }
    tokfix();

    if(toklast() == '!' || toklast() == '?')
    {
        result = tFID;
    }
    else
    {
        if(IS_lex_state(EXPR_FNAME))
        {
            register int c = nextc();
            if(c == '=' && !peek('~') && !peek('>') && (!peek('=') || (peek_n('>', 1))))
            {
                result = tIDENTIFIER;
                tokadd(c);
                tokfix();
            }
            else
            {
                pushback(c);
            }
        }
        if(result == 0 && ISUPPER(tok()[0]))
        {
            result = tCONSTANT;
        }
        else
        {
            result = tIDENTIFIER;
        }
    }

    if(IS_LABEL_POSSIBLE())
    {
        if(IS_LABEL_SUFFIX(0))
        {
            SET_LEX_STATE(EXPR_ARG|EXPR_LABELED);
            nextc();
            set_yylval_name(TOK_INTERN());
            return tLABEL;
        }
    }
    if(mb == ENC_CODERANGE_7BIT && !IS_lex_state(EXPR_DOT))
    {
        const struct kwtable *kw;

        /* See if it is a reserved word.  */
        kw = rb_reserved_word(tok(), toklen());
        if(kw)
        {
            enum lex_state_e state = lex_state;
            SET_LEX_STATE(kw->state);
            if(IS_lex_state_for(state, EXPR_FNAME))
            {
                set_yylval_name(rb_intern2(tok(), toklen()));
                return kw->id[0];
            }
            if(IS_lex_state(EXPR_BEG))
            {
                command_start = TRUE;
            }
            if(kw->id[0] == keyword_do)
            {
                if(lpar_beg && lpar_beg == paren_nest)
                {
                    lpar_beg = 0;
                    --paren_nest;
                    return keyword_do_LAMBDA;
                }
                if(COND_P())
                    return keyword_do_cond;
                if(CMDARG_P() && !IS_lex_state_for(state, EXPR_CMDARG))
                    return keyword_do_block;
                if(IS_lex_state_for(state, (EXPR_BEG | EXPR_ENDARG)))
                    return keyword_do_block;
                return keyword_do;
            }
            if(IS_lex_state_for(state, (EXPR_BEG | EXPR_LABELED)))
                return kw->id[0];
            else
            {
                if(kw->id[0] != kw->id[1])
                    SET_LEX_STATE(EXPR_BEG | EXPR_LABEL);
                return kw->id[1];
            }
        }
    }

    if(IS_lex_state(EXPR_BEG_ANY | EXPR_ARG_ANY | EXPR_DOT))
    {
        if(cmd_state)
        {
            SET_LEX_STATE(EXPR_CMDARG);
        }
        else
        {
            SET_LEX_STATE(EXPR_ARG);
        }
    }
    else if(lex_state == EXPR_FNAME)
    {
        SET_LEX_STATE(EXPR_ENDFN);
    }
    else
    {
        SET_LEX_STATE(EXPR_END);
    }

    ident = tokenize_ident(parser, last_state);
    if(!IS_lex_state_for(last_state, EXPR_DOT|EXPR_FNAME) &&
        (result == tIDENTIFIER) && /* not EXPR_FNAME, not attrasgn */
        lvar_defined(ident))
    {
        SET_LEX_STATE(EXPR_END|EXPR_LABEL);
    }
    return result;
}