using dnlib.DotNet;

namespace Pushpay.SemVerAnalyzer.Engine.Rules
{
	internal class TypeRemovedRule : IVersionAnalysisRule<TypeDef>
	{
		public VersionBumpType Bump => VersionBumpType.Major;

		public bool Applies(TypeDef online, TypeDef local)
		{
			return online != null && local == null;
		}

		public string GetMessage(TypeDef info)
		{
			return $"`{info.Name}` is no longer present or accessible.";
		}
	}
}
