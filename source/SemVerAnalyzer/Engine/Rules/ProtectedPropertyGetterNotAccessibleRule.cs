using dnlib.DotNet;

using Pushpay.SemVerAnalyzer.Assembly;
using SemVerAnalyzer.Abstractions;

namespace Pushpay.SemVerAnalyzer.Engine.Rules
{
	internal class ProtectedPropertyGetterNotAccessibleRule : IVersionAnalysisRule<PropertyDef>
	{
		public VersionBumpType Bump => VersionBumpType.Major;

		public bool Applies(PropertyDef online, PropertyDef local)
		{
			if (online == null || local == null) {
				return false;
			}

			return online.GetMethod != null && online.GetMethod.IsFamily &&
				   local.GetMethod != null && !(local.GetMethod.IsFamily || local.GetMethod.IsPublic);
		}

		public string GetMessage(PropertyDef info)
		{
			return $"`{info.GetName()}` getter is no longer accessible.";
		}
	}
}
