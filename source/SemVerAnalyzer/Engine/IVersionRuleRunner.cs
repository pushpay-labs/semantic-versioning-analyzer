using System.Collections.Generic;

using dnlib.DotNet;

namespace Pushpay.SemVerAnalyzer.Engine
{
	interface IVersionRuleRunner
	{
		IEnumerable<Change> Analyze(TypeDef online, TypeDef local);
	}
}
