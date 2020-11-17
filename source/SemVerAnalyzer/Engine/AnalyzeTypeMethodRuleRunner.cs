using System.Collections.Generic;
using System.Linq;

using dnlib.DotNet;

using Pushpay.SemVerAnalyzer.Assembly;
using SemVerAnalyzer.Abstractions;

namespace Pushpay.SemVerAnalyzer.Engine
{
	internal class AnalyzeTypeMethodRuleRunner : IVersionRuleRunner
	{
		readonly IEnumerable<IVersionAnalysisRule<MethodDef>> _methodRules;

		public AnalyzeTypeMethodRuleRunner(IEnumerable<IVersionAnalysisRule<MethodDef>> methodRules)
		{
			_methodRules = methodRules;
		}

		public IEnumerable<Change> Analyze(TypeDef online, TypeDef local)
		{
			if (online == null || local == null) {
				return Enumerable.Empty<Change>();
			}

			var methods = online.MatchMethods(local);
			var changes = new List<Change>();
			foreach (var method in methods) {
				changes.AddRange(_methodRules
									 .Where(r => r.Applies(method.Online, method.Local))
									 .Select(r => new Change {
										 Bump = r.Bump,
										 Message = r.GetMessage(method.Online ?? method.Local)
									 }));
			}

			return changes;
		}
	}
}
