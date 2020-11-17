using PowerAssert;
using Pushpay.SemVerAnalyzer.Engine;
using SemVerAnalyzer.Abstractions;
using Xunit;

namespace Pushpay.SemVerAnalyzer.Tests
{
	public class SemVerStringTests
	{
		[Fact]
		public void ExtractMajor()
		{
			PAssert.IsTrue(() => "2.3.4".MajorVersion() == 2);
		}

		[Fact]
		public void ExtractMinor()
		{
			PAssert.IsTrue(() => "2.3.4".MinorVersion() == 3);
		}

		[Fact]
		public void ExtractPatch()
		{
			PAssert.IsTrue(() => "2.3.4".PatchVersion() == 4);
		}

		[Fact]
		public void SuggestMajor()
		{
			PAssert.IsTrue(() => "2.3.4".GetSuggestedVersion(VersionBumpType.Major) == "3.0.0");
		}

		[Fact]
		public void SuggestMinor()
		{
			PAssert.IsTrue(() => "2.3.4".GetSuggestedVersion(VersionBumpType.Minor) == "2.4.0");
		}

		[Fact]
		public void SuggestPatch()
		{
			PAssert.IsTrue(() => "2.3.4".GetSuggestedVersion(VersionBumpType.Patch) == "2.3.5");
		}

		[Fact]
		public void SuggestNone()
		{
			PAssert.IsTrue(() => "2.3.4".GetSuggestedVersion(VersionBumpType.None) == "2.3.4");
		}
	}
}
