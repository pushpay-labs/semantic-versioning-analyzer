using System;
using System.Linq;
using dnlib.DotNet;
using Pushpay.SemVerAnalyzer.Assembly;
using SemVerAnalyzer.Abstractions;

namespace Pushpay.SemVerAnalyzer.Engine.Rules
{
	public class TypeMarkedObsoleteRule : IVersionAnalysisRule<TypeDef>
	{
		public VersionBumpType Bump => VersionBumpType.Minor;

		public bool Applies(TypeDef online, TypeDef local)
		{
			if (online == null || local == null) return false;

			var localIsObsolete = local.CustomAttributes.Any(a => a.AttributeType.FullName == typeof(ObsoleteAttribute).FullName);
			var onlineIsObsolete = online.CustomAttributes.Any(a => a.AttributeType.FullName == typeof(ObsoleteAttribute).FullName);

			return localIsObsolete && !onlineIsObsolete;
		}

		public string GetMessage(TypeDef info)
		{
			return $"`{info.GetName()}` has been marked obsolete.";
		}
	}
}
