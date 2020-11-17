using SemVerAnalyzer.Abstractions;

namespace Pushpay.SemVerAnalyzer.Engine
{
	public class Change
	{
		public VersionBumpType Bump { get; set; }
		public string Message { get; set; }
	}
}
