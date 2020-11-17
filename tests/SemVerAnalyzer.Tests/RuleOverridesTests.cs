using System.Threading.Tasks;
using PowerAssert;
using Xunit;
using Xunit.Abstractions;

namespace Pushpay.SemVerAnalyzer.Tests
{
	public class RuleOverridesTests :  IClassFixture<IntegrationFixture>
	{
		readonly ITestOutputHelper _testOutputHelper;
		readonly CompareCommandRunner _runner;
		readonly IntegrationFixture _fixture;

		public RuleOverridesTests(IntegrationFixture integrationFixture,
			ITestOutputHelper testOutputHelper)
		{
			_fixture = integrationFixture;
			_testOutputHelper = testOutputHelper;
			_runner = integrationFixture.CompareCommandRunner;
		}

		[Fact]
		public async Task OverrideRemoveTypeToMinor()
		{
			var command = new CompareCommand
			{
				Assembly = "Local.dll",
				PackageName = "Major"
			};
			_fixture.Settings.RuleOverrides["TypeRemovedRule"] = RuleOverrideType.Minor;

			var report = await _runner.Compare(command);
			_testOutputHelper.WriteLine(report);

			using (var poly = PAssert.Poly())
			{
				poly.IsTrue(() => report != null);
				poly.IsTrue(() => report.Contains("- (Minor) `ClassToRemove` is no longer present or accessible."));
			}
		}

		[Fact]
		public async Task OverrideRemoveTypeToPatch()
		{
			var command = new CompareCommand
			{
				Assembly = "Local.dll",
				PackageName = "Major"
			};
			_fixture.Settings.RuleOverrides["TypeRemovedRule"] = RuleOverrideType.Patch;

			var report = await _runner.Compare(command);
			_testOutputHelper.WriteLine(report);

			using (var poly = PAssert.Poly())
			{
				poly.IsTrue(() => report != null);
				poly.IsTrue(() => report.Contains("- (Patch) `ClassToRemove` is no longer present or accessible."));
			}
		}

		[Fact]
		public async Task OverrideRemoveTypeToIgnore()
		{
			var command = new CompareCommand
			{
				Assembly = "Local.dll",
				PackageName = "Major"
			};
			_fixture.Settings.RuleOverrides["TypeRemovedRule"] = RuleOverrideType.Ignore;

			var report = await _runner.Compare(command);
			_testOutputHelper.WriteLine(report);

			using (var poly = PAssert.Poly())
			{
				poly.IsTrue(() => report != null);
				poly.IsTrue(() => !report.Contains("`ClassToRemove` is no longer present or accessible."));
			}
		}
	}
}
