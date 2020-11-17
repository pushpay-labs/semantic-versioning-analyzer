using System;
using System.Collections.Generic;
using System.Linq;

using dnlib.DotNet;

using Pushpay.SemVerAnalyzer.Assembly;

namespace Pushpay.SemVerAnalyzer.Engine
{
	internal class AssemblyVersionAnalyzer : IAssemblyVersionAnalyzer
	{
		readonly IEnumerable<IVersionRuleRunner> _ruleRunners;
		readonly IEnumerable<IVersionAnalysisRule<TypeDef>> _typeRules;
		readonly IEnumerable<IVersionAnalysisRule<AssemblyReference>> _assemblyRules;
		readonly AppSettings _settings;

		public AssemblyVersionAnalyzer(IEnumerable<IVersionRuleRunner> ruleRunners,
									   IEnumerable<IVersionAnalysisRule<TypeDef>> typeRules,
									   IEnumerable<IVersionAnalysisRule<AssemblyReference>> assemblyRules,
									   AppSettings settings)
		{
			_ruleRunners = ruleRunners;
			_typeRules = typeRules;
			_assemblyRules = assemblyRules;
			_settings = settings;
		}

		public VersionAnalysisResult AnalyzeVersions(AssemblyPublicInterface localPublicInterface,
													 AssemblyPublicInterface onlinePublicInterface)
		{
			var result = new VersionAnalysisResult();

			result.ActualBump = CompareVersions(onlinePublicInterface.Version, localPublicInterface.Version, result);
			result.CalculatedBump = AnalyzeInterfaces(onlinePublicInterface, localPublicInterface, result.Changes);

			return result;
		}

		static VersionBumpType CompareVersions(string online, string local, VersionAnalysisResult result)
		{
			if (online.MajorVersion() > local.MajorVersion()) {
				result.Comments.Add("Cannot downgrade major version.");
				return VersionBumpType.Downgrade;
			}

			if (online.MajorVersion() < local.MajorVersion()) {
				if (local.MinorVersion() != 0) {
					result.Comments.Add("The minor version should be reset to 0 when bumping the major version.");
				} else if (local.PatchVersion() != 0) {
					result.Comments.Add("The patch version should be reset to 0 when bumping the major version.");
				}

				return VersionBumpType.Major;
			}

			if (online.MinorVersion() > local.MinorVersion()) {
				result.Comments.Add("Cannot downgrade minor version.");
				return VersionBumpType.Downgrade;
			}

			if (online.MinorVersion() < local.MinorVersion()) {
				if (local.PatchVersion() != 0) {
					result.Comments.Add("The patch version should be reset to 0 when bumping the minor version.");
				}

				return VersionBumpType.Minor;
			}

			if (online.PatchVersion() > local.PatchVersion()) {
				result.Comments.Add("Cannot downgrade patch version.");
				return VersionBumpType.Downgrade;
			}

			if (online.PatchVersion() < local.PatchVersion()) {
				return VersionBumpType.Patch;
			}

			return VersionBumpType.None;
		}

		VersionBumpType AnalyzeInterfaces(AssemblyPublicInterface online, AssemblyPublicInterface local, List<Change> comments)
		{
			var overallBump = VersionBumpType.None;

			var references = online.References.FullOuterJoin(local.References,
															 o => o.Name,
															 l => l.Name,
															 (o, l) => new {Online = o, Local = l});
			foreach (var reference in references) {
				var changes = _assemblyRules.Where(r => r.Applies(reference.Online, reference.Local))
					.Select(r =>
					{
						var overrideType = _settings.GetOverrideType(r.GetType());
						return new Change {
							Bump = overrideType.ToBumpType(r.Bump),
							Message = r.GetMessage(reference.Local)
						};
					}).ToList();

				comments.AddRange(changes.Where(c => c.Bump != VersionBumpType.None));

				overallBump = Max(overallBump, changes.Select(c => c.Bump));
			}

			var types = online.Types.FullOuterJoin(local.Types,
												   o => o.FullName,
												   l => l.FullName,
												   (o, l) => new { Online = o, Local = l });
			foreach (var type in types)
			{
				var changes = _typeRules.Where(r => r.Applies(type.Online, type.Local))
					.Select(r =>
					{
						var overrideType = _settings.GetOverrideType(r.GetType());
						return new Change
						{
							Bump = overrideType.ToBumpType(r.Bump),
							Message = r.GetMessage(type.Online ?? type.Local)
						};
					}).ToList();

				changes.AddRange(_ruleRunners.SelectMany(a => a.Analyze(type.Online, type.Local)));

				comments.AddRange(changes.Where(c => c.Bump != VersionBumpType.None));

				overallBump = Max(overallBump, changes.Select(c => c.Bump));
			}

			return overallBump;
		}

		static VersionBumpType Max(VersionBumpType a, IEnumerable<VersionBumpType> others)
		{
			var othersList = others.ToList();

			if (othersList.Count == 0) {
				return a;
			}

			return (VersionBumpType)Math.Max((int)a, othersList.Max(b => (int)b));
		}
	}
}
