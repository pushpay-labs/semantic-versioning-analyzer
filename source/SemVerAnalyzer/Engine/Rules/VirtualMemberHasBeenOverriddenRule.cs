using dnlib.DotNet;
using Pushpay.SemVerAnalyzer.Abstractions;
using Pushpay.SemVerAnalyzer.Assembly;

namespace Pushpay.SemVerAnalyzer.Engine.Rules
{
	internal abstract class VirtualMemberHasBeenOverriddenRule<T> : IVersionAnalysisRule<T>
		where T : IMemberDef
	{
		public VersionBumpType Bump => VersionBumpType.Patch;

		public bool Applies(T online, T local)
		{
			if (local == null) return false;
			if (online != null) return false;

			var lMethod = local.GetUnderlyingMethodDef();

			return lMethod.IsOverride();
		}

		public string GetMessage(T info)
		{
			return $"`{info.GetName()}` is a new override.";
		}
	}

	internal class VirtualMethodHasBeenOverriddenRule : VirtualMemberHasBeenOverriddenRule<MethodDef> { }

	internal class VirtualPropertyHasBeenOverriddenRule : VirtualMemberHasBeenOverriddenRule<PropertyDef> { }

	internal class VirtualEventHasBeenOverriddenRule : VirtualMemberHasBeenOverriddenRule<EventDef> { }
}
