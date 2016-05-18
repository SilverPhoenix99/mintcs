using System.Linq.Expressions;

namespace Mint.MethodBinding.Binders
{
    public interface ClrParameterBinder
    {
        int Index { get; }
        MethodInformation Info;
        Expression[] Arguments;

        Expression Bind();
    }

    public abstract class BaseParameterBinder : ClrParameterBinder
    {
        public int Index { get; }

        public BaseParameterBinder(int index, MethodInformation info, Expression[] arguments)
        {
            Index = index;
            Info = info;
            Arguments = arguments;
        }
    }

    public class PrefixRequiredParameterBinder : BaseParameterBinder
    {
        public RequiredParameterBinder(int index, MethodInformation info, Expression[] arguments)
            : base(index, info, arguments)
        { }

        public Expression Bind() => arguments[Index];
    }

    public class OptionalParameterBinder : BaseParameterBinder
    {
        public OptionalParameterBinder(int index, MethodInformation info, Expression[] arguments)
            : base(index, info, arguments)
        { }

        public Expression Bind()
        {
            if(Index < arguments.Length)
            {
                return arguments[Index];
            }

            var parameter = info.MethodInfo.GetParameters()[Index];
            var defaultValue = parameter.HasDefaultValue ? parameter.DefaultValue : null;
            return Expression.Constant(defaultValue);
        }
    }
}