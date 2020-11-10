using System.Collections.Generic;
using System.Linq;

namespace Pushpay.SemVerAnalyzer.Engine
{
	public class VersionAnalysisResult
	{
		public VersionBumpType ActualBump { get; set; }

		public VersionBumpType CalculatedBump { get; set; }

		public List<string> Comments { get; } = new List<string>();

		public List<Change> Changes { get; } = new List<Change>();

		public List<string> GetAllComments() => Comments.Concat(Changes.OrderBy(c => c.Message).Select(c => $"({c.Bump}) {c.Message}")).ToList();
	}
}
