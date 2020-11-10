using dnlib.DotNet;

using Pushpay.SemVerAnalyzer.Assembly;

namespace Pushpay.SemVerAnalyzer.Engine.Rules
{
	abstract class VirtualMemberHasBeenSealedRule<T> : IVersionAnalysisRule<T>
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

			return oMethod.IsVirtual && !oMethod.IsFinal &&
			       lMethod.IsVirtual && lMethod.IsFinal;
		}

		public string GetMessage(T info)
		{
			return $"`{info.GetName()}` has been sealed.";
		}
	}

	class VirtualMethodHasBeenSealedRule : VirtualMemberHasBeenSealedRule<MethodDef> { }

	class VirtualPropertyHasBeenSealedRule : VirtualMemberHasBeenSealedRule<PropertyDef> { }

	class VirtualEventHasBeenSealedRule : VirtualMemberHasBeenSealedRule<EventDef> { }
}
