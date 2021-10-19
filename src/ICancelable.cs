using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace RSG
{
    public interface ICancelable
    {
        bool CanBeCanceled { get; }
        ICancelable Parent { get; }
        HashSet<ICancelable> Children { get; }
        void AttachParent(ICancelable parent);
        void AttachChild(ICancelable child);
        
        /// <summary>
        /// Cancels sequence of promises from the first pending parent towards this promise.
        /// If you have something like this:
        /// var promise = new Promise();
        /// var thenPromise1 = promise.Then(...);
        /// var thenPromise2 = promise.Then(...);
        /// and then you call
        /// thenPromise2.Cancel() then it will cancel promise and thenPromise1 and leave thenPromise2 tangled in pending state.
        /// To fix this you should call either:
        /// thenPromise1.Cancel(); thenPromise2.Cancel();
        /// or promise.CancelSelfAndAllChildren();
        /// </summary>
        void Cancel();
        
        /// <summary>
        /// DO NOT USE it if you aren't absolutely sure about how it works. See Cancel & CancelSelfAndAllChildren methods instead.
        /// Cancels only current promise. In most cases it is not what you might think.
        /// If you have something like this:
        /// var promise = new Promise();
        /// var thenPromise1 = promise.Then(...).Then(...);
        /// and you call thenPromise1.CancelSelf() then all Then callbacks will still be called upon Resolve call.
        /// It only cancels the result promise of calling last .Then
        /// </summary>
        void CancelSelf();
    }
}