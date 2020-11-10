using Pushpay.SemVerAnalyzer.Assembly;

namespace Pushpay.SemVerAnalyzer.Engine.Rules
{
	class ReferencesMajorBumpedRule : IVersionAnalysisRule<AssemblyReference>
	{
		public VersionBumpType Bump => VersionBumpType.Major;

		public bool Applies(AssemblyReference online, AssemblyReference local)
		{
			return online.Version.MajorVersion() < local.Version.MajorVersion();
		}

		public string GetMessage(AssemblyReference info)
		{
			return $"The updated reference to {info.Name}@{info.Version} represents a major bump.";
		}
	}
}
