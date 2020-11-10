using dnlib.DotNet;

using Pushpay.SemVerAnalyzer.Assembly;

namespace Pushpay.SemVerAnalyzer.Engine.Rules
{
	class PublicPropertySetterNotPublicRule : IVersionAnalysisRule<PropertyDef>
	{
		public VersionBumpType Bump => VersionBumpType.Major;

		public bool Applies(PropertyDef online, PropertyDef local)
		{
			if (online == null || local == null) {
				return false;
			}

			return online.SetMethod != null && online.SetMethod.IsPublic &&
			       local.SetMethod != null && !local.SetMethod.IsPublic;
		}

		public string GetMessage(PropertyDef info)
		{
			return $"`{info.GetName()}` setter is no longer public.";
		}
	}
}
