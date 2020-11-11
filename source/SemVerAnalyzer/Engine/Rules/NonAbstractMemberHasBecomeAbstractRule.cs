using dnlib.DotNet;

using Pushpay.SemVerAnalyzer.Assembly;

namespace Pushpay.SemVerAnalyzer.Engine.Rules
{
	internal abstract class NonAbstractMemberHasBecomeAbstractRule<T> : IVersionAnalysisRule<T>
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

			return !oMethod.IsAbstract && lMethod.IsAbstract;
		}

		public string GetMessage(T info)
		{
			return $"`{info.GetName()}` has become abstract.";
		}
	}

	internal class NonAbstractMethodHasBecomeAbstractRule : NonAbstractMemberHasBecomeAbstractRule<MethodDef> { }

	internal class NonAbstractPropertyHasBecomeAbstractRule : NonAbstractMemberHasBecomeAbstractRule<PropertyDef> { }

	internal class NonAbstractEventHasBecomeAbstractRule : NonAbstractMemberHasBecomeAbstractRule<EventDef> { }
}
