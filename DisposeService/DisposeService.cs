using System;
using RSG;

// ReSharper disable once CheckNamespace
namespace DisposeUtilities
{
    public static class DisposeService
    {
        public static T DisposeWith<T>(this T disposable, IDisposeProvider provider) where T : IDisposable
        {
            if (provider == null)
            {
                disposable.Dispose();
                return disposable;
            }

            provider.ChildDisposables.Add(disposable);
            return disposable;
        }

        public static T DisposeWith<T>(this T disposable, IPromise promise) where T : IDisposable
        {
            if (promise.CurState != PromiseState.Pending)
            {
                disposable?.Dispose();
                return disposable;
            }

            promise.Finally(() => disposable?.Dispose());
            return disposable;
        }
        
        public static T DisposeWith<T, TPromise>(this T disposable, IPromise<TPromise> promise) where T : IDisposable
        {
            if (promise.CurState != PromiseState.Pending)
            {
                disposable?.Dispose();
                return disposable;
            }

            promise.Finally(() => disposable?.Dispose());
            return disposable;
        }
        
        public static T DisposeWith<T>(this T disposable, MonoBehaviour provider) where T : IDisposable
        {
            if (provider.IsDestroyed())
            {
                disposable.Dispose();
                return disposable;
            }
            
            IDisposeProvider disposer = provider.gameObject.RequireComponent<ComponentDisposeProvider>();
            return disposable.DisposeWith(disposer);
        }
        
        public static T CancelWith<T>(this T cancelable, IDisposeProvider provider) where T : ICancellablePromise
        {
            new DeferredDisposable(cancelable.Cancel).DisposeWith(provider);
            return cancelable;
        }
        
        public static T CancelWith<T>(this T cancelable, MonoBehaviour provider) where T : ICancellablePromise
        {
            new DeferredDisposable(cancelable.Cancel).DisposeWith(provider);
            return cancelable;
        }

        public static T CancelWith<T>(this T cancelable, IPromise promise) where T : ICancellablePromise
        {
            promise.OnCancel(cancelable.Cancel);
            return cancelable;
        }
        
        public static T CancelWith<T, TPromise>(this T cancelable, IPromise<TPromise> promise) where T : ICancellablePromise
        {
            promise.OnCancel(cancelable.Cancel);
            return cancelable;
        }

        public static void HandledDispose(IDisposeProvider provider)
        {
            provider.ChildDisposables.Dispose();
        }
    }
}