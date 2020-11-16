using dnlib.DotNet;
using Pushpay.SemVerAnalyzer.Engine;

namespace AuxiliaryRules
{
	public class InternalTypeRemoved : IVersionAnalysisRule<TypeDef>
	{
		public VersionBumpType Bump => VersionBumpType.Major;

		public bool Applies(TypeDef online, TypeDef local) => online != null && local == null && !online.IsPublic;

		public string GetMessage(TypeDef info) => "internal type removed";
	}
}
