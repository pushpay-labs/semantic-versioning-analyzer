using dnlib.DotNet;

using Pushpay.SemVerAnalyzer.Assembly;

namespace Pushpay.SemVerAnalyzer.Engine.Rules
{
	internal class InaccessiblePropertyGetterIsNowProtectedRule : IVersionAnalysisRule<PropertyDef>
	{
		public VersionBumpType Bump => VersionBumpType.Minor;

		public bool Applies(PropertyDef online, PropertyDef local)
		{
			if (online == null || local == null) {
				return false;
			}

			return online.GetMethod != null && !(online.GetMethod.IsFamily || online.GetMethod.IsPublic) &&
				   local.GetMethod != null && local.GetMethod.IsFamily;
		}

		public string GetMessage(PropertyDef info)
		{
			return $"`{info.GetName()}` getter was inaccessible and is now protected.";
		}
	}
}
