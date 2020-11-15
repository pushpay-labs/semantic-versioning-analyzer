using System;
using System.IO;
using System.Linq;
using System.Text;
using PowerAssert;
using Xunit;

namespace Pushpay.SemVerAnalyzer.Tests
{
	public class DocumentationTests
	{
		[Fact]
		public void ReadmeListsAllBuiltInRules()
		{
			var readmeLines = File.ReadAllLines("README.md");
			var ruleInterfaceType = typeof(AppModule).Assembly
				.GetTypes()
				.FirstOrDefault(t => t.Name == "IVersionAnalysisRule" && !t.IsGenericType);
			var ruleTypes = typeof(AppModule).Assembly
				.GetTypes()
				.Where(t => !t.IsAbstract && !t.IsInterface && ruleInterfaceType.IsAssignableFrom(t));

			var sb = new StringBuilder();

			foreach (Type type in ruleTypes)
			{
				if (!readmeLines.Contains($"  - `{type.Name}`"))
				{
					sb.AppendLine(type.Name);
				}
			}

			PAssert.IsTrue(() => sb.ToString() == string.Empty);
		}
	}
}
