using System;
using System.Buffers;
using System.Buffers.Text;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Unicode;

#nullable enable
namespace FileExplorerCore.Helpers
{
	[InterpolatedStringHandler]
	public ref struct Utf8InterpolatedStringHandler
	{
		// Storage for the built-up string
		ValueBuilder<byte> builder;

		public Utf8InterpolatedStringHandler(int literalLength, int formattedCount, Span<byte> buffer)
		{
			builder = new ValueBuilder<byte>(buffer);
		}

		public Utf8InterpolatedStringHandler(int literalLength, int formattedCount)
		{
			builder = new ValueBuilder<byte>(literalLength + formattedCount * 11);
		}

		public void AppendLiteral(ReadOnlySpan<char> s)
		{
			var span = builder.AppendSpan(Encoding.UTF8.GetByteCount(s));

			Utf8.FromUtf16(s, span, out _, out _, false, false);
		}

		public void AppendFormatted<T>(T t)
		{
			AppendFormattedInternal(t, default);
		}

		public void AppendFormatted<T>(T t, string format)
		{
			if (StandardFormat.TryParse(format, out var standardFormat))
			{
				AppendFormattedInternal(t, standardFormat);
			}
		}

		private void AppendFormattedInternal<T>(T t, StandardFormat standardFormat)
		{
			if (t is null)
			{
				return;
			}

			var written = 0;
			var span = builder.RawChars[builder.Length..];

			var succeeded = typeof(T) == typeof(DateTime) && Utf8Formatter.TryFormat((DateTime)(object)t, span, out written, standardFormat) ||
			                typeof(T) == typeof(DateTimeOffset) && Utf8Formatter.TryFormat((DateTimeOffset)(object)t, span, out written, standardFormat) ||
			                typeof(T) == typeof(Guid) && Utf8Formatter.TryFormat((Guid)(object)t, span, out written, standardFormat) ||
			                typeof(T) == typeof(TimeSpan) && Utf8Formatter.TryFormat((TimeSpan)(object)t, span, out written, standardFormat) ||
			                typeof(T) == typeof(bool) && Utf8Formatter.TryFormat((bool)(object)t, span, out written, standardFormat) ||
			                typeof(T) == typeof(byte) && Utf8Formatter.TryFormat((byte)(object)t, span, out written, standardFormat) ||
			                typeof(T) == typeof(sbyte) && Utf8Formatter.TryFormat((sbyte)(object)t, span, out written, standardFormat) ||
			                typeof(T) == typeof(short) && Utf8Formatter.TryFormat((short)(object)t, span, out written, standardFormat) ||
			                typeof(T) == typeof(ushort) && Utf8Formatter.TryFormat((ushort)(object)t, span, out written, standardFormat) ||
			                typeof(T) == typeof(int) && Utf8Formatter.TryFormat((int)(object)t, span, out written, standardFormat) ||
			                typeof(T) == typeof(uint) && Utf8Formatter.TryFormat((uint)(object)t, span, out written, standardFormat) ||
			                typeof(T) == typeof(long) && Utf8Formatter.TryFormat((long)(object)t, span, out written, standardFormat) ||
			                typeof(T) == typeof(ulong) && Utf8Formatter.TryFormat((ulong)(object)t, span, out written, standardFormat) ||
			                typeof(T) == typeof(float) && Utf8Formatter.TryFormat((float)(object)t, span, out written, standardFormat) ||
			                typeof(T) == typeof(double) && Utf8Formatter.TryFormat((double)(object)t, span, out written, standardFormat) ||
			                typeof(T) == typeof(decimal) && Utf8Formatter.TryFormat((decimal)(object)t, span, out written, standardFormat);


			if (succeeded)
			{
				builder.Position += written;
			}
			else
			{
				AppendLiteral(t.ToString());
			}
		}

		internal byte[] ToArrayAndClear()
		{
			var bytes = builder.ToArray();
			builder.Dispose();

			return bytes;
		}
	}
}