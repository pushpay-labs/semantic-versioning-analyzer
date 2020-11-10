using System.Linq;

using dnlib.DotNet;

namespace Pushpay.SemVerAnalyzer.Engine.Rules
{
	class EnumMemberValueChangedRule : IVersionAnalysisRule<TypeDef>
	{
		public VersionBumpType Bump => VersionBumpType.Major;

		public bool Applies(TypeDef online, TypeDef local)
		{
			if (online == null || local == null) {
				return false;
			}

			if (!online.IsEnum) {
				return false;
			}

			var onlineValues = online.Fields
				.Where(f => f.Constant != null)
				.Select(f => new {f.Name, Value = (int)f.Constant.Value})
				.ToList();
			var localValues = local.Fields
				.Where(f => f.Constant != null)
				.Select(f => new {f.Name, Value = (int)f.Constant.Value})
				.ToList();

			var enumerable = onlineValues.Join(localValues,
			                                   o => o.Name,
			                                   l => l.Name,
			                                   (o, l) => new {OnlineValue = o.Value, LocalValue = l.Value});
			return enumerable
				.Any(n => n.OnlineValue != n.LocalValue);
		}

		public string GetMessage(TypeDef info)
		{
			return $"`{info.Name}` has a member that changed numeric value.";
		}
	}
}
