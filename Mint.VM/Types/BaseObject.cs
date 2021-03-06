﻿using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Mint
{
    public abstract class BaseObject : FrozenObject
    {
        public const string VAR_START = @"[a-zA-Z_\u0080-\uffff]";
        public static readonly string IDENT_CHAR = $"(?:{VAR_START}|\\d)";
        public static readonly Regex IVAR = new Regex($"^@{VAR_START}{IDENT_CHAR}*$", RegexOptions.Compiled);
        public static readonly Regex CVAR = new Regex($"^@@{VAR_START}{IDENT_CHAR}*$", RegexOptions.Compiled);
        public static readonly Regex BACK_REF = new Regex(@"^$[&+`']", RegexOptions.Compiled);
        public static readonly Regex NTH_REF = new Regex(@"^$[1-9]\d*", RegexOptions.Compiled);

        public static readonly Regex GVAR = new Regex(
            $"^\\$(?:-{IDENT_CHAR}|(?:{VAR_START}|0){IDENT_CHAR}*|[~*$?!@/\\;,.=:<>\"])$",
            RegexOptions.Compiled);


        protected readonly IDictionary<Symbol, iObject> variables = new LinkedDictionary<Symbol, iObject>();
        protected Class effectiveClass;
        protected bool isFrozen;


        protected BaseObject(Class klass)
        {
            effectiveClass = klass;
        }


        protected BaseObject()
            : this(Class.OBJECT)
        { }


        public override Class Class => HasSingletonClass ? EffectiveClass.Superclass : EffectiveClass;
        public override Class EffectiveClass => effectiveClass;
        public override bool HasSingletonClass => EffectiveClass.IsSingleton;
        public override IEnumerable<Symbol> InstanceVariables => variables.Keys;
        public override bool Frozen => isFrozen;

        public override Class SingletonClass =>
            !EffectiveClass.IsSingleton
                ? effectiveClass = new Class(EffectiveClass, isSingleton: true)
                : EffectiveClass;


        public override iObject Freeze()
        {
            isFrozen = true;
            return this;
        }


        public override iObject InstanceVariableGet(Symbol name)
        {
            Object.ValidateInstanceVariableName(name.Name);
            return variables.TryGetValue(name, out var ivar) ? ivar : new NilClass();
        }


        public override iObject InstanceVariableSet(Symbol name, iObject obj)
        {
            Object.ValidateInstanceVariableName(name.Name);
            return variables[name] = obj;
        }
    }
}
