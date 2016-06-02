static int
parser_magic_comment(struct parser_params *parser, const char *str, long len)
{
    int indicator = 0;
    VALUE name = 0, val = 0;
    const char *beg, *end, *vbeg, *vend;
#define str_copy(_s, _p, _n) ((_s) \
        ? (void)(rb_str_resize((_s), (_n)), \
           MEMCPY(RSTRING_PTR(_s), (_p), char, (_n)), (_s)) \
        : (void)((_s) = STR_NEW((_p), (_n))))

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
            switch (*str)
            {
                case '\'': case '"': case ':': case ';':
                    continue;
            }
            if(!ISSPACE(*str))
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