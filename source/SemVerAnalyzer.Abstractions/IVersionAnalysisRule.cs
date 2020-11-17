namespace SemVerAnalyzer.Abstractions
{
	public interface IVersionAnalysisRule
	{
		VersionBumpType Bump { get; }
	}

	public interface IVersionAnalysisRule<in T> : IVersionAnalysisRule
	{
		bool Applies(T online, T local);
		string GetMessage(T info);
	}
}
