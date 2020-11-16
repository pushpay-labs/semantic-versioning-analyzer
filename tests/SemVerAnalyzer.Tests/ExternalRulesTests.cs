using System.Threading.Tasks;
using PowerAssert;
using Xunit;
using Xunit.Abstractions;

namespace Pushpay.SemVerAnalyzer.Tests
{
	public class ExternalRulesTests :  IClassFixture<IntegrationFixture>
	{
		readonly ITestOutputHelper _testOutputHelper;
		readonly CompareCommandRunner _runner;

		public ExternalRulesTests(IntegrationFixture integrationFixture,
			ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
			_runner = integrationFixture.CompareCommandRunner;
		}

		[Fact]
		public async Task CompareMajorChange()
		{
			var command = new CompareCommand
			{
				Assembly = "Local.dll",
				PackageName = "Major"
			};

			var report = await _runner.Compare(command);
			_testOutputHelper.WriteLine(report);

			using (var poly = PAssert.Poly())
			{
				poly.IsTrue(() => report != null);
				poly.IsTrue(() => report.Contains("internal type removed"));
			}
		}
	}
}
