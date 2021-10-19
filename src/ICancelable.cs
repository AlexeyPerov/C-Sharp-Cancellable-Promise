using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace RSG
{
    public interface ICancelable
    {
        /// <summary>
        /// Current state of promise
        /// </summary>
        PromiseState CurState { get; }
        
        bool CanBeCanceled { get; }
        ICancelable Parent { get; }
        HashSet<ICancelable> Children { get; }
        void AttachParent(ICancelable parent);
        void AttachChild(ICancelable child);
        
        /// <summary>
        /// Cancels the whole chain where this promise exists.
        /// </summary>
        void Cancel();
    }
}