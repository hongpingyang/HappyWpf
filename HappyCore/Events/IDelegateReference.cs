using System;

namespace HappyCore.Events
{
    public interface IDelegateReference
    {
        Delegate Target { get; }
    }
}
