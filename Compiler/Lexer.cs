using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using mint.Extensions;
using static mint.Compiler.Lexer.States;
using static mint.Compiler.TokenType;

namespace mint.Compiler
{
    public partial class Lexer : IEnumerable<Token>
    {
		int p,
			ts,
			te,
			act,
			top,
			line_jump,
			paren_nest,
			lpar_beg;

        uint cs,
             cond,
             cmdarg;

        bool __end__seen = false,
			 in_cmd      = false,
			 in_kwarg    = false;

		uint[] stack = new uint[10];
        int[] lines;

		readonly Queue<Token> tokens = new Queue<Token>();
        readonly Stack<iLiteral> literals = new Stack<iLiteral>();

		string data;

        public Lexer(string data = null)
        {
			Data = data ?? "";
            TabWidth = 8;
        }

		public string Data
		{
			get { return data; }
			set
			{
				data = value;
				Reset();
			}
		}

        public bool Cmdarg => (cmdarg & 1u) == 1u;

        public bool Cond => (cond & 1u) == 1u;

		public bool Eof => p > Data.Length;

        public States State
        {
            get { return (States) cs; }
            set { cs = (uint) value; }
        }

		public int TabWidth { get; set; }

        public Token NextToken()
		{
			if(Eof) { return Token.Null; }
			Advance();
			if(tokens.Count == 0) { return Token.Null; }
			return tokens.Dequeue();
		}

		public void Reset()
		{
			p = 0;
			cs = Lex_en_BOF;
			ts = -1;
			te = -1;
			act = 0;
			top = 0;
			line_jump = -1;
			paren_nest = 0;
			cond = 0;
			cmdarg = 0;
			lpar_beg = 0;
			__end__seen = false;
			in_cmd = false;
			in_kwarg = false;

            List<int> lines = new List<int>();
            lines.Add(0);

			for(int i = 0; i < data.Length; i++)
			{
				var c = data[i];
				if(c == 0 || c == 0x4 || c == 0x1a)
				{
                    data = data.Substring(0, i);
					break;
				}

                if(c == '\n')
                {
                    lines.Add(i);
                }
			}

            lines.Add(data.Length);
            this.lines = lines.ToArray();
		}

        public IEnumerator<Token> GetEnumerator()
        {
            Token tok;
            do
            {
                yield return tok = NextToken();
            } while(tok.Type != None);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Tuple<int, int> Location(int pos = -1)
        {
            if(pos < 0)
            {
                pos = ts;
            }

            pos = Array.BinarySearch(lines, pos) + 1;
            var line = Math.Abs(pos < 0 ? -pos : pos + 1);
            return new Tuple<int, int>(line, pos - lines[line - 1] + 1);
        }

        public void PushCmdarg(bool val)
        {
            cmdarg = (cmdarg << 1) | (val ? 1u : 0u);
        }

        public void LexPopCmdarg()
        {
            cmdarg = (cmdarg >> 1) | (cmdarg & 1u);
        }

        public bool PopCmdarg() => ((cmdarg >>= 1) & 1u) == 1u;

        public void PushCond(bool val)
        {
            cond = (cond << 1) | (val ? 1u : 0u);
        }

        public void LexPopCond()
        {
            cond = (cond >> 1) | (cond & 1u);
        }

        public bool PopCond() => ((cond >>= 1) & 1u) == 1u;

        private uint Peek(int n = 0)
        {
            // TODO use encoding to advance chars
            n = p + n;
            return 0 <= n && n < data.Length ? data[n] : 0u;
        }
        
        private string CurrentToken(int ts = -1, int te = -1, int ots = 0, int ote = 0)
        {
            if(ts < 0) { ts = this.ts; }
            if(te < 0) { te = this.te; }
            ts += ots;

            return data.Substring(ts, te + ote - ts);
        }

        private bool FcalledBy(bool reject = true, uint offset = 0, params States[] states)
        {
            var intStates = states.Select((s) => (int) s);

            for(var i = top-offset-1; i >= 0; i--)
            {
                if(reject && stack[i] == (int) COMMON_EXPR)
                {
                    continue;
                }

                if(intStates.Contains(stack[i]))
                {
                    return true;
                }
            }

            return false;
        }

        private bool FcalledBy(bool reject = true, params States[] states)
        {
            return FcalledBy(reject: reject, states: states);
        }

        private bool FcalledBy(params States[] states)
        {
            return FcalledBy(states: states);
        }

        private Token GenToken(TokenType type,
                               string token = null, Tuple<int, int> location = null,
                               int ts = -1, int te = -1, int ots = 0, int ote = 0)
        {
            if(ts < 0) { ts = this.ts; }
            if(te < 0) { te = this.te; }

            token = token ?? CurrentToken(ts, te, ots, ote);
            location = location ?? Location(ts + ots);

            if(token.Length == 1)
            {
                var c = token[0];
                switch(token[0])
                {
                    case '(': goto case '[';
                    case '[':
                    {
                        paren_nest++;
                        if(cs != (uint) EXPR_FNAME && cs != (uint) EXPR_DOT)
                        {
                            PushCond(false);
                            PushCmdarg(false);
                        }
                        break;
                    }

                    case ')': goto case ']';
                    case ']':
                    {
                        paren_nest--;
                        LexPopCond();
                        LexPopCmdarg();
                        break;
                    }

                    case '{':
                    {
                        if(literals.Count != 0)
                        {
                            literals.Peek().BraceCount++;
                        }

                        PushCond(false);
                        PushCmdarg(false);
                        if(type == kLAMBEG)
                        {
                            lpar_beg = 0;
                            paren_nest--;
                        }
                        break;
                    }

                    case '}':
                    {
                        LexPopCond();
                        LexPopCmdarg();
                        cs = (uint) EXPR_ENDARG;
                        if(literals.Count != 0)
                        {
                            var lit = literals.Peek();
                            if(lit.BraceCount == 0)
                            {
                                type = tSTRING_DEND;
                                lit.ContentStart = te + ote;
                                cs = lit.State;
                            }
                            else
                            {
                                lit.BraceCount--;
                            }
                        }
                        break;
                    }
                }
            }

            var tok = new Token(type, token, location);
            tokens.Enqueue(tok);
            return tok;
        }

        private Token GenToken(Dictionary<string, TokenType> type,
                               string token = null, Tuple<int, int> location = null,
                               int ts = -1, int te = -1, int ots = 0, int ote = 0)
        {
            if(ts < 0) { ts = this.ts; }
            if(te < 0) { te = this.te; }

            token = token ?? CurrentToken(ts, te, ots, ote);

            if(type == OPERATORS && token.Last() == '=')
            {
                token = token.Substring(0, token.Length-1);
                return GenToken(tOP_ASSIGN, token, location, ts, te, ots, ote);
            }
            
            return GenToken(type[token], token, location, ts, te, ots, ote);
        }

        private Token GenToken(Dictionary<string, Tuple<States, TokenType, TokenType>> type,
                               string token = null, Tuple<int, int> location = null,
                               int ts = -1, int te = -1, int ots = 0, int ote = 0)
        {
            var config = RESERVED[token];
            var state = config.Item1;
            var token_type = config.Item2;
            ProcessReserved(ref state, ref token_type, config.Item3);

            return GenToken(token_type, token, location, ts, te, ots, ote);
        }

        private void ProcessReserved(ref States state, ref TokenType type, TokenType alt_type)
        {
            if(cs == (uint) EXPR_FNAME)
            {
                return;
            }

            in_cmd = in_cmd || state == EXPR_BEG;

            if(type == kDO)
            {
                if(lpar_beg > 0 && lpar_beg == paren_nest)
                {
                    lpar_beg = 0;
                    paren_nest -= 1;
                    type = kDO_LAMBDA;
                }
                else if(Cond)
                {
                    type = kDO_COND;
                }
                else if((Cmdarg && cs != (uint) EXPR_ENDARG) || cs == (uint) EXPR_BEG || cs == (uint) EXPR_ENDARG)
                {
                    type = kDO_BLOCK;
                }
            }
            else if(cs != (uint) EXPR_BEG && cs != (uint) EXPR_LABELARG && alt_type != type)
            {
                type = alt_type;
                state = EXPR_BEG;
            }
        }

        private Heredoc GenHeredocToken(int ts = -1)
        {
            if(ts < 0)
            {
                ts = this.ts;
            }

            var heredoc = new Heredoc(CurrentToken(ts: ts), te);
            GenToken(heredoc.Type, token: heredoc.FullId, ts: ts);
            literals.Push(heredoc);
            return heredoc;
        }

        private Token GenLiteralToken(int ts = -1)
        {
            if(ts < 0) { ts = this.ts; }

            var token = CurrentToken(ts: ts);

            var can_label = (token == "\"" || token == "'")
                            && (
                                (
                                    !in_cmd
                                    && (
                                        cs == (uint) EXPR_BEG
                                        || cs == (uint) EXPR_ENDFN
                                        || FcalledBy(EXPR_BEG, EXPR_ENDFN)
                                    )
                                )
                                || cs == (uint) EXPR_ARG 
                                || cs == (uint) EXPR_CMDARG 
                                || cs == (uint) EXPR_LABELARG 
                                || FcalledBy(EXPR_ARG, EXPR_CMDARG, EXPR_LABELARG)
                            );

            var lit = new Literal(token, te, can_label);
            literals.Push(lit);

            return GenToken(lit.Type, token: token, ts: ts);
        }

        private bool GenInterpolationToken(TokenType type)
        {
            var lit = literals.Peek();
            lit.CommitIndent();
            if(!lit.Interpolates)
            {
                return false;
            }
            var tok = CurrentToken(ots: 1);
            GenStringContentToken(-tok.Length - 1);
            GenToken(tSTRING_DVAR, token: "#");
            GenToken(type, token: tok, ts: ts + 1);
            lit.ContentStart = te;
            return true;
        }

        private bool GenStringContentToken(int ote = int.MinValue)
        {
            if(ote == int.MinValue) { ote = ts - te; }
            var lit = literals.Peek();
            var tok = CurrentToken(ts: lit.ContentStart, ote: ote);
            if(tok.Length == 0)
            {
                return false;
            }
            GenToken(tSTRING_CONTENT, token: tok, ts: lit.ContentStart);
            return true;
        }

        private Token GenStringEndToken()
        {
            var lit = literals.Pop();
            var tok = CurrentToken();
            if(lit.Delimiter == "/")
            {
                return GenToken(tREGEXP_END, token: tok);
            }
            
            if(tok.Last() == ':')
            {
                return GenToken(tREGEXP_END, token: tok);
            }

            if(lit.Dedents)
            {
                var token = GenToken(tSTRING_END, token: tok);
                token.Properties["dedent"] = lit.Indent;
                return token;
            }

            return GenToken(tSTRING_END, token: tok);
        }

        private const int RATIONAL_FLAG = 1;
        private const int IMAGINARY_FLAG = 2;

        private Token GenNumberToken(TokenType type, int num_base, int num_flags, int ts = -1)
        {
            if((num_flags & RATIONAL_FLAG) == 1)
            {
                type = tRATIONAL;
            }
            else if((num_flags & IMAGINARY_FLAG) == 1)
            {
                type = tIMAGINARY;
            }

            var tok = GenToken(type, ts: ts);
            tok.Properties["num_base"] = num_base;
            return tok;
        }

        private Token KeywordToken(TokenType token_type, int lts, int lte, States next_state, string token = null)
        {
            // fexec te = lte;
            if(lte >= 0)
            {
                p = (te = lte) - 1;
            }

            if(FcalledBy(false, COMMON_EXPR) || cs == (uint) COMMON_EXPR)
            {
                PopFcall();
            }

            // fnext *next_state;
            var current_state = cs;

            var tok = GenToken(token_type, token: token, ts: lts >= 0 ? lts : ts);
            if(current_state == cs)
            {
                cs = (uint) next_state;
            }

            return tok;
        }

        private int NextBOL()
        {
            if(line_jump > p)
            {
                return line_jump;
            }

            for(int p = this.p; p < data.Length; p++)
            {
                if(data[p] == '\n')
                {
                    return p + 1;
                }
            }

            return data.Length + 1;
        }

        private void PushFcall(States state)
        {
            stack[top++] = (uint) state;
        }

        private void PopFcall()
        {
            top--;
        }

        private static readonly Dictionary<string, TokenType> OPERATORS = new Dictionary<string, TokenType>()
        {
            { "*",  kMUL     },
            { "**", kPOW     },
            { "+",  kPLUS    },
            { "-",  kMINUS   },
            { "&",  kBIN_AND },
            { "::", kCOLON2  },
            { "(",  kLPAREN2 },
            { "[",  kLBRACK2 },
            { "{",  kLBRACE2 },
        };

        private static readonly Dictionary<string, TokenType> KEYWORDS = new Dictionary<string, TokenType>()
        {
            { "!",   kNOTOP     },
            { "!=",  kNEQ       },
            { "!@",  kNOTOP     },
            { "!~",  kNMATCH    },
            { "&",   kAMPER     },
            { "&&",  kANDOP     },
            { "&.",  kANDDOT    },
            { "(",   kLPAREN    },
            { ")",   kRPAREN    },
            { "*",   kSTAR      },
            { "**",  kDSTAR     },
            { "+",   kUPLUS     },
            { "+@",  kUPLUS     },
            { ",",   kCOMMA     },
            { "-",   kUMINUS    },
            { "->",  kLAMBDA    },
            { "-@",  kUMINUS    },
            { ".",   kDOT       },
            { "..",  kDOT2      },
            { "...", kDOT3      },
            { "/",   kDIV       },
            { ":",   kCOLON     },
            { "::",  kCOLON3    },
            { ";",   kSEMICOLON },
            { "<",   kLESS      },
            { "<<",  kLSHIFT    },
            { "<=",  kLEQ       },
            { "<=>", kCMP       },
            { "=",   kASSIGN    },
            { "==",  kEQ        },
            { "===", kEQQ       },
            { "=>",  kASSOC     },
            { "=~",  kMATCH     },
            { ">",   kGREATER   },
            { ">=",  kGEQ       },
            { ">>",  kRSHIFT    },
            { "?",   kQMARK     },
            { "[",   kLBRACK    },
            { "[]",  kAREF      },
            { "[]=", kASET      },
            { "]",   kRBRACK    },
            { "^",   kXOR       },
            { "`",   kBACKTICK  },
            { "{",   kLBRACE    },
            { "|",   kPIPE      },
            { "||",  kOROP      },
            { "}",   kRBRACE    },
            { "~",   kNEG       },
            { "~@",  kNEG       },
            { "%",   kPERCENT   },
            { "\\",  kBACKSLASH },
        };

        private static readonly Dictionary<string, Tuple<States, TokenType, TokenType>> RESERVED =
            new Dictionary<string, Tuple<States, TokenType, TokenType>>()
            {
                { "alias",        new Tuple<States, TokenType, TokenType>(EXPR_FNAME, kALIAS,        kALIAS)        },
                { "and",          new Tuple<States, TokenType, TokenType>(EXPR_BEG,   kAND,          kAND)          },
                { "BEGIN",        new Tuple<States, TokenType, TokenType>(EXPR_END,   kAPP_BEGIN,    kAPP_BEGIN)    },
                { "begin",        new Tuple<States, TokenType, TokenType>(EXPR_BEG,   kBEGIN,        kBEGIN)        },
                { "break",        new Tuple<States, TokenType, TokenType>(EXPR_MID,   kBREAK,        kBREAK)        },
                { "case",         new Tuple<States, TokenType, TokenType>(EXPR_BEG,   kCASE,         kCASE)         },
                { "class",        new Tuple<States, TokenType, TokenType>(EXPR_CLASS, kCLASS,        kCLASS)        },
                { "def",          new Tuple<States, TokenType, TokenType>(EXPR_FNAME, kDEF,          kDEF)          },
                { "defined?",     new Tuple<States, TokenType, TokenType>(EXPR_ARG,   kDEFINED,      kDEFINED)      },
                { "do",           new Tuple<States, TokenType, TokenType>(EXPR_BEG,   kDO,           kDO)           },
                { "else",         new Tuple<States, TokenType, TokenType>(EXPR_BEG,   kELSE,         kELSE)         },
                { "elsif",        new Tuple<States, TokenType, TokenType>(EXPR_BEG,   kELSIF,        kELSIF)        },
                { "END",          new Tuple<States, TokenType, TokenType>(EXPR_END,   kAPP_END,      kAPP_END)      },
                { "end",          new Tuple<States, TokenType, TokenType>(EXPR_END,   kEND,          kEND)          },
                { "ensure",       new Tuple<States, TokenType, TokenType>(EXPR_BEG,   kENSURE,       kENSURE)       },
                { "false",        new Tuple<States, TokenType, TokenType>(EXPR_END,   kFALSE,        kFALSE)        },
                { "for",          new Tuple<States, TokenType, TokenType>(EXPR_BEG,   kFOR,          kFOR)          },
                { "if",           new Tuple<States, TokenType, TokenType>(EXPR_BEG,   kIF,           kIF_MOD)       },
                { "in",           new Tuple<States, TokenType, TokenType>(EXPR_BEG,   kIN,           kIN)           },
                { "module",       new Tuple<States, TokenType, TokenType>(EXPR_BEG,   kMODULE,       kMODULE)       },
                { "next",         new Tuple<States, TokenType, TokenType>(EXPR_MID,   kNEXT,         kNEXT)         },
                { "nil",          new Tuple<States, TokenType, TokenType>(EXPR_END,   kNIL,          kNIL)          },
                { "not",          new Tuple<States, TokenType, TokenType>(EXPR_ARG,   kNOT,          kNOT)          },
                { "or",           new Tuple<States, TokenType, TokenType>(EXPR_BEG,   kOR,           kOR)           },
                { "redo",         new Tuple<States, TokenType, TokenType>(EXPR_END,   kREDO,         kREDO)         },
                { "rescue",       new Tuple<States, TokenType, TokenType>(EXPR_MID,   kRESCUE,       kRESCUE_MOD)   },
                { "retry",        new Tuple<States, TokenType, TokenType>(EXPR_END,   kRETRY,        kRETRY)        },
                { "return",       new Tuple<States, TokenType, TokenType>(EXPR_MID,   kRETURN,       kRETURN)       },
                { "self",         new Tuple<States, TokenType, TokenType>(EXPR_END,   kSELF,         kSELF)         },
                { "super",        new Tuple<States, TokenType, TokenType>(EXPR_ARG,   kSUPER,        kSUPER)        },
                { "then",         new Tuple<States, TokenType, TokenType>(EXPR_BEG,   kTHEN,         kTHEN)         },
                { "true",         new Tuple<States, TokenType, TokenType>(EXPR_END,   kTRUE,         kTRUE)         },
                { "undef",        new Tuple<States, TokenType, TokenType>(EXPR_FNAME, kUNDEF,        kUNDEF)        },
                { "unless",       new Tuple<States, TokenType, TokenType>(EXPR_BEG,   kUNLESS,       kUNLESS_MOD)   },
                { "until",        new Tuple<States, TokenType, TokenType>(EXPR_BEG,   kUNTIL,        kUNTIL_MOD)    },
                { "when",         new Tuple<States, TokenType, TokenType>(EXPR_BEG,   kWHEN,         kWHEN)         },
                { "while",        new Tuple<States, TokenType, TokenType>(EXPR_BEG,   kWHILE,        kWHILE_MOD)    },
                { "yield",        new Tuple<States, TokenType, TokenType>(EXPR_ARG,   kYIELD,        kYIELD)        },
                { "__ENCODING__", new Tuple<States, TokenType, TokenType>(EXPR_END,   k__ENCODING__, k__ENCODING__) },
                { "__FILE__",     new Tuple<States, TokenType, TokenType>(EXPR_END,   k__FILE__,     k__FILE__)     },
                { "__LINE__",     new Tuple<States, TokenType, TokenType>(EXPR_END,   k__LINE__,     k__LINE__)     }
            };
    }
}