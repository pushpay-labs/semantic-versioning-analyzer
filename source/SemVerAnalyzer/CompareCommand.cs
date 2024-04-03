using System.IO;
using System.Text.RegularExpressions;

using CommandLine;

namespace Pushpay.SemVerAnalyzer
{
	public class CompareCommand
	{
		[Option('a', "assembly", Required = true, HelpText = "The built assembly to test.")]
		public string Assembly { get; set; }

		[Option('o', "outputPath", Required = false, HelpText = "The output file path for the report.")]
		public string OutputPath { get; set; }

		[Option('c', "config", Required = true, HelpText = "Path to the configuration file.")]
		public string Configuration { get; set; }

		[Option('r', "additional-rules", Required = false, HelpText = "A path to a single assembly or folder of assemblies which contain additional rules.")]
		public string AdditionalRulesPath { get; set; }

		[Option('p', "package-name", HelpText = "If the package name is different than the DLL file name, specify it here.")]
		public string PackageName { get; set; }

		[Option("omit-disclaimer", HelpText = "Omits the disclaimer paragraph that appears at the top of the output.")]
		public bool? OmitDisclaimer { get; set; }

		[Option('h', "include-header", HelpText = "Includes a header with the assembly and package at the top of the output.")]
		public bool? IncludeHeader { get; set; }

		[Option("assume-changes", HelpText = "Assumes that something changed, making Patch the lowest bump rather than None. Default is false.")]
		public bool? AssumeChanges { get; set; }

		[Option("show-changes", HelpText = "Show all changes, even if the version is as expected. Default is false.")]
		public bool? ShowChanges { get; set; }

		[Option('f', "framework", Required = false, HelpText = "Indicates the framework from the Nuget package to use as a comparison.")]
		public string Framework { get; set; }

		public string FullAssemblyPath => Path.GetFullPath(Assembly);
		public string AssemblyFileName => Path.GetFileNameWithoutExtension(Assembly);

		public string Validate()
		{
			if (string.IsNullOrWhiteSpace(PackageName))
			{
				var match = Regex.Match(Assembly, @"^(.*(\/|\\))?(?<packageName>.*)\.dll$", RegexOptions.IgnoreCase);
				if (!match.Success)
					return "Cannot extract package name from provided assembly file name";
				PackageName = match.Groups["packageName"].Value;
			}

			if (!File.Exists(Assembly))
				return $"Cannot find assembly file '{Assembly}'";

			if (!File.Exists(Configuration))
				return $"Cannot find assembly file '{Assembly}'";

			return null;
		}
	}
}
