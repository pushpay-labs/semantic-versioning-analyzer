using System;
using System.Collections.Generic;

namespace Pushpay.SemVerAnalyzer
{
	public class AppSettings
	{
		Dictionary<string, RuleOverrideType> _ruleOverrides;

		public Dictionary<string, RuleOverrideType> RuleOverrides
		{
			get => _ruleOverrides ??= new Dictionary<string, RuleOverrideType>();
			set => _ruleOverrides = value;
		}

		public string AdditionalRulesPath { get; set; }
		public bool OmitDisclaimer { get; set; }
		public bool IncludeHeader { get; set; }
		public bool AssumeChanges { get; set; }

		public RuleOverrideType GetOverrideType(Type ruleType)
		{
			if (!RuleOverrides.TryGetValue(ruleType.Name, out var overrideType)) return RuleOverrideType.NoOverride;

			return overrideType;
		}
	}
}
