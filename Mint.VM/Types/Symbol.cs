using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Mint.Reflection;

namespace Mint
{
    public struct Symbol : iObject
    {
        private readonly Sym sym;

        public long Id => sym.Id;

        public string Name => sym.Name;

        public Class Class => Class.SYMBOL;

        public Class SingletonClass { get { throw new TypeError("can't define singleton"); } }

        public Class EffectiveClass => Class.SYMBOL;

        public bool HasSingletonClass => false;

        public bool Frozen => true;

        public Symbol(string name)
        {
            sym = Sym.New(name);
        }

        public iObject Freeze() => this;

        public override string ToString() => sym.Name;

        public string Inspect() => ":" + sym.Name; // TODO: use String#Inspect if there is whitespace or non-ascii

        public bool IsA(Class klass) => Class.IsA(this, klass);

        public bool Equals(Symbol obj) => sym.Id == obj.sym.Id;

        public iObject Send(iObject name, params iObject[] args) => Object.Send(this, name, args);

        public override bool Equals(object obj) => obj is Symbol && Equals((Symbol) obj);

        public override int GetHashCode() => sym.Id.GetHashCode();

        public bool Equal(object other) => Equals(other);

        public iObject InstanceVariableGet(Symbol name)
        {
            Object.ValidateInstanceVariableName(name.Name);
            return null;
        }

        public iObject InstanceVariableGet(string name) => InstanceVariableGet(new Symbol(name));

        public iObject InstanceVariableSet(Symbol name, iObject obj)
        {
            Object.ValidateInstanceVariableName(name.Name);
            throw new RuntimeError($"can't modify frozen {EffectiveClass.FullName}");
        }

        public iObject InstanceVariableSet(string name, iObject obj) => InstanceVariableSet(new Symbol(name), obj);

        #region Static

        public static readonly Symbol SELF;
        public static readonly Symbol AREF;
        public static readonly Symbol ASET;
        public static readonly Symbol METHOD_MISSING;
        public static readonly Symbol NOT_OP;
        public static readonly Symbol EQ;
        public static readonly Symbol EQQ;
        public static readonly Symbol NEQ;
        public static readonly Symbol CMP;
        public static readonly Symbol TO_HASH;
        public static readonly Symbol TO_ARY;
        public static readonly Symbol PLUS;
        public static readonly Symbol MINUS;
        public static readonly Symbol DIV;
        public static readonly Symbol PERCENT;
        public static readonly Symbol MUL;
        public static readonly Symbol POW;
        public static readonly Symbol GREATER;
        public static readonly Symbol GEQ;
        public static readonly Symbol LESS;
        public static readonly Symbol LEQ;
        public static readonly Symbol LSHIFT;
        public static readonly Symbol RSHIFT;
        public static readonly Symbol BIN_AND;
        public static readonly Symbol BIN_OR;
        public static readonly Symbol NEG;
        public static readonly Symbol XOR;
        public static readonly Symbol UPLUS;
        public static readonly Symbol UMINUS;

        private static readonly IDictionary<string, WeakReference<Sym>> SYMBOLS;

        public static bool operator == (Symbol self, object obj) => self.Equals(obj);

        public static bool operator != (Symbol self, object obj) => !self.Equals(obj);

        public static explicit operator Symbol(string s) => new Symbol(s);

        public static explicit operator string(Symbol s) => s.Name;

#if DEBUG
        public static int Count => SYMBOLS.Count;
#endif

        static Symbol()
        {
            SYMBOLS = new Dictionary<string, WeakReference<Sym>>();
            SELF = new Symbol("self");
            AREF = new Symbol("[]");
            ASET = new Symbol("[]=");
            METHOD_MISSING = new Symbol("method_missing");
            NOT_OP = new Symbol("!");
            EQ = new Symbol("==");
            EQQ = new Symbol("===");
            NEQ = new Symbol("!=");
            CMP = new Symbol("<=>");
            TO_HASH = new Symbol("to_hash");
            TO_ARY = new Symbol("to_ary");
            PLUS = new Symbol("+");
            MINUS = new Symbol("-");
            MUL = new Symbol("*");
            POW = new Symbol("**");
            DIV = new Symbol("/");
            PERCENT = new Symbol("%");
            GREATER = new Symbol(">");
            GEQ = new Symbol(">=");
            LESS = new Symbol("<");
            LEQ = new Symbol("<=");
            LSHIFT = new Symbol("<<");
            RSHIFT = new Symbol(">>");
            BIN_AND = new Symbol("&");
            BIN_OR = new Symbol("|");
            NEG = new Symbol("~@");
            XOR = new Symbol("^");
            UPLUS = new Symbol("+@");
            UMINUS = new Symbol("-@");
        }

        #endregion

        private class Sym
        {
            private static long nextId;

            public readonly long Id = Interlocked.Increment(ref nextId) << 4 | 0xe;
            public readonly string Name;

            private Sym(string name)
            {
                Name = name;
            }

            ~Sym()
            {
                lock(typeof(Symbol))
                {
                    SYMBOLS.Remove(Name);
                }
            }

            public static Sym New(string name)
            {
                lock(typeof(Symbol))
                {
                    WeakReference<Sym> weakSym;
                    Sym sym;
                    if(SYMBOLS.TryGetValue(name, out weakSym) && weakSym.TryGetTarget(out sym))
                    {
                        return sym;
                    }

                    sym = new Sym(name);
                    SYMBOLS[name] = new WeakReference<Sym>(sym, false);
                    return sym;
                }
            }
        }

        public static class Reflection
        {
            public static readonly ConstructorInfo Ctor = Reflector<Symbol>.Ctor<string>();
        }

        public static class Expressions
        {
            public static NewExpression New(Expression name) => Expression.New(Reflection.Ctor, name);
        }
    }
}
