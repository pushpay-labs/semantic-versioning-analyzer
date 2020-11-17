using dnlib.DotNet;

using Pushpay.SemVerAnalyzer.Assembly;
using SemVerAnalyzer.Abstractions;

namespace Pushpay.SemVerAnalyzer.Engine.Rules
{
	internal abstract class VirtualMemberIsNotVirtualRule<T> : IVersionAnalysisRule<T>
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

	internal class VirtualMethodIsNotVirtualRule : VirtualMemberIsNotVirtualRule<MethodDef> { }

	internal class VirtualPropertyIsNotVirtualRule : VirtualMemberIsNotVirtualRule<PropertyDef> { }

	internal class VirtualEventIsNotVirtualRule : VirtualMemberIsNotVirtualRule<EventDef> { }
}
