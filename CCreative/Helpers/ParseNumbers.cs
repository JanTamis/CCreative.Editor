using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using SixLabors.ImageSharp;

namespace CCreative.Helpers;

[RequiresPreviewFeatures]
internal static class ParseNumbers
{
	internal const int LeftAlign = 0x0001;
	internal const int RightAlign = 0x0004;
	internal const int PrefixSpace = 0x0008;
	internal const int PrintSign = 0x0010;
	internal const int PrintBase = 0x0020;
	internal const int PrintAsI1 = 0x0040;
	internal const int PrintAsI2 = 0x0080;
	internal const int PrintAsI4 = 0x0100;
	internal const int TreatAsUnsigned = 0x0200;
	internal const int TreatAsI1 = 0x0400;
	internal const int TreatAsI2 = 0x0800;
	internal const int IsTight = 0x1000;
	internal const int NoSpace = 0x2000;
	internal const int PrintRadixBase = 0x4000;

	private const int MinRadix = 2;
	private const int MaxRadix = 36;

	public static long StringToLong(ReadOnlySpan<char> s, int radix, int flags)
	{
		var pos = 0;
		return StringToLong(s, radix, flags, ref pos);
	}

	public static long StringToLong(ReadOnlySpan<char> s, int radix, int flags, ref int currPos)
	{
		var i = currPos;

		// Do some radix checking.
		// A radix of -1 says to use whatever base is spec'd on the number.
		// Parse in Base10 until we figure out what the base actually is.
		var r = -1 == radix ? 10 : radix;

		var length = s.Length;

		// Get rid of the whitespace and then check that we've still got some digits to parse.
		if ((flags & IsTight) == 0 && (flags & NoSpace) == 0)
		{
			EatWhiteSpace(s, ref i);
		}

		// Check for a sign
		var sign = 1;
		switch (s[i])
		{
			case '-':
				sign = -1;
				i++;
				break;
			case '+':
				i++;
				break;
		}

		if (radix is -1 or 16 && i + 1 < length && s[i] == '0')
		{
			if (s[i + 1] == 'x' || s[i + 1] == 'X')
			{
				r = 16;
				i += 2;
			}
		}

		var grabNumbersStart = i;
		var result = GrabLongs(r, s, ref i, (flags & TreatAsUnsigned) != 0);


		// Put the current index back into the correct place.
		currPos = i;


		if (r == 10)
		{
			result *= sign;
		}

		return result;
	}

	public static int StringToInt(ReadOnlySpan<char> s, int radix, int flags = IsTight)
	{
		var pos = 0;
		return StringToInt(s, radix, flags, ref pos);
	}

	public static int StringToInt(ReadOnlySpan<char> s, int radix, int flags, ref int currPos)
	{
		// They're required to tell me where to start parsing.
		var i = currPos;

		// Do some radix checking.
		// A radix of -1 says to use whatever base is spec'd on the number.
		// Parse in Base10 until we figure out what the base actually is.
		var r = -1 == radix ? 10 : radix;

		var length = s.Length;

		// Get rid of the whitespace and then check that we've still got some digits to parse.
		if ((flags & IsTight) == 0 && (flags & NoSpace) == 0)
		{
			EatWhiteSpace(s, ref i);
		}

		// Check for a sign
		var sign = 1;
		switch (s[i])
		{
			case '-':
				sign = -1;
				i++;
				break;
			case '+':
				i++;
				break;
		}

		// Consume the 0x if we're in an unknown base or in base-16.
		if (radix is -1 or 16 && i + 1 < length && s[i] == '0')
		{
			if (s[i + 1] is 'x' or 'X')
			{
				r = 16;
				i += 2;
			}
		}

		var result = GrabInts(r, s, ref i, (flags & TreatAsUnsigned) != 0);

		// Put the current index back into the correct place.
		currPos = i;

		if (r == 10)
		{
			result *= sign;
		}

		return result;
	}

	public static unsafe ReadOnlySpan<char> BinaryIntegerToString<T>(T n, int radix, int width = -1, char paddingChar = ' ', int flags = 0) where T : IBinaryInteger<T>, IConvertible
	{
		Span<char> buffer = stackalloc char[67]; // Longest possible string length for an integer in binary notation with prefix

		// If the number is negative, make it positive and remember the sign.
		// If the number is MIN_VALUE, this will still be negative, so we'll have to
		// special case this later.
		var isNegative = false;
		uint l;
		if (n < T.Zero)
		{
			isNegative = true;

			// For base 10, write out -num, but other bases write out the
			// 2's complement bit pattern
			l = 10 == radix ? (-n).ToUInt32(null) : n.ToUInt32(null);
		}
		else
		{
			l = n.ToUInt32(null);
		}

		// The conversion to a uint will sign extend the number.  In order to ensure
		// that we only get as many bits as we expect, we chop the number.
		if ((flags & PrintAsI1) != 0)
		{
			l &= 0xFF;
		}
		else if ((flags & PrintAsI2) != 0)
		{
			l &= 0xFFFF;
		}

		// Special case the 0.
		int index;
		if (0 == l)
		{
			buffer[0] = '0';
			index = 1;
		}
		else
		{
			index = 0;
			for (var i = 0; i < buffer.Length; i++) // for (...;i<buffer.Length;...) loop instead of do{...}while(l!=0) to help JIT eliminate span bounds checks
			{
				var div = l / (uint)radix; // TODO https://github.com/dotnet/runtime/issues/5213
				var charVal = l - div * (uint)radix;
				l = div;

				buffer[i] = charVal < 10 ? (char)(charVal + '0') : (char)(charVal + 'a' - 10);

				if (l == 0)
				{
					index = i + 1;
					break;
				}
			}
		}

		// If they want the base, append that to the string (in reverse order)
		if (radix != 10 && (flags & PrintBase) != 0)
		{
			switch (radix)
			{
				case 16:
					buffer[index++] = 'x';
					buffer[index++] = '0';
					break;
				case 8:
					buffer[index++] = '0';
					break;
			}
		}

		if (10 == radix)
		{
			// If it was negative, append the sign, else if they requested, add the '+'.
			// If they requested a leading space, put it on.
			if (isNegative)
			{
				buffer[index++] = '-';
			}
			else if ((flags & PrintSign) != 0)
			{
				buffer[index++] = '+';
			}
			else if ((flags & PrefixSpace) != 0)
			{
				buffer[index++] = ' ';
			}
		}

		// Figure out the size of and allocate the resulting string
		Span<char> result = stackalloc char[Math.Max(width, index)];

		// Put the characters into the string in reverse order.
		// Fill the remaining space, if there is any, with the correct padding character.
		var pIndex = 0;
		var padding = result.Length - index;

		if ((flags & LeftAlign) != 0)
		{
			for (var i = 0; i < padding; i++)
			{
				result[pIndex++] = paddingChar;
			}

			for (var i = 0; i < index; i++)
			{
				result[pIndex++] = buffer[index - i - 1];
			}
		}
		else
		{
			for (var i = 0; i < index; i++)
			{
				result[pIndex++] = buffer[index - i - 1];
			}

			for (var i = 0; i < padding; i++)
			{
				result[pIndex++] = paddingChar;
			}
		}

		for (var i = 0; i < result.Length; i++)
		{
			result[i] = Char.ToUpperInvariant(result[i]);
		}

		return new ReadOnlySpan<char>(Unsafe.AsPointer(ref result[0]), result.Length);
	}

	private static void EatWhiteSpace(ReadOnlySpan<char> s, ref int i)
	{
		var localIndex = i;
		for (; localIndex < s.Length && char.IsWhiteSpace(s[localIndex]); localIndex++)
		{
			;
		}

		i = localIndex;
	}

	private static long GrabLongs(int radix, ReadOnlySpan<char> s, ref int i, bool isUnsigned)
	{
		ulong result = 0;
		ulong maxVal;

		// Allow all non-decimal numbers to set the sign bit.
		if (radix == 10 && !isUnsigned)
		{
			maxVal = 0x7FFFFFFFFFFFFFFF / 10;

			// Read all of the digits and convert to a number
			while (i < s.Length && IsDigit(s[i], radix, out var value))
			{
				result = result * (ulong)radix + (ulong)value;
				i++;
			}
		}
		else
		{
			Debug.Assert(radix is 2 or 8 or 10 or 16);
			maxVal =
				radix switch
				{
					10 => 0xffffffffffffffff / 10,
					16 => 0xffffffffffffffff / 16,
					8 => 0xffffffffffffffff / 8,
					_ => 0xffffffffffffffff / 2,
				};

			// Read all of the digits and convert to a number
			while (i < s.Length && IsDigit(s[i], radix, out var value))
			{
				var temp = result * (ulong)radix + (ulong)value;

				result = temp;
				i++;
			}
		}

		return (long)result;
	}

	private static int GrabInts(int radix, ReadOnlySpan<char> s, ref int i, bool isUnsigned)
	{
		uint result = 0;

		// Allow all non-decimal numbers to set the sign bit.
		if (radix == 10 && !isUnsigned)
		{
			// Read all of the digits and convert to a number
			while (i < s.Length && IsDigit(s[i], radix, out var value))
			{
				result = result * (uint)radix + (uint)value;
				i++;
			}
		}
		else
		{
			// Read all of the digits and convert to a number
			while (i < s.Length && IsDigit(s[i], radix, out var value))
			{
				var temp = result * (uint)radix + (uint)value;

				result = temp;
				i++;
			}
		}

		return (int)result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsDigit(char c, int radix, out int result)
	{
		int tmp;
		if ((uint)(c - '0') <= 9)
		{
			result = tmp = c - '0';
		}
		else if ((uint)(c - 'A') <= 'Z' - 'A')
		{
			result = tmp = c - 'A' + 10;
		}
		else if ((uint)(c - 'a') <= 'z' - 'a')
		{
			result = tmp = c - 'a' + 10;
		}
		else
		{
			result = -1;
			return false;
		}

		return tmp < radix;
	}
}