using dnlib.DotNet;

using Pushpay.SemVerAnalyzer.Assembly;
using SemVerAnalyzer.Abstractions;

namespace Pushpay.SemVerAnalyzer.Engine.Rules
{
	internal class MethodOnInterfaceAddedRule : IVersionAnalysisRule<MethodDef>
	{
		public VersionBumpType Bump => VersionBumpType.Major;

		public bool Applies(MethodDef online, MethodDef local)
		{
			var type = (online ?? local).DeclaringType;

			return type.IsInterface && online == null && local != null;
		}

		public string GetMessage(MethodDef info)
		{
			return $"`{info.GetName()}` is new or has been made accessible.";
		}
	}
}
