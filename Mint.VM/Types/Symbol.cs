using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Threading;

namespace Mint
{
    public struct Symbol : iObject
    {
        private readonly Sym sym;

        public Symbol(string name)
        {
            sym = Sym.New(name);
        }

        public long   Id                => sym.id;
        public string Name              => sym.name;
        public Class  Class             => Class.SYMBOL;
        public Class  SingletonClass    { get { throw new TypeError("can't define singleton"); } }
        public Class  CalculatedClass   => Class.SYMBOL;
        public bool   HasSingletonClass => false;
        public bool   Frozen            => true;

        public void Freeze() { }

        public override string ToString() => sym.name;

        public string Inspect() => ":" + sym.name; // TODO: use String#Inspect if there is whitespace or non-ascii

        public bool IsA(Class klass) => Class.IsA(this, klass);

        public bool Equals(Symbol obj) => sym.id == obj.sym.id;

        public iObject Send(iObject name, params iObject[] args) => Object.Send(this, name, args);

        //public DynamicMetaObject GetMetaObject(Expression parameter) => new Object.Meta(parameter, this);

        public override bool Equals(object obj) => obj is Symbol && Equals((Symbol) obj);

        public override int GetHashCode() => sym.id.GetHashCode();

        public bool Equal(object other) => Equals(other);

        #region Static

        public static readonly Symbol SELF;
        public static readonly Symbol AREF;
        public static readonly Symbol ASET;
        public static readonly Symbol METHOD_MISSING;

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
        }

        #endregion

        private class Sym
        {
            private static long nextId = 0;

            public readonly long id = Interlocked.Increment(ref nextId) << 4 | 0xe;
            public readonly string name;

            public Sym(string name)
            {
                this.name = name;
            }

            ~Sym()
            {
                lock(SYMBOLS)
                {
                    SYMBOLS.Remove(name);
                }
            }

            public static Sym New(string name)
            {
                lock(SYMBOLS)
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
    }
}
