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

		[Option('r', "additional-rules", Required = false, HelpText = "A path to a single assembly or folder of assemblies which contain additional rules.  Overrides `additionalRules` setting in JSON configuration file.")]
		public string AdditionalRulesPath { get; set; }
		
		public string FullAssemblyPath => Path.GetFullPath(Assembly);
		public string PackageName { get; set; }

		public string Validate()
		{
			var match = Regex.Match(Assembly, @"^(.*(\/|\\))?(?<packageName>.*)\.dll$", RegexOptions.IgnoreCase);
			if (!match.Success) {
				return "Cannot extract package name from provided assembly file name";
			}

			if (!File.Exists(Assembly)) {
				return $"Cannot find assembly file '{Assembly}'";
			}

			if (!File.Exists(Configuration)) {
				return $"Cannot find assembly file '{Assembly}'";
			}

			PackageName = match.Groups["packageName"].Value;

			return null;
		}
	}
}
