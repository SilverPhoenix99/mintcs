using System.Dynamic;

namespace Mint
{
    public interface iObject : IDynamicMetaObjectProvider
    {
        long  Id { get; }

        // Always returns the class that isn't singleton
        Class Class { get; }

        // Creates a new singleton class if needed
        Class SingletonClass { get; }

        // Returns SingletonClass if it exists, otherwise it returns Class
        Class CalculatedClass { get; }

        bool  HasSingletonClass { get; }

        bool Frozen { get; }

        void Freeze();

        string Inspect();

        bool IsA(Class klass);

        //iObject MethodMissing(params iObject[] args);

        iObject Send(iObject name, params iObject[] args);

        bool Equal(object other);
    }
}