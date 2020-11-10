using dnlib.DotNet;

using Pushpay.SemVerAnalyzer.Assembly;

namespace Pushpay.SemVerAnalyzer.Engine.Rules
{
	class PropertySetterRemovedRule : IVersionAnalysisRule<PropertyDef>
	{
		public VersionBumpType Bump => VersionBumpType.Major;

		public bool Applies(PropertyDef online, PropertyDef local)
		{
			if (online == null || local == null) {
				return false;
			}

			return online.SetMethod != null && local.SetMethod == null;
		}

		public string GetMessage(PropertyDef info)
		{
			return $"`{info.GetName()}` no longer has a setter.";
		}
	}
}
