using dnlib.DotNet;
using Pushpay.SemVerAnalyzer.Abstractions;

namespace Pushpay.SemVerAnalyzer.Engine.Rules
{
	internal class PropertyOnInterfaceAddedRule : IVersionAnalysisRule<PropertyDef>
	{
		public VersionBumpType Bump => VersionBumpType.Major;

		public bool Applies(PropertyDef online, PropertyDef local)
		{
			var type = (online ?? local).DeclaringType;

			return type.IsInterface && online == null && local != null;
		}

		public string GetMessage(PropertyDef info)
		{
			return $"`{info.GetName()}` is new or has been made accessible.";
		}
	}
}
