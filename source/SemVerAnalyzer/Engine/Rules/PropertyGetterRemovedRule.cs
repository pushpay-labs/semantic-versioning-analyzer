using dnlib.DotNet;
using Pushpay.SemVerAnalyzer.Abstractions;

namespace Pushpay.SemVerAnalyzer.Engine.Rules
{
	internal class PropertyGetterRemovedRule : IVersionAnalysisRule<PropertyDef>
	{
		public VersionBumpType Bump => VersionBumpType.Major;

		public bool Applies(PropertyDef online, PropertyDef local)
		{
			if (online == null || local == null) {
				return false;
			}

			return online.GetMethod != null && local.GetMethod == null;
		}

		public string GetMessage(PropertyDef info)
		{
			return $"`{info.GetName()}` no longer has a getter.";
		}
	}
}
