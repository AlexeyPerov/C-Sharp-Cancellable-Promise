// ReSharper disable once CheckNamespace
namespace DisposeUtilities
{
	public interface IDisposeProvider
	{
		DisposableCollection ChildDisposables { get; }
	}
}