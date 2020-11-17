using dnlib.DotNet;

using Pushpay.SemVerAnalyzer.Assembly;
using SemVerAnalyzer.Abstractions;

namespace Pushpay.SemVerAnalyzer.Engine.Rules
{
	internal class PropertyOnInterfaceSetterAddedRule : IVersionAnalysisRule<PropertyDef>
	{
		public VersionBumpType Bump => VersionBumpType.Major;

		public bool Applies(PropertyDef online, PropertyDef local)
		{
			if (online == null || local == null) {
				return false;
			}

			return online.DeclaringType.IsInterface &&
				   online.SetMethod == null && local.SetMethod != null;
		}

		public string GetMessage(PropertyDef info)
		{
			return $"`{info.GetName()}` now has a setter.";
		}
	}
}
