using System.Threading.Tasks;
using Autofac;
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
			integrationFixture.Settings.AdditionalRulesPath = "AuxiliaryRules.dll";
			integrationFixture.Builder.RegisterModule(new ExternalRuleModule(integrationFixture.Settings));

			_testOutputHelper = testOutputHelper;
			_runner = integrationFixture.CompareCommandRunner;
		}

		[Fact]
		public async Task CompareMajorChange()
		{
			var command = new CompareCommand
			{
				Assembly = "Local.dll",
				PackageName = "Minor"
			};

			var report = await _runner.Compare(command);
			_testOutputHelper.WriteLine(report);

			using (var poly = PAssert.Poly())
			{
				poly.IsTrue(() => report != null);
				poly.IsTrue(() => report.Contains("always bump major"));
			}
		}
	}
}
