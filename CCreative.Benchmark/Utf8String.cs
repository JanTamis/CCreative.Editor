using System;
using System.Buffers;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Unicode;
using Microsoft.Toolkit.HighPerformance.Buffers;
using Microsoft.Toolkit.HighPerformance.Helpers;

namespace FileExplorerCore.Helpers;

/// <summary>
/// A string that uses the UTF-8 encoding
/// </summary>
public struct Utf8String : IEnumerable<char>
{
	private static readonly Dictionary<int, Utf8String> InternPool = new();

	private readonly byte[] _data;

	private int _length = -1;

	/// <summary>
	/// The character length of this string
	/// </summary>
	public int Length
	{
		get
		{
			if (_length < 0)
			{
				_length = Encoding.UTF8.GetCharCount(_data);
			}

			return _length;
		}
	}

	private int ByteLength => _data.Length;

	/// <summary>
	/// A empty Utf8String
	/// </summary>
	public static readonly Utf8String Empty = new(Array.Empty<byte>(), 0);

	/// <summary>
	/// Creates	a new Utf8String from a set of characters
	/// </summary>
	/// <param name="chars">the characters to create the string from</param>
	public Utf8String(ReadOnlySpan<char> chars)
	{
		if (chars.IsEmpty)
		{
			_data = Array.Empty<byte>();
			_length = 0;

			return;
		}

		if (InternPool.Count > 0)
		{
			var hash = HashCode<char>.Combine(chars);

			if (InternPool.TryGetValue(hash, out var poolItem))
			{
				this = poolItem;
				return;
			}
		}

		_data = new byte[Encoding.UTF8.GetByteCount(chars)];

		Utf8.FromUtf16(chars, _data, out _length, out _);
	}

	/// <summary>
	/// Creates a Utf8String from a UTF-8 file
	/// </summary>
	/// <param name="filepath">the path to the file</param>
	/// <returns>the Utf8String from the file</returns>
	public static Utf8String FromFile(string filepath)
	{
		using var stream = File.OpenRead(filepath);

		return FromStream(stream);
	}

	/// <summary>
	/// Returns a Utf8String from a UTF-8 encoded stream
	/// </summary>
	/// <param name="data">the data to create the string from</param>
	/// <returns>the string from the stream</returns>
	/// <exception cref="ArgumentException">if no data can be read from the stream</exception>
	public static Utf8String FromStream(Stream data)
	{
		if (data.CanRead)
		{
			if (data.CanSeek)
			{
				var buffer = new byte[data.Length];
				data.Read(buffer);

				return new Utf8String(buffer);
			}
			else
			{
				using var buffer = new ArrayPoolList<byte>(1024);

				Span<byte> span = stackalloc byte[1024];

				while (data.Read(span) is var read && read is not 0)
				{
					buffer.AppendSpan(span[..read]);
				}

				return new Utf8String(buffer.ToArray());
			}
		}

		throw new ArgumentException("can't read data from the stream", nameof(data));
	}

	private Utf8String(byte[] data, int length = -1)
	{
		_data = data;
		_length = length;
	}

	private Utf8String(ReadOnlySpan<byte> data, int length = -1) : this(data.ToArray(), length)
	{
	}

	/// <summary>
	/// returns the character from a specific index
	/// </summary>
	/// <param name="index">the index of the character</param>
	/// <exception cref="IndexOutOfRangeException">if the index was out of the bounds</exception>
	[IndexerName("Chars")]
	public char this[Index index]
	{
		get
		{
			var offset = index.GetOffset(Length);

			if (offset >= Length)
			{
				throw new IndexOutOfRangeException(nameof(index));
			}

			Span<char> buffer = stackalloc char[2];

			if (index.IsFromEnd)
			{
				var byteOffset = ByteLength;
				var charOffset = Length - 1;

				while (Rune.DecodeLastFromUtf8(_data.AsSpan(0, byteOffset), out var rune, out var bytesConsumed) is OperationStatus.Done)
				{
					if (charOffset <= offset)
					{
						rune.EncodeToUtf16(buffer);
						return buffer[charOffset - offset];
					}

					byteOffset -= bytesConsumed;
					charOffset -= rune.Utf16SequenceLength;
				}
			}
			else
			{
				var byteOffset = 0;
				var charOffset = 0;

				while (Rune.DecodeFromUtf8(_data.AsSpan(byteOffset), out var rune, out var bytesConsumed) is OperationStatus.Done)
				{
					if (charOffset >= offset)
					{
						rune.EncodeToUtf16(buffer);
						return buffer[charOffset - offset];
					}

					byteOffset += bytesConsumed;
					charOffset += rune.Utf16SequenceLength;
				}
			}

			return '\0';
		}
	}

	/// <summary>
	/// Returns a subset of the string specified by the range
	/// </summary>
	/// <param name="range">the range of the subset</param>
	[IndexerName("Chars")]
	public Utf8String this[Range range]
	{
		get
		{
			var (offset, length) = range.GetOffsetAndLength(Length);

			return Substring(offset, length);
		}
	}

	/// <summary>
	/// Creates a new string with a specific length (in bytes) and initializes it after creation by using the specified callback  
	/// </summary>
	/// <param name="length">the length of the specified Utf8String (in bytes)</param>
	/// <param name="state">the state to propagate to the action</param>
	/// <param name="action">the action that will be invoked while making the string</param>
	/// <typeparam name="TState">The type of the element to pass to <param name="action"></param></typeparam>
	/// <returns>a immutable Utf8String</returns>
	public static Utf8String Create<TState>(int length, TState state, SpanAction<byte, TState> action)
	{
		var buffer = new byte[length];

		action(buffer, state);

		return new Utf8String(buffer);
	}

	/// <summary>
	/// Creates a string from String Interpolation
	/// </summary>
	/// <remarks>See https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/tokens/interpolated for more information</remarks>
	/// <param name="handler">the interpolated string handler</param>
	public static Utf8String Create(ref Utf8InterpolatedStringHandler handler)
	{
		return new Utf8String(handler.ToArrayAndClear());
	}

	/// <summary>
	/// Creates a string from String Interpolation
	/// </summary>
	/// <remarks>See https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/tokens/interpolated for more information</remarks>
	/// <param name="buffer">the temporary buffer the use while creating the string</param>
	/// <param name="handler">the interpolated string handler</param>
	public static Utf8String Create(Span<byte> buffer, [InterpolatedStringHandlerArgument("buffer")] ref Utf8InterpolatedStringHandler handler)
	{
		return new Utf8String(handler.ToArrayAndClear());
	}

	#region Concat

	/// <summary>
	/// Combines two strings
	/// </summary>
	/// <param name="str0">the first string</param>
	/// <param name="str1">the second string</param>
	/// <returns>the combined string</returns>
	public static Utf8String Concat(Utf8String str0, Utf8String str1)
	{
		var length = str0.ByteLength + str1.ByteLength;
		var buffer = new byte[length];

		str0.CopyTo(buffer);
		str1.CopyTo(buffer.AsSpan(str0.ByteLength));

		return new Utf8String(buffer, str0.Length + str1.Length);
	}

	/// <summary>
	/// Combines three strings
	/// </summary>
	/// <param name="str0">the first string</param>
	/// <param name="str1">the second string</param>
	/// <param name="str2">the third string</param>
	/// <returns>the combined string</returns>
	public static Utf8String Concat(Utf8String str0, Utf8String str1, Utf8String str2)
	{
		var length = str0.ByteLength + str1.ByteLength + str2.ByteLength;
		var buffer = new byte[length];

		str0.CopyTo(buffer.AsSpan());
		str1.CopyTo(buffer.AsSpan(str0.ByteLength));
		str2.CopyTo(buffer.AsSpan(str0.ByteLength + str1.ByteLength));

		return new Utf8String(buffer, str0.Length + str1.Length + str2.Length);
	}

	/// <summary>
	/// Combines four strings
	/// </summary>
	/// <param name="str0">the first String</param>
	/// <param name="str1">the second String</param>
	/// <param name="str2">the third String</param>
	/// <param name="str3">the fourth String</param>
	/// <returns></returns>
	public static Utf8String Concat(Utf8String str0, Utf8String str1, Utf8String str2, Utf8String str3)
	{
		var length = str0.ByteLength + str1.ByteLength + str2.ByteLength + str3.ByteLength;
		var buffer = new byte[length];

		str0.CopyTo(buffer.AsSpan());
		str1.CopyTo(buffer.AsSpan(str0.ByteLength));
		str2.CopyTo(buffer.AsSpan(str0.ByteLength + str1.ByteLength));
		str3.CopyTo(buffer.AsSpan(str0.ByteLength + str1.ByteLength + str2.ByteLength));

		return new Utf8String(buffer, str0.Length + str1.Length + str2.Length + str3.Length);
	}

	/// <summary>
	/// Combines a array of strings
	/// </summary>
	/// <param name="strings">the array of strings to combine</param>
	/// <returns>the combined string</returns>
	public static Utf8String Concat(params Utf8String[] strings)
	{
		var length = 0;
		var byteLength = 0;

		foreach (var item in strings)
		{
			length += item.Length;
			byteLength += item.ByteLength;
		}

		var buffer = new byte[byteLength];
		var offset = 0;

		foreach (var str in strings)
		{
			str.CopyTo(buffer.AsSpan(offset));
			offset += str.ByteLength;
		}

		return new Utf8String(buffer, length);
	}

	/// <summary>
	/// Combines a IEnumerable of strings
	/// </summary>
	/// <param name="strings"></param>
	/// <returns></returns>
	public static Utf8String Concat(IEnumerable<Utf8String> strings)
	{
		using var builder = new ValueBuilder<byte>(stackalloc byte[1024]);

		foreach (var str in strings)
		{
			builder.AppendSpan(str.AsBytes());
		}

		return new Utf8String(builder.AsSpan());
	}

	#endregion

	#region Trimming

	/// <summary>
	/// Removes the whitespace from the beginning and the end of the string
	/// </summary>
	/// <returns>the trimmed string</returns>
	public Utf8String Trim()
	{
		var startOffset = 0;
		var byteLength = ByteLength;
		var length = Length;

		while (Rune.DecodeFromUtf8(_data.AsSpan(startOffset), out var rune, out var bytesConsumed) is OperationStatus.Done && Rune.IsWhiteSpace(rune))
		{
			startOffset += bytesConsumed;
			length--;
		}

		while (Rune.DecodeLastFromUtf8(_data.AsSpan(0, byteLength), out var rune, out var bytesConsumed) is OperationStatus.Done && Rune.IsWhiteSpace(rune))
		{
			byteLength -= bytesConsumed;
			length--;
		}

		return new Utf8String(_data[startOffset..byteLength], length);
	}

	/// <summary>
	/// Removes the whitespace of the beginning of the string
	/// </summary>
	/// <returns>the trimmed string</returns>
	public Utf8String TrimStart()
	{
		var startOffset = 0;
		var byteLength = ByteLength;
		var length = Length;

		while (Rune.DecodeFromUtf8(_data.AsSpan(startOffset), out var rune, out var bytesConsumed) is OperationStatus.Done && Rune.IsWhiteSpace(rune))
		{
			startOffset += bytesConsumed;
			length--;
		}

		return new Utf8String(_data[startOffset..byteLength], length);
	}

	/// <summary>
	/// Removes the whitespace of the end of the string
	/// </summary>
	/// <returns>the trimmed string</returns>
	public Utf8String TrimEnd()
	{
		var byteLength = ByteLength;
		var length = Length;

		while (Rune.DecodeLastFromUtf8(_data.AsSpan(0, byteLength), out var rune, out var bytesConsumed) is OperationStatus.Done && Rune.IsWhiteSpace(rune))
		{
			byteLength -= bytesConsumed;
			length--;
		}

		return new Utf8String(_data[..byteLength], length);
	}

	#endregion

	#region Remove

	/// <summary>
	/// Returns a new string in which a specified number of characters in the current instance beginning at a specified position have been deleted.
	/// </summary>
	/// <param name="startIndex">the zero-based position to begin deleting characters</param>
	/// <returns>a new string that is equivalent to this instance except for the removed characters</returns>
	public Utf8String Remove(int startIndex)
	{
		return Substring(0, startIndex);
	}

	/// <summary>
	/// Returns a new string in which a specified number of characters in the current instance beginning at a specified position have been deleted
	/// </summary>
	/// <param name="startIndex">the zero-based position to begin deleting characters</param>
	/// <param name="count">the number of characters to delete</param>
	/// <returns>a new string that is equivalent to this instance except for the removed characters</returns>
	public Utf8String Remove(int startIndex, int count)
	{
		var byteStartOffset = 0;

		for (var i = 0; i < startIndex; i++)
		{
			var result = TryGetCount(_data, byteStartOffset);

			if (result != -1)
			{
				byteStartOffset += result;
			}
		}

		var byteEndOffset = byteStartOffset;

		for (var i = 0; i < count; i++)
		{
			var result = TryGetCount(_data, byteEndOffset);

			if (result != -1)
			{
				byteEndOffset += result;
			}
		}

		var buffer = new byte[Length - (byteEndOffset - byteStartOffset)];

		AsBytes().Slice(0, byteStartOffset).CopyTo(buffer);
		AsBytes().Slice(byteEndOffset).CopyTo(buffer.AsSpan(byteStartOffset));

		// using var builder = new ArrayPoolList<char>(Length - count);
		// using var buffer = SpanOwner<char>.Allocate(Length);
		//
		// CopyTo(buffer.Span);
		//
		// buffer.Span[..startIndex].CopyTo(builder.AppendSpan(startIndex));
		// buffer.Span[(startIndex + count)..].CopyTo(builder.AppendSpan(Length - startIndex - count));
		//
		// return new Utf8String(builder.AsSpan());

		return new Utf8String(buffer);
	}

	#endregion

	#region Substring

	/// <summary>
	/// Returns a part of the string from the specified index to the end of the string
	/// </summary>
	/// <param name="startIndex">the start of the substring</param>
	/// <returns>the substring</returns>
	public Utf8String Substring(int startIndex)
	{
		var byteStartOffset = 0;

		for (var i = 0; i < startIndex; i++)
		{
			var result = TryGetCount(_data, byteStartOffset);

			if (result != -1)
			{
				byteStartOffset += result;
			}
		}

		return new Utf8String(_data[byteStartOffset..], Length - startIndex);
	}

	/// <summary>
	/// Returns a part of the string from the specified index with the specified length
	/// </summary>
	/// <param name="startIndex">the start of the substring</param>
	/// <param name="length">the length of the substring</param>
	/// <returns>the substring</returns>
	public Utf8String Substring(int startIndex, int length)
	{
		var byteStartOffset = 0;

		for (var i = 0; i < startIndex; i++)
		{
			var result = TryGetCount(_data, byteStartOffset);

			if (result != -1)
			{
				byteStartOffset += result;
			}
		}

		var byteEndOffset = byteStartOffset;

		for (var i = 0; i < length; i++)
		{
			var result = TryGetCount(_data, byteEndOffset);

			if (result != -1)
			{
				byteEndOffset += result;
			}
		}

		return new Utf8String(_data[byteStartOffset..(byteEndOffset - byteStartOffset)], length);
	}

	#endregion

	#region Format

	/// <summary>
	/// Tries to format the specified value
	/// </summary>
	/// <param name="value">the value to try to format</param>
	/// <param name="result">the formatted string</param>
	/// <typeparam name="T">the type of the <see cref="value"/></typeparam>
	/// <returns>if the value was successfully formatted</returns>
	public static bool TryFormat<T>(T value, out Utf8String result) where T : struct
	{
		Span<byte> buffer = stackalloc byte[128];

		var written = 0;
		var succeeded = typeof(T) == typeof(DateTime) && Utf8Formatter.TryFormat((DateTime)(object)value, buffer, out written) ||
		                typeof(T) == typeof(DateTimeOffset) && Utf8Formatter.TryFormat((DateTimeOffset)(object)value, buffer, out written) ||
		                typeof(T) == typeof(Guid) && Utf8Formatter.TryFormat((Guid)(object)value, buffer, out written) ||
		                typeof(T) == typeof(TimeSpan) && Utf8Formatter.TryFormat((TimeSpan)(object)value, buffer, out written) ||
		                typeof(T) == typeof(bool) && Utf8Formatter.TryFormat((bool)(object)value, buffer, out written) ||
		                typeof(T) == typeof(byte) && Utf8Formatter.TryFormat((byte)(object)value, buffer, out written) ||
		                typeof(T) == typeof(decimal) && Utf8Formatter.TryFormat((decimal)(object)value, buffer, out written) ||
		                typeof(T) == typeof(double) && Utf8Formatter.TryFormat((double)(object)value, buffer, out written) ||
		                typeof(T) == typeof(float) && Utf8Formatter.TryFormat((float)(object)value, buffer, out written) ||
		                typeof(T) == typeof(int) && Utf8Formatter.TryFormat((int)(object)value, buffer, out written) ||
		                typeof(T) == typeof(long) && Utf8Formatter.TryFormat((long)(object)value, buffer, out written) ||
		                typeof(T) == typeof(sbyte) && Utf8Formatter.TryFormat((sbyte)(object)value, buffer, out written) ||
		                typeof(T) == typeof(short) && Utf8Formatter.TryFormat((short)(object)value, buffer, out written) ||
		                typeof(T) == typeof(uint) && Utf8Formatter.TryFormat((uint)(object)value, buffer, out written) ||
		                typeof(T) == typeof(ulong) && Utf8Formatter.TryFormat((ulong)(object)value, buffer, out written) ||
		                typeof(T) == typeof(ushort) && Utf8Formatter.TryFormat((ushort)(object)value, buffer, out written);

		result = new Utf8String(buffer[..written]);
		return succeeded;
	}

	/// <summary>
	/// Tries to format the specified value
	/// </summary>
	/// <param name="value">the value to try to format</param>
	/// <typeparam name="T">the type of the <see cref="value"/></typeparam>
	/// <returns>the formatted string</returns>
	/// <exception cref="InvalidOperationException">if the value could not be formatted</exception>
	public static Utf8String Format<T>(T value) where T : struct
	{
		if (TryFormat(value, out var result))
		{
			return result;
		}

		throw new InvalidOperationException($"Unable to format value of type {typeof(T).FullName}");
	}

	#endregion

	/// <summary>
	/// Returns a array of the characters of this string
	/// </summary>
	/// <returns>a array of the characters of this string</returns>
	public char[] ToCharArray()
	{
		var array = new char[Length];

		CopyTo(array);

		return array;
	}

	/// <summary>
	/// Combines a array of strings and insert a character between the strings
	/// </summary>
	/// <param name="separator">the character to insert between the strings</param>
	/// <param name="strings">the strings the combine</param>
	/// <returns>a string that contains the combination of the strings seperated bij the separator</returns>
	public static Utf8String Join(char separator, params Utf8String[] strings)
	{
		var length = strings.Length - 1;
		var byteLength = 0;

		var rune = new Rune(separator);

		foreach (var item in strings)
		{
			length += item.Length;
			byteLength += item.ByteLength;
		}

		using var builder = new ArrayPoolList<byte>(byteLength);

		for (var i = 0; i < strings.Length; i++)
		{
			var str = strings[i];

			str.CopyTo(builder.AppendSpan(str.ByteLength));

			if (i < strings.Length - 1)
			{
				rune.EncodeToUtf8(builder.AppendSpan(rune.Utf8SequenceLength));
			}
		}

		return new Utf8String(builder.ToArray(), length);
	}

	/// <summary>
	/// Returns if the string is empty
	/// </summary>
	/// <returns>if the string is empty</returns>
	public bool IsEmpty()
	{
		return ByteLength is 0;
	}

	/// <summary>
	/// Returns if the string if empty or only contains whitespace characters
	/// </summary>
	/// <returns>if the string if empty or only contains whitespace characters</returns>
	public bool IsEmptyOrWhiteSpace()
	{
		if (IsEmpty())
		{
			return true;
		}

		foreach (var rune in EnumerateRunes())
		{
			if (Rune.IsWhiteSpace(rune))
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Returns if this string contains the specified character
	/// </summary>
	/// <param name="value">the specified character</param>
	/// <returns>if the string contains the specified character</returns>
	public bool Contains(char value)
	{
		using var buffer = SpanOwner<char>.Allocate(Length);
		CopyTo(buffer.Span);

		return buffer.Span.Contains(value);
	}

	public int IndexOf(char value)
	{
		using var buffer = SpanOwner<char>.Allocate(Length);
		CopyTo(buffer.Span);

		return buffer.Span.IndexOf(value);
	}

	public int IndexOf(char value, int startIndex)
	{
		using var buffer = SpanOwner<char>.Allocate(Length);
		CopyTo(buffer.Span);

		return buffer.Span.Slice(startIndex).IndexOf(value);
	}

	public int IndexOf(char value, int startIndex, int count)
	{
		using var buffer = SpanOwner<char>.Allocate(Length);
		CopyTo(buffer.Span);

		return buffer.Span.Slice(startIndex, count).IndexOf(value);
	}

	public int LastIndexOf(char value)
	{
		using var buffer = SpanOwner<char>.Allocate(Length);
		CopyTo(buffer.Span);

		return buffer.Span.IndexOf(value);
	}

	public int LastIndexOf(char value, int startIndex)
	{
		using var buffer = SpanOwner<char>.Allocate(Length);
		CopyTo(buffer.Span);

		return buffer.Span.Slice(startIndex).LastIndexOf(value);
	}

	public int LastIndexOf(char value, int startIndex, int count)
	{
		using var buffer = SpanOwner<char>.Allocate(Length);
		CopyTo(buffer.Span);

		return buffer.Span.Slice(startIndex, count).LastIndexOf(value);
	}

	public int IndexOfAny(ReadOnlySpan<char> anyOf)
	{
		var index = 0;

		foreach (var item in this)
		{
			if (anyOf.Contains(item))
				return index;

			index++;
		}

		return -1;
	}

	public bool EndsWith(char value)
	{
		Span<byte> buffer = stackalloc byte[4];
		var rune = new Rune(value);

		var bytesWritten = rune.EncodeToUtf8(buffer);

		return AsBytes().EndsWith(buffer[..bytesWritten]);
	}

	public bool EndsWith(Utf8String value)
	{
		return AsBytes().EndsWith(value.AsBytes());
	}

	public ref readonly byte GetPinnableReference()
	{
		return ref MemoryMarshal.GetArrayDataReference(_data);
	}

	public IEnumerable<Utf8String> Split(char separator, StringSplitOptions options = StringSplitOptions.None)
	{
		var byteOffset = 0;
		var previousOffset = 0;

		var buffer = new char[2];

		while (Rune.DecodeFromUtf8(_data.AsSpan(byteOffset), out var rune, out var bytesConsumed) is OperationStatus.Done)
		{
			var charCount = rune.EncodeToUtf16(buffer);

			for (var i = 0; i < charCount; i++)
			{
				var charByteCount = Math.Min(bytesConsumed, 2);

				if (buffer[i] == separator)
				{
					var length = byteOffset - previousOffset;

					if (!options.HasFlag(StringSplitOptions.RemoveEmptyEntries) || length > 0)
					{
						yield return options.HasFlag(StringSplitOptions.TrimEntries)
							? new Utf8String(_data.AsSpan(previousOffset, length)).Trim()
							: new Utf8String(_data.AsSpan(previousOffset, length));
					}

					previousOffset = byteOffset + charByteCount;
				}

				byteOffset += charByteCount;
				bytesConsumed -= charByteCount;
			}
		}

		if (!options.HasFlag(StringSplitOptions.RemoveEmptyEntries) || previousOffset < _data.Length)
		{
			yield return new Utf8String(_data[previousOffset..]);
		}
	}

	public IEnumerable<Utf8String> Split(char[] separators, StringSplitOptions options = StringSplitOptions.None)
	{
		var byteOffset = 0;
		var previousOffset = 0;

		var buffer = new char[2];

		while (Rune.DecodeFromUtf8(_data.AsSpan(byteOffset), out var rune, out var bytesConsumed) is OperationStatus.Done)
		{
			var charCount = rune.EncodeToUtf16(buffer);

			for (var i = 0; i < charCount; i++)
			{
				var charByteCount = Math.Min(bytesConsumed, 2);

				if (Array.IndexOf(separators, buffer[i]) > -1)
				{
					var length = byteOffset - previousOffset;

					if (!options.HasFlag(StringSplitOptions.RemoveEmptyEntries) || length > 0)
					{
						yield return options.HasFlag(StringSplitOptions.TrimEntries)
							? new Utf8String(_data.AsSpan(previousOffset, length)).Trim()
							: new Utf8String(_data.AsSpan(previousOffset, length));
					}

					previousOffset = byteOffset + charByteCount;
				}

				byteOffset += charByteCount;
				bytesConsumed -= charByteCount;
			}
		}

		if (!options.HasFlag(StringSplitOptions.RemoveEmptyEntries) || previousOffset < _data.Length)
		{
			yield return new Utf8String(_data[previousOffset..]);
		}
	}

	public Utf8String ToUpper()
	{
		var result = new byte[ByteLength];
		var offset = 0;

		foreach (var rune in EnumerateRunes())
		{
			offset += Rune
				.ToUpper(rune, CultureInfo.CurrentCulture)
				.EncodeToUtf8(result.AsSpan(offset..));
		}

		return new Utf8String(result, _length);
	}

	public Utf8String ToUpperInvariant()
	{
		var result = new byte[ByteLength];
		var offset = 0;

		foreach (var rune in EnumerateRunes())
		{
			offset += Rune
				.ToUpperInvariant(rune)
				.EncodeToUtf8(result.AsSpan(offset..));
		}

		return new Utf8String(result, _length);
	}

	public Utf8String ToLower()
	{
		var result = new byte[ByteLength];
		var offset = 0;

		foreach (var rune in EnumerateRunes())
		{
			offset += Rune
				.ToLower(rune, CultureInfo.CurrentCulture)
				.EncodeToUtf8(result.AsSpan(offset..));
		}

		return new Utf8String(result, _length);
	}

	public Utf8String ToLowerInvariant()
	{
		var result = new byte[ByteLength];
		var offset = 0;

		foreach (var rune in EnumerateRunes())
		{
			offset += Rune
				.ToLowerInvariant(rune)
				.EncodeToUtf8(result.AsSpan(offset..));
		}

		return new Utf8String(result, _length);
	}

	public bool IsInterned()
	{
		using var buffer = SpanOwner<char>.Allocate(Length);
		CopyTo(buffer.Span);

		return InternPool.ContainsKey(HashCode<char>.Combine(buffer.Span));
	}

	public Utf8String Intern()
	{
		using var buffer = SpanOwner<char>.Allocate(Length);
		CopyTo(buffer.Span);

		var hash = HashCode<char>.Combine(buffer.Span);

		if (!InternPool.ContainsKey(hash))
		{
			InternPool.Add(hash, this);
		}

		return this;
	}

	public Utf8String PadLeft(int totalWidth, char paddingChar = ' ')
	{
		if (totalWidth <= Length)
		{
			return this;
		}

		var rune = new Rune(paddingChar);

		Span<byte> buffer = stackalloc byte[rune.Utf8SequenceLength];
		rune.EncodeToUtf8(buffer);

		var resultBuffer = new byte[ByteLength + buffer.Length * (totalWidth - Length)];

		CopyTo(resultBuffer.AsSpan(buffer.Length * (totalWidth - Length)));

		var temp = resultBuffer.AsSpan(0, buffer.Length * (totalWidth - Length));

		if (buffer.Length == 1)
		{
			temp.Fill(buffer[0]);
		}
		else
		{
			while (buffer.TryCopyTo(temp))
			{
				temp = temp[buffer.Length..];
			}
		}

		return new Utf8String(resultBuffer);
	}

	public Utf8String PadRight(int totalWidth, char paddingChar = ' ')
	{
		if (totalWidth <= Length)
		{
			return this;
		}

		var rune = new Rune(paddingChar);

		Span<byte> buffer = stackalloc byte[rune.Utf8SequenceLength];
		rune.EncodeToUtf8(buffer);

		var resultBuffer = new byte[ByteLength + buffer.Length * (totalWidth - Length)];

		CopyTo(resultBuffer.AsSpan(0, ByteLength));

		var temp = resultBuffer.AsSpan(ByteLength);

		if (buffer.Length == 1)
		{
			temp.Fill(buffer[0]);
		}
		else
		{
			while (buffer.TryCopyTo(temp))
			{
				temp = temp[buffer.Length..];
			}
		}

		return new Utf8String(resultBuffer);
	}

	public void CopyTo(Span<char> span)
	{
		Utf8.ToUtf16(_data, span, out _, out _, false, false);
	}

	public bool TryCopyTo(Span<char> span)
	{
		return Utf8.ToUtf16(_data, span, out _, out _, false, false) is OperationStatus.Done;
	}

	public void CopyTo(Span<byte> span)
	{
		_data.CopyTo(span);
	}

	public bool TryCopyTo(Span<byte> span)
	{
		return AsBytes().TryCopyTo(span);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<byte> AsBytes()
	{
		return _data.AsSpan();
	}

	public override string ToString()
	{
		return Encoding.UTF8.GetString(_data);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(HashCode<byte>.Combine(_data), Length);
	}

	public IEnumerable<Rune> EnumerateRunes()
	{
		var byteCount = 0;

		while (Rune.DecodeFromUtf8(_data.AsSpan(byteCount), out var rune, out var bytesConsumed) is OperationStatus.Done)
		{
			yield return rune;

			byteCount += bytesConsumed;
		}
	}

	public IEnumerable<Rune> EnumerateRunesReversed()
	{
		var byteCount = ByteLength;

		while (Rune.DecodeLastFromUtf8(_data.AsSpan(0, byteCount), out var rune, out var bytesConsumed) is OperationStatus.Done)
		{
			yield return rune;

			byteCount -= bytesConsumed;
		}
	}

	public IEnumerator<char> GetEnumerator()
	{
		var buffer = ArrayPool<char>.Shared.Rent(2);

		foreach (var rune in EnumerateRunes())
		{
			var count = rune.EncodeToUtf16(buffer);

			if (count > 0)
			{
				yield return MemoryMarshal.GetArrayDataReference(buffer);
			}

			if (count > 1)
			{
				yield return Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(buffer), 1);
			}
		}

		ArrayPool<char>.Shared.Return(buffer);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public bool Equals(Utf8String x, Utf8String y)
	{
		return x.AsBytes().SequenceEqual(y.AsBytes());
	}

	public static bool operator ==(Utf8String x, Utf8String y)
	{
		return x.AsBytes().SequenceEqual(y.AsBytes());
	}

	public static bool operator !=(Utf8String x, Utf8String y)
	{
		return !(x == y);
	}

	public static Utf8String operator +(Utf8String x, Utf8String y)
	{
		return Concat(x, y);
	}

	public override bool Equals(object? obj)
	{
		return obj is Utf8String dynamicString && Equals(dynamicString, this);
	}

	private static int TryGetCount(ReadOnlySpan<byte> buffer, int index)
	{
		if (index >= buffer.Length)
			return -1;

		var x = buffer[index];

		var byteCount =
			x < 192 ? 1 :
			x < 224 ? 2 :
			x < 240 ? 3 :
			4;

		if (index + byteCount > buffer.Length)
			return -1;

		return byteCount;
	}
}