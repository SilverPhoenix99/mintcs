﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Threading;

namespace Mint.Types
{
    public struct Symbol : iObject
    {
        public static Class CLASS = new Class(name: "Symbol");

        private static IDictionary<string, WeakReference<Sym>> SYMBOLS = new Dictionary<string, WeakReference<Sym>>();

        private Sym sym;


        public Symbol(string name)
        {
            sym = Sym.New(name);
        }


        public long   Id                => sym.id;
        public string Name              => sym.name;
        public Class  Class             => CLASS;
        public Class  SingletonClass    { get { throw new TypeError("can't define singleton"); } }
        public Class  RealClass         => CLASS;
        public bool   HasSingletonClass => false;
        public bool   Frozen            => true;

        public void Freeze() {}

        public override string ToString() => sym.name;
        
        public string Inspect() => ":" + sym.name;

        public bool IsA(Class klass) => Class.IsA(this, klass);

        public DynamicMetaObject GetMetaObject(Expression parameter) => new Object.Meta(parameter, this);

#if DEBUG
        internal static int Count => SYMBOLS.Count;
#endif

        static Symbol()
        {
            Object.DefineClass(CLASS);
        }

        private class Sym
        {
            private static long nextId = 0;

            public readonly long id = Interlocked.Increment(ref nextId) << 2 | 0x2;
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
