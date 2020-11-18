using System.Collections.Generic;
using System.Linq;

using dnlib.DotNet;
using Pushpay.SemVerAnalyzer.Abstractions;
using Pushpay.SemVerAnalyzer.Assembly;

namespace Pushpay.SemVerAnalyzer.Engine
{
	internal class AnalyzeTypeEventsRuleRunner : IVersionRuleRunner
	{
		readonly IEnumerable<IVersionAnalysisRule<EventDef>> _eventRules;

		public AnalyzeTypeEventsRuleRunner(IEnumerable<IVersionAnalysisRule<EventDef>> eventRules)
		{
			_eventRules = eventRules;
		}

		public IEnumerable<Change> Analyze(TypeDef online, TypeDef local)
		{
			if (online == null || local == null) {
				return Enumerable.Empty<Change>();
			}

			var events = online.MatchEvents(local);
			var changes = new List<Change>();
			foreach (var evt in events) {
				changes.AddRange(_eventRules
									 .Where(r => r.Applies(evt.Online, evt.Local))
									 .Select(r => new Change {
										 Bump = r.Bump,
										 Message = r.GetMessage(evt.Online ?? evt.Local)
									 }));
			}

			return changes;
		}
	}
}
