#define str_copy(_s, _p, _n) \
    ((_s) \
        ? (void)(rb_str_resize((_s), (_n)), MEMCPY(RSTRING_PTR(_s), (_p), char, (_n)), (_s)) \
        : (void)((_s) = STR_NEW((_p), (_n))))

typedef void (*rb_magic_comment_setter_t)(struct parser_params *parser, const char *name, const char *val);
typedef long (*rb_magic_comment_length_t)(struct parser_params *parser, const char *name, long len);

struct magic_comment
{
    const char *name;
    rb_magic_comment_setter_t func;
    rb_magic_comment_length_t length;
};

static const struct magic_comment magic_comments[] =
{
    {"coding", magic_comment_encoding, parser_encode_length},
    {"encoding", magic_comment_encoding, parser_encode_length},
    {"frozen_string_literal", parser_set_compile_option_flag},
    {"warn_indent", parser_set_token_info}, // will not be implemented
};

static int
parser_magic_comment(struct parser_params *parser, const char *str, long len)
{
    int indicator = 0;
    VALUE name = 0, val = 0;
    const char *beg, *end, *vbeg, *vend;

    if(len <= 7)
    {
        return FALSE;
    }
    if(!!(beg = magic_comment_marker(str, len)))
    {
        if(!(end = magic_comment_marker(beg, str + len - beg)))
        {
            return FALSE;
        }
        indicator = TRUE;
        str = beg;
        len = end - beg - 3;
    }

    /* %r"([^\\s\'\":;]+)\\s*:\\s*(\"(?:\\\\.|[^\"])*\"|[^\"\\s;]+)[\\s;]*" */
    while(len > 0)
    {
        const struct magic_comment *p = magic_comments;
        char *s;
        int i;
        long n = 0;

        for(; len > 0 && *str; str++, --len)
        {
            switch(*str)
            {
                case '\'': case '"': case ':': case ';':
                    continue;
                default:
                    if(ISSPACE(*str))
                        continue;
            }
            break;
        }
        for(beg = str; len > 0; str++, --len)
        {
            switch(*str)
            {
                case '\'': case '"': case ':': case ';':
                    break;
                default:
                    if(ISSPACE(*str))
                        break;
                    continue;
            }
            break;
        }
        for(end = str; len > 0 && ISSPACE(*str); str++, --len);
        if(!len)
            break;
        if(*str != ':')
        {
            if(!indicator)
                return FALSE;
            continue;
        }

        // *str == ':'

        do
        {
            str++;
        } while (--len > 0 && ISSPACE(*str));
        if(!len)
        {
            break;
        }
        if(*str == '"')
        {
            for(vbeg = ++str; --len > 0 && *str != '"'; str++)
            {
                if(*str == '\\')
                {
                    --len;
                    ++str;
                }
            }
            vend = str;
            if(len)
            {
                --len;
                ++str;
            }
        }
        else
        {
            for(vbeg = str; len > 0 && *str != '"' && *str != ';' && !ISSPACE(*str); --len, str++)
            { }
            vend = str;
        }
        if(indicator)
        {
            while(len > 0 && (*str == ';' || ISSPACE(*str)))
            {
                --len;
                str++;
            }
        }
        else
        {
            while(len > 0 && (ISSPACE(*str)))
            {
                --len, str++;
            }
            if(len)
            {
                return FALSE;
            }
        }

        n = end - beg;
        str_copy(name, beg, n);
        s = RSTRING_PTR(name);
        for(i = 0; i < n; ++i)
        {
            if(s[i] == '-')
            {
                s[i] = '_';
            }
        }
        do
        {
            if(STRNCASECMP(p->name, s, n) == 0 && !p->name[n])
            {
                n = vend - vbeg;
                if(p->length)
                {
                    n = (*p->length)(parser, vbeg, n);
                }
                str_copy(val, vbeg, n);
                (*p->func)(parser, p->name, RSTRING_PTR(val));
                break;
            }
        } while (++p < magic_comments + numberof(magic_comments));
    }

    return TRUE;
}


static const char *
magic_comment_marker(const char *str, long len)
{
    long i = 2;

    while(i < len)
    {
        switch(str[i])
        {
            case '-':
                if(str[i-1] == '*' && str[i-2] == '-') // '-*' [-]
                {
                    return str + i + 1;
                }
                i += 2;
                break;

            case '*':
                if(i + 1 >= len)
                    return 0;
                if(str[i+1] != '-') // [*] ^'-'
                {
                    i += 4;
                }
                else if(str[i-1] != '-') // ^'-' [*] '-'
                {
                    i += 2;
                }
                else // '-' [*] '-'
                {
                    return str + i + 2;
                }
                break;

            default:
                i += 3;
                break;
        }
    }
    return 0;
}

static long
parser_encode_length(struct parser_params *parser, const char *name, long len)
{
    long nlen;

    if(len > 5 && name[nlen = len - 5] == '-')
    {
        if(rb_memcicmp(name + nlen + 1, "unix", 4) == 0)
            return nlen;
    }
    if(len > 4 && name[nlen = len - 4] == '-')
    {
        if(rb_memcicmp(name + nlen + 1, "dos", 3) == 0)
            return nlen;
        if(rb_memcicmp(name + nlen + 1, "mac", 3) == 0 &&
            !(len == 8 && rb_memcicmp(name, "utf8-mac", len) == 0))
            /* exclude UTF8-MAC because the encoding named "UTF8" doesn't exist in Ruby */
            return nlen;
    }
    return len;
}

static void
magic_comment_encoding(struct parser_params *parser, const char *name, const char *val)
{
    if(!comment_at_top(parser))
    {
        return;
    }
    parser_set_encode(parser, val);
}

static void
parser_set_compile_option_flag(struct parser_params *parser, const char *name, const char *val)
{
    int b;

    if(parser->token_seen)
    {
        rb_warning1("`%s' is ignored after any tokens", WARN_S(name));
        return;
    }

    b = parser_get_bool(parser, name, val);
    if(b < 0) return;

    if(!parser->compile_option)
        parser->compile_option = rb_obj_hide(rb_ident_hash_new());
    rb_hash_aset(parser->compile_option, ID2SYM(rb_intern(name)), (b ? Qtrue : Qfalse));
}

static void
parser_set_token_info(struct parser_params *parser, const char *name, const char *val)
{
    int b = parser_get_bool(parser, name, val);
    if(b >= 0)
        parser->token_info_enabled = b;
}

static int
parser_get_bool(struct parser_params *parser, const char *name, const char *val)
{
    if(strcasecmp(val, "true") == 0)
    {
        return TRUE;
    }

    if(strcasecmp(val, "false") == 0)
    {
        return FALSE;
    }

    rb_compile_warning(ruby_sourcefile, ruby_sourceline, "invalid value for %s: %s", name, val);
    return -1;
}