using Pushpay.SemVerAnalyzer.Abstractions;
using Pushpay.SemVerAnalyzer.Assembly;

namespace Pushpay.SemVerAnalyzer.Engine.Rules
{
	internal class ReferencesPatchBumpedRule : IVersionAnalysisRule<AssemblyReference>
	{
		public VersionBumpType Bump => VersionBumpType.Patch;

		public bool Applies(AssemblyReference online, AssemblyReference local)
		{
			if (online == null || local == null) return false;

			return online.Version.PatchVersion() < local.Version.PatchVersion();
		}

		public string GetMessage(AssemblyReference info)
		{
			return $"The updated reference to {info.Name}@{info.Version} represents a patch bump.";
		}
	}
}
