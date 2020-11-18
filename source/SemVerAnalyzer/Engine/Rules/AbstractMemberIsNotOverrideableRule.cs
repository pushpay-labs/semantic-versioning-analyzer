using dnlib.DotNet;
using Pushpay.SemVerAnalyzer.Abstractions;
using Pushpay.SemVerAnalyzer.Assembly;

namespace Pushpay.SemVerAnalyzer.Engine.Rules
{
	internal abstract class AbstractMemberIsNotOverrideableRule<T> : IVersionAnalysisRule<T>
		where T : IMemberDef
	{
		public VersionBumpType Bump => VersionBumpType.Major;

		public bool Applies(T online, T local)
		{
			if (online == null || local == null) {
				return false;
			}

			var oMethod = online.GetUnderlyingMethodInfo();
			var lMethod = local.GetUnderlyingMethodInfo();

			return oMethod.IsAbstract && !(lMethod.IsAbstract || lMethod.IsVirtual);
		}

		public string GetMessage(T info)
		{
			return $"`{info.GetName()}` is no longer overrideable.";
		}
	}

	internal class AbstractMethodIsNotOverrideableRule : AbstractMemberIsNotOverrideableRule<MethodDef> { }

	internal class AbstractPropertyIsNotOverrideableRule : AbstractMemberIsNotOverrideableRule<PropertyDef> { }

	internal class AbstractEventIsNotOverrideableRule : AbstractMemberIsNotOverrideableRule<EventDef> { }
}
