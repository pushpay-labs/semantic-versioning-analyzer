using dnlib.DotNet;

using Pushpay.SemVerAnalyzer.Assembly;

namespace Pushpay.SemVerAnalyzer.Engine.Rules
{
	internal class PropertyOnConcreteTypeAddedRule : IVersionAnalysisRule<PropertyDef>
	{
		public VersionBumpType Bump => VersionBumpType.Minor;

		public bool Applies(PropertyDef online, PropertyDef local)
		{
			var type = (online ?? local).DeclaringType;

			return !type.IsInterface && online == null && local != null;
		}

		public string GetMessage(PropertyDef info)
		{
			return $"`{info.GetName()}` is new or has been made accessible.";
		}
	}
}
