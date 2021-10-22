using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DisposeUtilities
{
	public class ComponentDisposeProvider : MonoBehaviour, IDisposeProvider
	{
		public DisposableCollection ChildDisposables => _collection ?? (_collection = new DisposableCollection());
		
		private DisposableCollection _collection;

		private void OnDestroy()
		{
			DisposeService.HandledDispose(this);
		}
	}
}