using NodaTime;
#pragma warning disable 414 // Field assigned but never used
#pragma warning disable IDE0052 // Remove unread private members
// ReSharper disable UnusedType.Global

namespace TestData
{
	public class ClassToRemove
	{
		Instant _now = default;
	}
}
