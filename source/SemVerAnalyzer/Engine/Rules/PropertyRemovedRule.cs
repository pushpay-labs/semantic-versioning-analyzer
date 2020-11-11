using dnlib.DotNet;

using Pushpay.SemVerAnalyzer.Assembly;

namespace Pushpay.SemVerAnalyzer.Engine.Rules
{
	internal class PropertyRemovedRule : IVersionAnalysisRule<PropertyDef>
	{
		public VersionBumpType Bump => VersionBumpType.Major;

		public bool Applies(PropertyDef online, PropertyDef local)
		{
			return online != null && local == null;
		}

		public string GetMessage(PropertyDef info)
		{
			return $"`{info.GetName()}` is no longer present or accessible.";
		}
	}
}
