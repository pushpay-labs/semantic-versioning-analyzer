using dnlib.DotNet;

using Pushpay.SemVerAnalyzer.Assembly;

namespace Pushpay.SemVerAnalyzer.Engine.Rules
{
	class MethodOnConcreteTypeAddedRule : IVersionAnalysisRule<MethodDef>
	{
		public VersionBumpType Bump => VersionBumpType.Minor;

		public bool Applies(MethodDef online, MethodDef local)
		{
			var type = (online ?? local).DeclaringType;

			return !type.IsInterface && online == null && local != null;
		}

		public string GetMessage(MethodDef info)
		{
			return $"`{info.GetName()}` is new or has been made accessible.";
		}
	}
}
