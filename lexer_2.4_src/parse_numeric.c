#define no_digits() do {yyerror("numeric literal without digits"); return 0;} while(0)

static int
parse_numeric(struct parser_params *parser, int c)
{

    int is_float, seen_point, seen_e, nondigit;
    int suffix;

    is_float = seen_point = seen_e = nondigit = 0;
    SET_LEX_STATE(EXPR_END);
    newtok();
    if(c == '-' || c == '+')
    {
        tokadd(c);
        c = nextc();
    }
    if(c == '0')
    {
        int start = toklen();
        c = nextc();
        if(c == 'x' || c == 'X')
        {
            /* hexadecimal */
            c = nextc();
            if(c != -1 && ISXDIGIT(c))
            {
                do
                {
                    if(c == '_')
                    {
                        if(nondigit)
                            break;
                        nondigit = c;
                        continue;
                    }
                    if(!ISXDIGIT(c))
                        break;
                    nondigit = 0;
                    tokadd(c);
                } while((c = nextc()) != -1);
            }
            pushback(c);
            tokfix();
            if(toklen() == start)
            {
                no_digits();
            }
            else if(nondigit)
                goto trailing_uc;
            suffix = number_literal_suffix(NUM_SUFFIX_ALL);
            return set_integer_literal(rb_cstr_to_inum(tok(), 16, FALSE), suffix);
        }
        if(c == 'b' || c == 'B')
        {
            /* binary */
            c = nextc();
            if(c == '0' || c == '1')
            {
                do
                {
                    if(c == '_')
                    {
                        if(nondigit)
                            break;
                        nondigit = c;
                        continue;
                    }
                    if(c != '0' && c != '1')
                        break;
                    nondigit = 0;
                    tokadd(c);
                } while((c = nextc()) != -1);
            }
            pushback(c);
            tokfix();
            if(toklen() == start)
            {
                no_digits();
            }
            else if(nondigit)
                goto trailing_uc;
            suffix = number_literal_suffix(NUM_SUFFIX_ALL);
            return set_integer_literal(rb_cstr_to_inum(tok(), 2, FALSE), suffix);
        }
        if(c == 'd' || c == 'D')
        {
            /* decimal */
            c = nextc();
            if(c != -1 && ISDIGIT(c))
            {
                do
                {
                    if(c == '_')
                    {
                        if(nondigit)
                            break;
                        nondigit = c;
                        continue;
                    }
                    if(!ISDIGIT(c))
                        break;
                    nondigit = 0;
                    tokadd(c);
                } while((c = nextc()) != -1);
            }
            pushback(c);
            tokfix();
            if(toklen() == start)
            {
                no_digits();
            }
            else if(nondigit)
                goto trailing_uc;
            suffix = number_literal_suffix(NUM_SUFFIX_ALL);
            return set_integer_literal(rb_cstr_to_inum(tok(), 10, FALSE), suffix);
        }
        if(c == '_')
        {
            /* 0_0 */
            goto octal_number;
        }
        if(c == 'o' || c == 'O')
        {
            /* prefixed octal */
            c = nextc();
            if(c == -1 || c == '_' || !ISDIGIT(c))
            {
                no_digits();
            }
        }
        if(c >= '0' && c <= '7')
        {
            /* octal */
          octal_number:
            do
            {
                if(c == '_')
                {
                    if(nondigit)
                        break;
                    nondigit = c;
                    continue;
                }
                if(c < '0' || c > '9')
                    break;
                if(c > '7')
                    goto invalid_octal;
                nondigit = 0;
                tokadd(c);
            } while((c = nextc()) != -1);
            if(toklen() > start)
            {
                pushback(c);
                tokfix();
                if(nondigit)
                    goto trailing_uc;
                suffix = number_literal_suffix(NUM_SUFFIX_ALL);
                return set_integer_literal(rb_cstr_to_inum(tok(), 8, FALSE), suffix);
            }
            if(nondigit)
            {
                pushback(c);
                goto trailing_uc;
            }
        }
        if(c > '7' && c <= '9')
        {
invalid_octal:
            yyerror("Invalid octal digit");
        }
        else if(c == '.' || c == 'e' || c == 'E')
        {
            tokadd('0');
        }
        else
        {
            pushback(c);
            suffix = number_literal_suffix(NUM_SUFFIX_ALL);
            return set_integer_literal(INT2FIX(0), suffix);
        }
    }

    for(;;)
    {
        switch (c)
        {
            case '0': case '1': case '2': case '3': case '4':
            case '5': case '6': case '7': case '8': case '9':
                nondigit = 0;
                tokadd(c);
                break;

            case '.':
                if(nondigit) goto trailing_uc;
                if(seen_point || seen_e)
                {
                    goto decode_num;
                }
                else
                {
                    int c0 = nextc();
                    if(c0 == -1 || !ISDIGIT(c0))
                    {
                        pushback(c0);
                        goto decode_num;
                    }
                    c = c0;
                }
                seen_point = toklen();
                tokadd('.');
                tokadd(c);
                is_float++;
                nondigit = 0;
                break;

            case 'e':
            case 'E':
                if(nondigit)
                {
                    pushback(c);
                    c = nondigit;
                    goto decode_num;
                }
                if(seen_e)
                {
                    goto decode_num;
                }
                nondigit = c;
                c = nextc();
                if(c != '-' && c != '+' && !ISDIGIT(c))
                {
                    pushback(c);
                    nondigit = 0;
                    goto decode_num;
                }
                tokadd(nondigit);
                seen_e++;
                is_float++;
                tokadd(c);
                nondigit = (c == '-' || c == '+') ? c : 0;
                break;

            case '_':        /* `_' in number just ignored */
                if(nondigit)
                    goto decode_num;
                nondigit = c;
                break;

            default:
                goto decode_num;
        }
        c = nextc();
    }

decode_num:

    pushback(c);
    if(nondigit)
    {
        char tmp[30];
trailing_uc:
        snprintf(tmp, sizeof(tmp), "trailing `%c' in number", nondigit);
        yyerror(tmp);
    }
    tokfix();
    if(is_float)
    {
        int type = tFLOAT;
        VALUE v;

        suffix = number_literal_suffix(seen_e ? NUM_SUFFIX_I : NUM_SUFFIX_ALL);
        if(suffix & NUM_SUFFIX_R)
        {
            type = tRATIONAL;
            v = parse_rational(parser, tok(), toklen(), seen_point);
        }
        else
        {
            double d = strtod(tok(), 0);
            if(errno == ERANGE)
            {
                rb_warning1("Float %s out of range", WARN_S(tok()));
                errno = 0;
            }
            v = DBL2NUM(d);
        }
        return set_number_literal(v, type, suffix);
    }
    suffix = number_literal_suffix(NUM_SUFFIX_ALL);
    return set_integer_literal(rb_cstr_to_inum(tok(), 10, FALSE), suffix);
}