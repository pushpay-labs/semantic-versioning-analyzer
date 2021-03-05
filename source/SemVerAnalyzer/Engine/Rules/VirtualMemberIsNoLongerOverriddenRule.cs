using dnlib.DotNet;
using Pushpay.SemVerAnalyzer.Abstractions;
using Pushpay.SemVerAnalyzer.Assembly;

namespace Pushpay.SemVerAnalyzer.Engine.Rules
{
	internal abstract class VirtualMemberIsNoLongerOverriddenRule<T> : IVersionAnalysisRule<T>
		where T : IMemberDef
	{
		public VersionBumpType Bump => VersionBumpType.Patch;

		public bool Applies(T online, T local)
		{
			if (local != null) return false;
			if (online == null) return false;

			var oMethod = online.GetUnderlyingMethodDef();

			return oMethod.IsOverride();
		}

		public string GetMessage(T info)
		{
			return $"`{info.GetName()}` is no longer overridden.";
		}
	}

	internal class VirtualMethodIsNoLongerOverriddenRule : VirtualMemberIsNoLongerOverriddenRule<MethodDef> { }

	internal class VirtualPropertyIsNoLongerOverriddenRule : VirtualMemberIsNoLongerOverriddenRule<PropertyDef> { }

	internal class VirtualEventIsNoLongerOverriddenRule : VirtualMemberIsNoLongerOverriddenRule<EventDef> { }
}
