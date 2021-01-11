// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Pushpay.SemVerAnalyzer.Abstractions;

namespace Pushpay.SemVerAnalyzer.Engine
{
	public class Semver : IComparable<Semver>
	{
		public int Major { get;set; }
		public int Minor { get;set; }
		public int Patch { get;set; }
		public string Prerelease { get;set; }
		public string Trailer { get;set; }

		public Semver(int major, int minor, int patch, string prerelease, string trailer){
			Major = major;
			Minor = minor;
			Patch = patch;
			Prerelease = prerelease;
			Trailer = trailer;
		}

		public override string ToString() => $"{Major}.{Minor}.{Patch}{(Prerelease != null ? Prerelease : "")}{(Trailer != null ? Trailer : "")}";

		public int CompareTo(Semver other) {
			var difference = Major.CompareTo(other.Major);
			if (difference != 0) return difference;
			
			difference = Minor.CompareTo(other.Minor);
			if (difference != 0) return difference;

			difference = Patch.CompareTo(other.Patch);
			if (difference != 0) return difference;
			
			difference = Prerelease.CompareTo(other.Prerelease);
			if (difference != 0) return difference;

			return Trailer.CompareTo(other.Trailer);
		}

		public string GetSuggestedVersionAfterBump(VersionBumpType bump)
		{
			return bump switch
			{
				VersionBumpType.Downgrade => null,
				VersionBumpType.None => ToString(),
				VersionBumpType.Patch => $"{Major}.{Minor}.{Patch + 1}",
				VersionBumpType.Minor => $"{Major}.{Minor + 1}.0",
				VersionBumpType.Major => $"{Major + 1}.0.0",
				_ => throw new ArgumentOutOfRangeException(nameof(bump), bump, null)
			};
		}
	}
}
