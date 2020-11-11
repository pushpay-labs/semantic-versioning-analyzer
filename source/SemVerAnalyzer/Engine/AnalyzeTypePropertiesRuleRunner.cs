using System.Collections.Generic;
using System.Linq;

using dnlib.DotNet;

using Pushpay.SemVerAnalyzer.Assembly;

namespace Pushpay.SemVerAnalyzer.Engine
{
	internal class AnalyzeTypePropertiesRuleRunner : IVersionRuleRunner
	{
		readonly IEnumerable<IVersionAnalysisRule<PropertyDef>> _propertyRules;

		public AnalyzeTypePropertiesRuleRunner(IEnumerable<IVersionAnalysisRule<PropertyDef>> propertyRules)
		{
			_propertyRules = propertyRules;
		}

		public IEnumerable<Change> Analyze(TypeDef online, TypeDef local)
		{
			if (online == null || local == null) {
				return Enumerable.Empty<Change>();
			}

			var properties = online.MatchProperties(local);
			var changes = new List<Change>();
			foreach (var property in properties) {
				changes.AddRange(_propertyRules
									 .Where(r => r.Applies(property.Online, property.Local))
									 .Select(r => new Change {
										 Bump = r.Bump,
										 Message = r.GetMessage(property.Online ?? property.Local)
									 }));
			}

			return changes;
		}
	}
}
