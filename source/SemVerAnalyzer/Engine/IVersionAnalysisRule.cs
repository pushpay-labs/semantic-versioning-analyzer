namespace Pushpay.SemVerAnalyzer.Engine
{
	interface IVersionAnalysisRule
	{
		VersionBumpType Bump { get; }
	}

	interface IVersionAnalysisRule<in T> : IVersionAnalysisRule
	{
		bool Applies(T online, T local);
		string GetMessage(T info);
	}
}
