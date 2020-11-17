using dnlib.DotNet;

using Pushpay.SemVerAnalyzer.Assembly;
using SemVerAnalyzer.Abstractions;

namespace Pushpay.SemVerAnalyzer.Engine.Rules
{
	internal class ProtectedPropertySetterNotAccessibleRule : IVersionAnalysisRule<PropertyDef>
	{
		public VersionBumpType Bump => VersionBumpType.Major;

		public bool Applies(PropertyDef online, PropertyDef local)
		{
			if (online == null || local == null) {
				return false;
			}

			return online.SetMethod != null && online.SetMethod.IsFamily &&
				   local.SetMethod != null && !(local.SetMethod.IsFamily || local.SetMethod.IsPublic);
		}

		public string GetMessage(PropertyDef info)
		{
			return $"`{info.GetName()}` setter is no longer accessible.";
		}
	}
}
