using System;
using System.Text.RegularExpressions;
using NuGet.Versioning;
using Pushpay.SemVerAnalyzer.Abstractions;

namespace Pushpay.SemVerAnalyzer.Engine
{
	public static class SemverExtensions
	{
		static readonly Regex _versionFormat = new Regex(@"^(?<major>[0-9]+)\.(?<minor>[0-9]+)\.(?<patch>[0-9]+)((?<prerelease>-\w*)|(?<trailer>\.0))?(?<githash>\+[0-9a-f]{40})?$");

		public static int MajorVersion(this string version) => version.ToSemver().Major;
		public static int MinorVersion(this string version) => version.ToSemver().Minor;
		public static int PatchVersion(this string version) => version.ToSemver().Patch;

		public static string GetSemVer(string version) => version.ToSemver().ToString();

		public static string GetSuggestedVersion(this string version, VersionBumpType bump) => version.ToSemver().GetSuggestedVersionAfterBump(bump);

		public static Semver ToSemver(this string version)
		{
			string prerelease;
			if (SemanticVersion.TryParse(version, out var result))
			{
				prerelease = result.IsPrerelease ? '-' + string.Join('.', result.ReleaseLabels) : string.Empty;
				return new Semver(result.Major, result.Minor, result.Patch, prerelease, string.Empty);
			}

			// This is kept to support trailers which are not part of SemVer2:
			var match = _versionFormat.Match(version);
			if (!match.Success) throw new FormatException("Not a version");
			
			var major = int.Parse(match.Groups["major"].Value);
			var minor = int.Parse(match.Groups["minor"].Value);
			var patch = int.Parse(match.Groups["patch"].Value);
			prerelease = match.Groups["prerelease"].Value;
			var trailer = match.Groups["trailer"].Value;
			return new Semver(major, minor, patch, prerelease, trailer);
		}

	}
}
