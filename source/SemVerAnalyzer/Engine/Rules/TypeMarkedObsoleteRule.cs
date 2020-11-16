using System;
using System.Linq;
using dnlib.DotNet;

namespace Pushpay.SemVerAnalyzer.Engine.Rules
{
	public class TypeMarkedObsoleteRule : IVersionAnalysisRule<TypeDef>
	{
		public VersionBumpType Bump => VersionBumpType.Minor;

		public bool Applies(TypeDef online, TypeDef local)
		{
			var localIsObsolete = local.CustomAttributes.Any(a => a.AttributeType.FullName == typeof(ObsoleteAttribute).FullName);
			var onlineIsObsolete = online.CustomAttributes.Any(a => a.AttributeType.FullName == typeof(ObsoleteAttribute).FullName);

			return localIsObsolete && !onlineIsObsolete;
		}

		public string GetMessage(TypeDef info)
		{
			return $"`{info.Name}` has been marked obsolete.";
		}
	}
}
