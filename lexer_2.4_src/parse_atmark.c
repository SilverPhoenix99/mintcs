static int
parse_atmark(struct parser_params *parser, const enum lex_state_e last_state)
{
    int result = tIVAR;
    register int c = nextc();

    newtok();
    tokadd('@');
    if(c == '@')
    {
        result = tCVAR;
        tokadd('@');
        c = nextc();
    }
    if(c == -1 || ISSPACE(c))
    {
        if(result == tIVAR)
        {
            compile_error(PARSER_ARG "`@' without identifiers is not allowed as an instance variable name");
        }
        else
        {
            compile_error(PARSER_ARG "`@@' without identifiers is not allowed as a class variable name");
        }
        return 0;
    }
    else if(ISDIGIT(c) || !parser_is_identchar())
    {
        pushback(c);
        if(result == tIVAR)
        {
            compile_error(PARSER_ARG "`@%c' is not allowed as an instance variable name", c);
        }
        else
        {
            compile_error(PARSER_ARG "`@@%c' is not allowed as a class variable name", c);
        }
        return 0;
    }

    if(tokadd_ident(parser, c))
        return 0;

    SET_LEX_STATE(EXPR_END);
    tokenize_ident(parser, last_state);
    return result;
}