namespace Pushpay.SemVerAnalyzer.Engine
{
	internal interface IVersionAnalysisRule
	{
		VersionBumpType Bump { get; }
	}

	internal interface IVersionAnalysisRule<in T> : IVersionAnalysisRule
	{
		bool Applies(T online, T local);
		string GetMessage(T info);
	}
}
