using dnlib.DotNet;
using Pushpay.SemVerAnalyzer.Abstractions;

namespace Pushpay.SemVerAnalyzer.Engine.Rules
{
	internal class EventRemovedRule : IVersionAnalysisRule<EventDef>
	{
		public VersionBumpType Bump => VersionBumpType.Major;

		public bool Applies(EventDef online, EventDef local)
		{
			return online != null && local == null;
		}

		public string GetMessage(EventDef info)
		{
			return $"`{info.GetName()}` is no longer present or accessible.";
		}
	}
}
