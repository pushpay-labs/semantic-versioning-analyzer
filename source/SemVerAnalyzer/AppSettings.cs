using System;
using System.Collections.Generic;

namespace Pushpay.SemVerAnalyzer
{
	public class AppSettings
	{
		public string AdditionalRulesPath { get; set; }
		public Dictionary<string, RuleOverrideType> RuleOverrides { get; set; }

		public RuleOverrideType GetOverrideType(Type ruleType)
		{
			if (!RuleOverrides.TryGetValue(ruleType.Name, out var overrideType)) return RuleOverrideType.NoOverride;

			return overrideType;
		}
	}
}
