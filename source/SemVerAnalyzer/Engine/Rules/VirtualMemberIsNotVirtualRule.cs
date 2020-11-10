using dnlib.DotNet;

using Pushpay.SemVerAnalyzer.Assembly;

namespace Pushpay.SemVerAnalyzer.Engine.Rules
{
	abstract class VirtualMemberIsNotVirtualRule<T> : IVersionAnalysisRule<T>
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

			return oMethod.IsVirtual && !lMethod.IsVirtual;
		}

		public string GetMessage(T info)
		{
			return $"`{info.GetName()}` is no longer virtual.";
		}
	}

	class VirtualMethodIsNotVirtualRule : VirtualMemberIsNotVirtualRule<MethodDef> { }

	class VirtualPropertyIsNotVirtualRule : VirtualMemberIsNotVirtualRule<PropertyDef> { }

	class VirtualEventIsNotVirtualRule : VirtualMemberIsNotVirtualRule<EventDef> { }
}
