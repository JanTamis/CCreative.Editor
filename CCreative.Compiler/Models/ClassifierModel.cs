using CCreative.Compilers.Enums;

namespace CCreative.Compilers.Models;

public readonly struct ClassifierModel
{
	/// <summary>
	/// Start point of the span.
	/// </summary>
	public int Start { get; init; }

	/// <summary>
	/// End of the span.
	/// </summary>
	public int End => Start + Length;

	/// <summary>
	/// Length of the span.
	/// </summary>
	public int Length { get; init; }

	public ClassifierType Type { get; init; }

	/// <summary>
	/// Determines whether or not the span is empty.
	/// </summary>
	public bool IsEmpty => Length == 0;

	public ClassifierModel(int start, int length, ClassifierType type)
	{
		Start = start;
		Length = length;
		Type = type;
	}

	/// <summary>
	/// Determines whether the position lies within the span.
	/// </summary>
	/// <param name="position">
	/// The position to check.
	/// </param>
	/// <returns>
	/// <c>true</c> if the position is greater than or equal to Start and strictly less 
	/// than End, otherwise <c>false</c>.
	/// </returns>
	public bool Contains(int position)
	{
		return unchecked((uint)(position - Start) < (uint)Length);
	}
}