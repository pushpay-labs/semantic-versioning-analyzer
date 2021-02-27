using System.Threading.Tasks;

using PowerAssert;

using Xunit;
using Xunit.Abstractions;

namespace Pushpay.SemVerAnalyzer.Tests
{
	public class IntegrationTests : IClassFixture<IntegrationFixture>
	{
		readonly IntegrationFixture _integrationFixture;
		readonly ITestOutputHelper _testOutputHelper;

		CompareCommandRunner Runner => _integrationFixture.CompareCommandRunner;

		public IntegrationTests(IntegrationFixture integrationFixture,
		                        ITestOutputHelper testOutputHelper)
		{
			_integrationFixture = integrationFixture;
			_testOutputHelper = testOutputHelper;
		}

		static readonly string[] _majorChanges = {
			"- (Major) `ClassToRemove` is no longer present or accessible.",
			"- (Major) `StructToRemove` is no longer present or accessible.",
			"- (Major) The updated reference to NodaTime@2.1.1 represents a major bump.",
		};

		[Fact]
		public async Task CompareMajorChange()
		{
			var command = new CompareCommand {
				Assembly = "Local.dll",
				PackageName = "Major"
			};

			var report = await Runner.Compare(command);
			_testOutputHelper.WriteLine(report);

			using (var poly = PAssert.Poly()) {
				poly.IsTrue(() => report != null);
				foreach (var change in _majorChanges) {
					poly.IsTrue(() => report.Contains(change));
				}
			}
		}

		static readonly string[] _minorChanges = {
			"- (Minor) `ClassToAdd` is new or has been made accessible.",
			"- (Minor) `StructToAdd` is new or has been made accessible.",
			"- (Minor) The updated reference to NodaTime@2.1.1 represents a minor bump.",
		};

		[Fact]
		public async Task CompareMinorChange()
		{
			var command = new CompareCommand {
				Assembly = "Local.dll",
				PackageName = "Minor"
			};

			var report = await Runner.Compare(command);
			_testOutputHelper.WriteLine(report);

			using (var poly = PAssert.Poly()) {
				poly.IsTrue(() => report != null);
				foreach (var change in _minorChanges) {
					poly.IsTrue(() => report.Contains(change));
				}
			}
		}

		static readonly string[] _patchChanges = {
			"- (Patch) The updated reference to NodaTime@2.1.1 represents a patch bump.",
		};

		[Fact]
		public async Task ComparePatchChange()
		{
			var command = new CompareCommand {
				Assembly = "Local.dll",
				PackageName = "Patch"
			};

			var report = await Runner.Compare(command);
			_testOutputHelper.WriteLine(report);

			using (var poly = PAssert.Poly()) {
				poly.IsTrue(() => report != null);
				foreach (var change in _patchChanges) {
					poly.IsTrue(() => report.Contains(change));
				}
			}
		}

		[Fact]
		public async Task CompareSameAssembly()
		{
			var command = new CompareCommand {
				Assembly = "Local.dll",
				PackageName = "Local"
			};

			var report = await Runner.Compare(command);

			PAssert.IsTrue(() => report == null);
		}

		[Fact]
		public async Task OmitDisclaimerTrue()
		{
			var command = new CompareCommand {
				Assembly = "Local.dll",
				PackageName = "Major",
				OmitDisclaimer = true
			};

			_integrationFixture.ApplyOverrides(command);

			var report = await Runner.Compare(command);

			PAssert.IsTrue(() => !report.Contains("*This is a sanity check"));
		}

		[Fact]
		public async Task OmitDisclaimerFalse()
		{
			var command = new CompareCommand {
				Assembly = "Local.dll",
				PackageName = "Major"
			};

			_integrationFixture.ApplyOverrides(command);

			var report = await Runner.Compare(command);

			PAssert.IsTrue(() => report.Contains("*This is a sanity check"));
		}

		[Fact]
		public async Task IncludeHeaderTrue()
		{
			var command = new CompareCommand {
				Assembly = "Local.dll",
				PackageName = "Major",
				IncludeHeader = true
			};

			_integrationFixture.ApplyOverrides(command);

			var report = await Runner.Compare(command);

			PAssert.IsTrue(() => report.Contains("# Local ( Major )"));
		}

		[Fact]
		public async Task IncludeHeaderFalse()
		{
			var command = new CompareCommand {
				Assembly = "Local.dll",
				PackageName = "Major"
			};

			_integrationFixture.ApplyOverrides(command);

			var report = await Runner.Compare(command);

			PAssert.IsTrue(() => !report.Contains("# Local ( Major )"));
		}
	}
}
