using System.Dynamic;

namespace Mint.Types
{
    public interface iObject : IDynamicMetaObjectProvider
    {
        long  Id                { get; }
       
        Class Class             { get; } // Always returns the class that isn't singleton

        Class SingletonClass    { get; } // Creates a new singleton class if needed
                
        Class RealClass         { get; } // Returns singleton class if it exists, otherwise it returns the Class

        bool  HasSingletonClass { get; }
        
        bool Frozen             { get; }

        void Freeze();

        string Inspect();
    }
}
