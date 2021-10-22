using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace DisposeUtilities
{
    public class DisposableCollection : IDisposable
    {
        private readonly List<IDisposable> _disposables = new List<IDisposable>();       
        
        public void Add(IDisposable disposable)
        {
            if (disposable != null && !_disposables.Contains(disposable))
            {
                _disposables.Add(disposable);
            }
        }

        public bool Remove(IDisposable disposable)
        {
            return _disposables.Remove(disposable);
        }

        public void Dispose()
        {
            _disposables.ToList().ForEach(DisposeAction);
            _disposables.Clear();
        }

        private void DisposeAction(IDisposable x)
        {
            try
            {
                x.Dispose();
            }
            catch (Exception e)
            {
                Debug.LogError("Error when disposing: " + e);
            }
        }
    }
}