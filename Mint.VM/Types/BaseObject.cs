using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Mint
{
    public abstract class BaseObject : FrozenObject
    {
        private Class calculatedClass;
        protected readonly IDictionary<Symbol, iObject> variables = new Dictionary<Symbol, iObject>();

        protected BaseObject(Class klass)
        {
            calculatedClass = klass;
        }

        protected BaseObject() : this(Object.CLASS) { }

        public override Class Class             => HasSingletonClass ? calculatedClass.Superclass : calculatedClass;
        public override Class CalculatedClass   => calculatedClass;
        public override bool  HasSingletonClass => calculatedClass.IsSingleton;
        public override bool  Frozen            { get; protected set; }

        public override Class SingletonClass =>
            !calculatedClass.IsSingleton
                ? calculatedClass = new Class(calculatedClass, isSingleton: true)
                : calculatedClass;

        public override void Freeze() { Frozen = true; }

        public iObject InstanceVariableGet(Symbol name)
        {
            iObject ivar;
            return variables.TryGetValue(name, out ivar) ? ivar : new NilClass();
        }

        public iObject InstanceVariableGet(String name) => InstanceVariableGet(new Symbol(name.Value));

        #region Static

        public const string VAR_START = @"[a-zA-Z_\u0080-\uffff]";
        public static readonly string IDENT_CHAR = $"(?:{VAR_START}|\\d)";

        public static readonly Regex IVAR = new Regex($"^@{VAR_START}{IDENT_CHAR}*$", RegexOptions.Compiled);
        public static readonly Regex CVAR = new Regex($"^@@{VAR_START}{IDENT_CHAR}*$", RegexOptions.Compiled);
        public static readonly Regex BACK_REF = new Regex(@"^$[&+`']", RegexOptions.Compiled);
        public static readonly Regex NTH_REF = new Regex(@"^$[1-9]\d*", RegexOptions.Compiled);

        public static readonly Regex GVAR = new Regex(
            $"^\\$(?:-{IDENT_CHAR}|(?:{VAR_START}|0){IDENT_CHAR}*|[~*$?!@/\\;,.=:<>\"])$",
            RegexOptions.Compiled);

        #endregion
    }
}
