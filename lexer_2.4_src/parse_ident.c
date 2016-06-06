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

    if((IS_lex_state(EXPR_LABEL | EXPR_ENDFN) && !cmd_state)
    || IS_lex_state(EXPR_ARG | EXPR_CMDARG))
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

    if(IS_lex_state(EXPR_BEG | EXPR_MID | EXPR_CLASS | EXPR_ARG | EXPR_CMDARG | EXPR_DOT))
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

static int
lvar_defined_gen(struct parser_params *parser, ID id)
{
    return (dyna_in_block() && dvar_defined_get(id)) || local_id(id);
}

static int
dvar_defined_gen(struct parser_params *parser, ID id, int get)
{
    struct vtable *vars, *args, *used;
    int i;

    args = lvtbl->args;
    vars = lvtbl->vars;
    used = lvtbl->used;

    while(POINTER_P(vars))
    {
        if(vtable_included(args, id))
        {
            return 1;
        }
        if((i = vtable_included(vars, id)) != 0)
        {
            if(used)
                used->tbl[i-1] |= LVAR_USED;
            return 1;
        }
        args = args->prev;
        vars = vars->prev;
        if(get)
            used = 0;

        if(used)
            used = used->prev;
    }

    if(vars == DVARS_INHERIT)
    {
        return rb_dvar_defined(id, parser->base_block);
    }

    return 0;
}

static int
local_id_gen(struct parser_params *parser, ID id)
{
    struct vtable *vars, *args, *used;

    vars = lvtbl->vars;
    args = lvtbl->args;
    used = lvtbl->used;

    while(vars && POINTER_P(vars->prev))
    {
        vars = vars->prev;
        args = args->prev;
        if(used)
            used = used->prev;
    }

    if(vars && vars->prev == DVARS_INHERIT)
    {
        return rb_local_defined(id, parser->base_block);
    }

    if(vtable_included(args, id))
    {
        return 1;
    }

    int i = vtable_included(vars, id);
    if (i && used)
        used->tbl[i-1] |= LVAR_USED;
    return i != 0;
}
