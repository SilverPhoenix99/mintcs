using System.Collections.Generic;
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

        protected readonly IDictionary<Symbol, iObject> variables = new Dictionary<Symbol, iObject>();

        public override Class Class => HasSingletonClass ? EffectiveClass.Superclass : EffectiveClass;

        protected Class effectiveClass;
        public override Class EffectiveClass => effectiveClass;

        public override bool  HasSingletonClass => EffectiveClass.IsSingleton;

        public override bool  Frozen { get; protected set; }

        public override Class SingletonClass =>
            !EffectiveClass.IsSingleton
                ? effectiveClass = new Class(EffectiveClass, isSingleton: true)
                : EffectiveClass;

        protected BaseObject(Class klass)
        {
            effectiveClass = klass;
        }

        protected BaseObject() : this(Class.OBJECT) { }

        public override iObject Freeze()
        {
            Frozen = true;
            return this;
        }

        public override iObject InstanceVariableGet(Symbol name)
        {
            Object.ValidateInstanceVariableName(name.Name);
            iObject ivar;
            return variables.TryGetValue(name, out ivar) ? ivar : new NilClass();
        }

        public override iObject InstanceVariableSet(Symbol name, iObject obj)
        {
            Object.ValidateInstanceVariableName(name.Name);
            return variables[name] = obj;
        }
    }
}
