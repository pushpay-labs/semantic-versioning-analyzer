using dnlib.DotNet;
using Pushpay.SemVerAnalyzer.Abstractions;
using Pushpay.SemVerAnalyzer.Assembly;

namespace Pushpay.SemVerAnalyzer.Engine.Rules
{
	internal abstract class VirtualMemberHasBeenSealedRule<T> : IVersionAnalysisRule<T>
		where T : IMemberDef
	{
		public VersionBumpType Bump => VersionBumpType.Major;

		public bool Applies(T online, T local)
		{
			if (online == null || local == null) {
				return false;
			}

			var oMethod = online.GetUnderlyingMethodDef();
			var lMethod = local.GetUnderlyingMethodDef();

			return oMethod.IsVirtual && !oMethod.IsFinal &&
				   lMethod.IsVirtual && lMethod.IsFinal;
		}

		public string GetMessage(T info)
		{
			return $"`{info.GetName()}` has been sealed.";
		}
	}

	internal class VirtualMethodHasBeenSealedRule : VirtualMemberHasBeenSealedRule<MethodDef> { }

	internal class VirtualPropertyHasBeenSealedRule : VirtualMemberHasBeenSealedRule<PropertyDef> { }

	internal class VirtualEventHasBeenSealedRule : VirtualMemberHasBeenSealedRule<EventDef> { }
}
