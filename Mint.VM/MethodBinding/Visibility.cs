﻿namespace Mint.MethodBinding
{
    public enum Visibility
    {
        Private,   // f()
        Protected, // self.f()
        Public     // anything.f()
    }
}