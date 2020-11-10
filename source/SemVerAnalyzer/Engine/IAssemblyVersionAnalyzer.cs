using Pushpay.SemVerAnalyzer.Assembly;

namespace Pushpay.SemVerAnalyzer.Engine
{
	public interface IAssemblyVersionAnalyzer
	{
		VersionAnalysisResult AnalyzeVersions(AssemblyPublicInterface localPublicInterface,
		                                      AssemblyPublicInterface onlinePublicInterface);
	}
}
