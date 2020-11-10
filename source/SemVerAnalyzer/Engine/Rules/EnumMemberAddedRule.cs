using System.Linq;

using dnlib.DotNet;

namespace Pushpay.SemVerAnalyzer.Engine.Rules
{
	class EnumMemberAddedRule : IVersionAnalysisRule<TypeDef>
	{
		public VersionBumpType Bump => VersionBumpType.Minor;

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
				.Select(f => new { f.Name, Value = (int)f.Constant.Value })
				.ToList();
			var localValues = local.Fields
				.Where(f => f.Constant != null)
				.Select(f => new { f.Name, Value = (int)f.Constant.Value })
				.ToList();

			return localValues.Any(n => !onlineValues.Contains(n));
		}

		public string GetMessage(TypeDef info)
		{
			return $"`{info.Name}` has a new value.";
		}
	}
}
