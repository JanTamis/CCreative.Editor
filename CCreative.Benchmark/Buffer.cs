using System.Buffers;

namespace FileExplorerCore.Helpers;

public ref struct Buffer<T> where T : unmanaged
{
	private T[] _array;
	private Span<T> _span;

	public Buffer(int length)
	{
		_array = ArrayPool<T>.Shared.Rent(length);
		_span = _array.AsSpan(0, length);
	}

	public ref T this[Index index] => ref _span[index];

	public Span<T> this[Range index] => _span[index];

	public static implicit operator Span<T>(Buffer<T> buffer) => buffer._span;
	public static implicit operator ReadOnlySpan<T>(Buffer<T> buffer) => buffer._span;

	public void Dispose()
	{
		if (_array != null)
		{
			ArrayPool<T>.Shared.Return(_array);

			_array = null!;
			_span = default;
		}
	}
}