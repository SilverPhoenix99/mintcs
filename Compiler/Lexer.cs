using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static mint.Compiler.Lexer.States;
using static mint.Compiler.TokenType;

namespace mint.Compiler
{
    public partial class Lexer : IEnumerable<Token>
    {
		int p,
            cs,
			ts,
			te,
			act,
			top,
			line_jump;

        bool __end__seen = false;
		int[] stack = new int[10];
        int[] lines;
        Token eof_token;

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

        public BitStack Cmdarg { get; set; }

        public BitStack Cond { get; set; }

        public bool Eof => p > Data.Length;

        public bool InCmd { get; set; }

        public bool InKwarg { get; set; }

        public int LParBeg { get; set; }

        public int ParenNest { get; set; }

        public States State
        {
            get { return (States) cs; }
            set { cs = (int) value; }
        }

		public int TabWidth { get; set; }

        public Token NextToken()
		{
			if(!Eof && tokens.Count == 0) { Advance(); }
			return tokens.Count == 0 ? eof_token : tokens.Dequeue();
		}

		public void Reset()
		{
			p = 0;
			cs = (int) BOF;
			ts = -1;
			te = -1;
			act = 0;
			top = 0;
			line_jump = -1;
			ParenNest = 0;
			Cond = new BitStack();
			Cmdarg = new BitStack();
            LParBeg = 0;
			__end__seen = false;
			InCmd = false;
            InKwarg = false;

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
            eof_token = new Token(EOF, "$eof", Location(data.Length));
		}

        public IEnumerator<Token> GetEnumerator()
        {
            Token tok;
            do
            {
                yield return tok = NextToken();
            } while(tok.Type != EOF);
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

            var line = Array.BinarySearch(lines, pos) + 1;
            line = Math.Abs(line < 0 ? -line : line);
            return new Tuple<int, int>(line, pos - lines[line - 1] + 1);
        }
        
        private uint Peek(int op = 0)
        {
            // TODO use encoding to advance chars
            op += p;

            var c = 0 <= op && op < data.Length ? data[op] : 0u;

            if(c > 0
                && (cs == (int) STRING_CONTENT || cs == (int) WORD_CONTENT))
            {
                return literals.Peek().TranslateDelimiter((char) c);
            }

            return c;
        }
        
        private string CurrentToken(int ts = -1, int te = -1, int ots = 0, int ote = 0)
        {
            if(ts < 0) { ts = this.ts; }
            if(te < 0) { te = this.te; }
            ts += ots;

            return data.Substring(ts, te + ote - ts);
        }

        private bool FcalledBy(bool reject, uint offset, params States[] states)
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

        private bool FcalledBy(bool reject, params States[] states)
        {
            return FcalledBy(reject, 0, states);
        }

        private bool FcalledBy(params States[] states)
        {
            return FcalledBy(true, 0, states);
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
                        ParenNest++;
                        if(cs != (uint) EXPR_FNAME && cs != (uint) EXPR_DOT)
                        {
                            Cond.Push(false);
                            Cmdarg.Push(false);
                        }
                        break;
                    }

                    case ')': goto case ']';
                    case ']':
                    {
                        ParenNest--;
                        Cond.LexPop();
                        Cmdarg.LexPop();
                        break;
                    }

                    case '{':
                    {
                        if(literals.Count != 0)
                        {
                            literals.Peek().BraceCount++;
                        }

                        Cond.Push(false);
                        Cmdarg.Push(false);
                        if(type == kLAMBEG)
                        {
                            LParBeg = 0;
                            ParenNest--;
                        }
                        break;
                    }

                    case '}':
                    {
                        Cond.LexPop();
                        Cmdarg.LexPop();
                        cs = (int) EXPR_ENDARG;
                        if(literals.Count != 0)
                        {
                            var lit = literals.Peek();
                            if(lit.BraceCount == 0)
                            {
                                type = tSTRING_DEND;
                                lit.ContentStart = te + ote;
                                PushFcall();
                                cs = (int) lit.State;
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

        private Token GenToken(IReadOnlyDictionary<string, TokenType> type,
                               string token = null, Tuple<int, int> location = null,
                               int ts = -1, int te = -1, int ots = 0, int ote = 0)
        {
            if(ts < 0) { ts = this.ts; }
            if(te < 0) { te = this.te; }

            token = token ?? CurrentToken(ts, te, ots, ote);

            if(type == OPERATORS && token.Last() == '=')
            {
                token = token.Substring(0, token.Length-1);
                return GenToken(tOP_ASGN, token, location, ts, te, ots, ote);
            }
            
            return GenToken(type[token], token, location, ts, te, ots, ote);
        }

        private Token GenToken(IReadOnlyDictionary<string, Tuple<States, TokenType, TokenType>> type,
                               string token = null, Tuple<int, int> location = null,
                               int ts = -1, int te = -1, int ots = 0, int ote = 0)
        {
            if(ts < 0) { ts = this.ts; }
            if(te < 0) { te = this.te; }

            token = token ?? CurrentToken(ts, te, ots, ote);

            var config = type[token];
            var token_type = ProcessReserved(config.Item1, config.Item2, config.Item3);

            return GenToken(token_type, token, location, ts, te, ots, ote);
        }

        private TokenType ProcessReserved(States state, TokenType type, TokenType alt_type)
        {
            if(State == EXPR_FNAME)
            {
                State = state;
                return type;
            }

            State = state;

            InCmd = InCmd || state == EXPR_BEG;

            if(type == kDO)
            {
                if(LParBeg > 0 && LParBeg == ParenNest)
                {
                    LParBeg = 0;
                    ParenNest--;
                    return kDO_LAMBDA;
                }
                else if(Cond.Peek)
                {
                    return kDO_COND;
                }
                else if((Cmdarg.Peek && State != EXPR_ENDARG) || State == EXPR_BEG || State == EXPR_ENDARG)
                {
                    return kDO_BLOCK;
                }
            }
            else if(State != EXPR_BEG && State != EXPR_LABELARG && alt_type != type)
            {
                State = EXPR_BEG;
                return alt_type;
            }

            return type;
        }

        private Heredoc GenHeredocToken(int ts = -1)
        {
            if(ts < 0)
            {
                ts = this.ts;
            }

            var tok = CurrentToken(ts: ts);
            var heredoc = new Heredoc(tok, te);
            GenToken(heredoc.Type, token: tok, ts: ts);
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
                                    !InCmd
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

        private bool GenInterpolationTokens(TokenType type)
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

        private Token GenStringContentToken(int ote = int.MinValue)
        {
            if(ote == int.MinValue) { ote = ts - te; }
            var lit = literals.Peek();
            var tok = CurrentToken(ts: lit.ContentStart, ote: ote);
            if(tok.Length == 0)
            {
                return null;
            }
            return GenToken(tSTRING_CONTENT, token: tok, ts: lit.ContentStart);
        }

        private Token GenStringEndToken()
        {
            var lit = literals.Pop();
            var tok = CurrentToken();
            if(lit.IsRegexp)
            {
                return GenToken(tREGEXP_END, token: tok);
            }
            
            if(tok[tok.Length - 1] == ':')
            {
                return GenToken(tLABEL_END, token: tok);
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

        private Token KeywordToken(dynamic token_type, int lts, int lte, States? next_state, string token = null)
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

            var tok = GenToken(token_type, token: token, ts: lts >= 0 ? lts : ts);

            if(next_state != null)
            {
                // fnext *next_state;
                cs = (int) next_state;
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
            stack[top++] = (int) state;
        }

        private void PushFcall()
        {
            stack[top++] = cs;
        }

        private void PopFcall()
        {
            top--;
        }

        private static readonly IReadOnlyDictionary<string, TokenType> OPERATORS =
            (IReadOnlyDictionary<string, TokenType>) new SortedList<string, TokenType>(9)
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

        private static readonly IReadOnlyDictionary<string, TokenType> KEYWORDS =
            (IReadOnlyDictionary<string, TokenType>) new SortedList<string, TokenType>(51)
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

        private static readonly IReadOnlyDictionary<string, Tuple<States, TokenType, TokenType>> RESERVED =
            (IReadOnlyDictionary<string, Tuple<States, TokenType, TokenType>>)
            new SortedList<string, Tuple<States, TokenType, TokenType>>(41)
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