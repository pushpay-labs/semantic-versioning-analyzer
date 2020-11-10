using dnlib.DotNet;

using Pushpay.SemVerAnalyzer.Assembly;

namespace Pushpay.SemVerAnalyzer.Engine.Rules
{
	class MethodRemovedRule : IVersionAnalysisRule<MethodDef>
	{
		public VersionBumpType Bump => VersionBumpType.Major;

		public bool Applies(MethodDef online, MethodDef local)
		{
			return online != null && local == null;
		}

		public string GetMessage(MethodDef info)
		{
			return $"`{info.GetName()}` is no longer present or accessible.";
		}
	}
}
