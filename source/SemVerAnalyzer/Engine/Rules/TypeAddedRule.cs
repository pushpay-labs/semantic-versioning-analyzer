using dnlib.DotNet;

namespace Pushpay.SemVerAnalyzer.Engine.Rules
{
	internal class TypeAddedRule : IVersionAnalysisRule<TypeDef>
	{
		public VersionBumpType Bump => VersionBumpType.Minor;

		public bool Applies(TypeDef online, TypeDef local)
		{
			return online == null && local != null;
		}

		public string GetMessage(TypeDef info)
		{
			return $"`{info.Name}` is new or has been made accessible.";
		}
	}
}
