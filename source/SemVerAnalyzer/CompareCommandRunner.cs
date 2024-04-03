using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pushpay.SemVerAnalyzer.Assembly;
using Pushpay.SemVerAnalyzer.Engine;
using Pushpay.SemVerAnalyzer.Nuget;

namespace Pushpay.SemVerAnalyzer
{
	public class CompareCommandRunner
	{
		readonly INugetClient _nugetClient;
		readonly IAssemblyVersionAnalyzer _analyzer;
		readonly AppSettings _settings;

		public CompareCommandRunner(INugetClient nugetClient,
			IAssemblyVersionAnalyzer analyzer,
			AppSettings settings)
		{
			_nugetClient = nugetClient;
			_analyzer = analyzer;
			_settings = settings;
		}

		public async Task<string> Compare(CompareCommand command)
		{
			string report = null;
			AssemblyPublicInterface localAssembly = null;
			var comments = new List<string>();
			try
			{
				localAssembly = AssemblyPublicInterface.Load(command.FullAssemblyPath);

				var bytes = await _nugetClient.GetAssemblyBytesFromPackage(command.PackageName, command.AssemblyFileName, localAssembly.Framework, comments);
				if (bytes == null)
				{
					return $"An error has current processing your request:\n\n" +
					       $"- {string.Join("\n- ", comments)}";
				}

				var onlineAssembly = AssemblyPublicInterface.Load(bytes);

				var result = _analyzer.AnalyzeVersions(localAssembly, onlineAssembly);

				if (_settings.ShowChanges || result.ActualBump != result.CalculatedBump)
				{
					comments = result.GetAllComments();
					if (_settings.IncludeHeader)
					{
						report = command.AssemblyFileName == command.PackageName
							? $"# {command.AssemblyFileName}\n\n"
							: $"# {command.AssemblyFileName} ( {command.PackageName} )\n\n";
					}
					if (!_settings.OmitDisclaimer)
					{
						report += "*This is a sanity check that indicates whether the change is more severe than intended.  " +
						         "It is not foolproof as not all changes can be detected by analyzing the public interface of an " +
						         "assembly.  Therefore the results of this check are to be considered as a suggestion.  You may determine " +
						         "that a particular change warrants a more severe version bump than is suggested by this check.*\n\n" +
						         "**Please use your best judgment when updating the version.  You know your change better than this check can.**\n\n";
					}
					report += $"## Summary\n\n" +
					          $"Actual new version: `{localAssembly.Version}` ({result.ActualBump})\n" +
					          $"Suggested new version: `{onlineAssembly.Version.GetSuggestedVersion(result.CalculatedBump)}` ({result.CalculatedBump}).\n";
					if (comments.Any())
					{
						report += $"\n## Details\n\n" +
						          $"- {string.Join("\n- ", comments)}\n";
					}
				}
			}
			catch (Exception e)
			{
				var sb = new StringBuilder();
				var which = localAssembly == null ? "built" : "online";
				sb.AppendLine($"An error occurred while attempting to load the {which} assembly.  Analysis cannot continue.");
				sb.AppendLine("Exception:");
				sb.AppendLine(e.Message);
				sb.AppendLine(e.StackTrace);
				if (e.InnerException != null)
				{
					sb.AppendLine("Inner Exception:");
					sb.AppendLine(e.InnerException.Message);
					sb.AppendLine(e.InnerException.StackTrace);
				}

				report = sb.ToString();
			}

			return report;
		}
	}
}
