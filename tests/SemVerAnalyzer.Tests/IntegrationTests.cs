using System.Threading.Tasks;

using PowerAssert;

using Xunit;
using Xunit.Abstractions;

namespace Pushpay.SemVerAnalyzer.Tests
{
	public class IntegrationTests : IClassFixture<IntegrationFixture>
	{
		readonly ITestOutputHelper _testOutputHelper;
		readonly CompareCommandRunner _runner;

		public IntegrationTests(IntegrationFixture integrationFixture,
		                        ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
			_runner = integrationFixture.CompareCommandRunner;
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

			var report = await _runner.Compare(command);
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

			var report = await _runner.Compare(command);
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

			var report = await _runner.Compare(command);
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

			var report = await _runner.Compare(command);

			PAssert.IsTrue(() => report == null);
		}

		[Fact]
		public async Task OmitDisclaimer()
		{
			var command = new CompareCommand {
				Assembly = "Local.dll",
				PackageName = "Major",
				OmitDisclaimer = true
			};

			var report = await _runner.Compare(command);

			PAssert.IsTrue(() => !report.Contains("*This is a sanity check"));
		}
	}
}
