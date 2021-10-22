using System;
using System.Diagnostics;

// ReSharper disable once CheckNamespace
namespace DisposeUtilities
{
	public class DeferredDisposable : IDisposable
	{
		private readonly Action _disposeAction;

		public DeferredDisposable(Action disposeAction)
		{
			_disposeAction = disposeAction;
		}

		public void Dispose()
		{
            try
            {
                _disposeAction?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
	}
}