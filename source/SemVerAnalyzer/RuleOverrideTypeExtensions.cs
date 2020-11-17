using System;
using Pushpay.SemVerAnalyzer.Engine;
using SemVerAnalyzer.Abstractions;

namespace Pushpay.SemVerAnalyzer
{
	public static class RuleOverrideTypeExtensions
	{
		public static VersionBumpType ToBumpType(this RuleOverrideType overrideType, VersionBumpType bump)
		{
			return overrideType switch
			{
				RuleOverrideType.NoOverride => bump,
				RuleOverrideType.Ignore => VersionBumpType.None,
				RuleOverrideType.Major => VersionBumpType.Major,
				RuleOverrideType.Minor => VersionBumpType.Minor,
				RuleOverrideType.Patch => VersionBumpType.Patch,
				_ => throw new ArgumentOutOfRangeException(nameof(overrideType), overrideType, null)
			};
		}
	}
}
