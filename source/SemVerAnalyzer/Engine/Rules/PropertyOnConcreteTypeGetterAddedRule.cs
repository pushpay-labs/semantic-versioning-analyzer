using dnlib.DotNet;

using Pushpay.SemVerAnalyzer.Assembly;
using SemVerAnalyzer.Abstractions;

namespace Pushpay.SemVerAnalyzer.Engine.Rules
{
	internal class PropertyOnConcreteTypeGetterAddedRule : IVersionAnalysisRule<PropertyDef>
	{
		public VersionBumpType Bump => VersionBumpType.Minor;

		public bool Applies(PropertyDef online, PropertyDef local)
		{
			if (online == null || local == null) {
				return false;
			}

			return !online.DeclaringType.IsInterface &&
				   online.GetMethod == null && local.GetMethod != null;
		}

		public string GetMessage(PropertyDef info)
		{
			return $"`{info.GetName()}` now has a getter.";
		}
	}
}
