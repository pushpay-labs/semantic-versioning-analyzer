using System.Collections.Generic;
using System.IO;
using System.Linq;
using dnlib.DotNet;
using PowerAssert;
using Pushpay.SemVerAnalyzer.Engine;
using Xunit;

namespace Pushpay.SemVerAnalyzer.Tests
{
	public abstract class StandardRuleCheckBase : IClassFixture<IntegrationFixture>
	{
		readonly IntegrationFixture _integrationFixture;

		protected StandardRuleCheckBase(IntegrationFixture integrationFixture)
		{
			_integrationFixture = integrationFixture;
		}

		protected void PerformFieldCheck<TRef, TLocal>(params string[] expectedMessages)
		{
			var data = File.ReadAllBytes(typeof(TRef).Assembly.Location);
			var modCtx = ModuleDef.CreateModuleContext();
			var module = ModuleDefMD.Load(data, modCtx);

			var refDef = module.GetTypes().Single(td => td.AssemblyQualifiedName == typeof(TRef).AssemblyQualifiedName);
			var localDef = module.GetTypes().Single(td => td.AssemblyQualifiedName == typeof(TLocal).AssemblyQualifiedName);

			var actualMessages = new List<string>();

			foreach (IVersionRuleRunner runner in _integrationFixture.RuleRunners)
			{
				actualMessages.AddRange(runner.Analyze(refDef, localDef).Select(c => c.Message));
			}

			AssertContentEqual(expectedMessages, actualMessages);
		}

		static void AssertContentEqual(IEnumerable<string> expected, IEnumerable<string> actual)
		{
			using (var poly = PAssert.Poly())
			{
				poly.IsTrue(() => expected.Count() == actual.Count());
				poly.IsTrue(() => !expected.Except(actual).Any());
				poly.IsTrue(() => !actual.Except(expected).Any());
			}
		}
	}
}
