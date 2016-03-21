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

        public virtual Method FindMethod(Symbol name)
        {
            // Method resolution:
            //   1. Methods defined in the object's singleton class (i.e. the object itself)
            //   2. Modules mixed into the singleton class in reverse order of inclusion
            //   3. Methods defined by the object's class
            //   4. Modules included into the object's class in reverse order of inclusion
            //   5. Methods defined by the object's superclass.

            return CalculatedClass.FindMethod(name);
        }

        public iObject InstanceVariableGet(Symbol name)
        {
            iObject ivar;
            return variables.TryGetValue(name, out ivar) ? ivar : new NilClass();
        }

        public iObject InstanceVariableGet(String name) => InstanceVariableGet(new Symbol(name.Value));

        #region Static

        public const string VAR_START   = @"[a-zA-Z_\u0080-\uffff]";
        public const string IDENT_CHAR  = $"(?:{VAR_START}|\\d)";

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
