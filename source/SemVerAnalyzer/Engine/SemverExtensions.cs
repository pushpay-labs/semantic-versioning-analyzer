using System;
using System.Text.RegularExpressions;

namespace Pushpay.SemVerAnalyzer.Engine
{
	public static class SemverExtensions
	{
		static readonly Regex _versionFormat = new Regex(@"^(?<major>[0-9]+)\.(?<minor>[0-9]+)\.(?<patch>[0-9]+)((?<prerelease>-\w*)|(?<trailer>\.0))?(?<githash>\+[0-9a-f]{40})?$");

		public static int MajorVersion(this string version)
		{
			var match = _versionFormat.Match(version);
			if (!match.Success) throw new FormatException("Not a version");

			var major = int.Parse(match.Groups["major"].Value);

			return major;
		}

		public static int MinorVersion(this string version)
		{
			var match = _versionFormat.Match(version);
			if (!match.Success) throw new FormatException("Not a version");

			var major = int.Parse(match.Groups["minor"].Value);

			return major;
		}

		public static int PatchVersion(this string version)
		{
			var match = _versionFormat.Match(version);
			if (!match.Success) throw new FormatException("Not a version");

			var major = int.Parse(match.Groups["patch"].Value);

			return major;
		}

		public static string GetSemVer(string version)
		{
			var match = _versionFormat.Match(version);
			if (!match.Success) throw new FormatException("Not a version");

			var major = int.Parse(match.Groups["major"].Value);
			var minor = int.Parse(match.Groups["minor"].Value);
			var patch = int.Parse(match.Groups["patch"].Value);

			return $"{major}.{minor}.{patch}";
		}
	}
}
