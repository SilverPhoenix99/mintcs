namespace Mint
{
    public interface iObject
    {
        long  Id { get; }

        // Always returns the class that isn't singleton
        Class Class { get; }

        // Creates a new singleton class if needed
        Class SingletonClass { get; }

        // Returns SingletonClass if it exists, otherwise it returns Class
        Class EffectiveClass { get; }

        bool HasSingletonClass { get; }

        bool Frozen { get; }

        iObject Freeze();

        string Inspect();

        bool IsA(Class klass);

        //iObject MethodMissing(params iObject[] args);

        iObject Send(iObject name, params iObject[] args);

        bool Equal(object other);

        iObject InstanceVariableGet(Symbol name);
        iObject InstanceVariableGet(string name);

        iObject InstanceVariableSet(Symbol name, iObject obj);
        iObject InstanceVariableSet(string name, iObject obj);
    }
}