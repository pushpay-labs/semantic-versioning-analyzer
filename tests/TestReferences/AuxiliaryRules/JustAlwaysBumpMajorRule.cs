using dnlib.DotNet;
using Pushpay.SemVerAnalyzer.Engine;

namespace AuxiliaryRules
{
	public class JustAlwaysBumpMajorRule : IVersionAnalysisRule<TypeDef>
	{
		public VersionBumpType Bump => VersionBumpType.Major;

		public bool Applies(TypeDef online, TypeDef local) => true;

		public string GetMessage(TypeDef info) => "always bump major";
	}
}
