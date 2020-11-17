using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Autofac;

using CommandLine;

using Microsoft.Extensions.Configuration;

namespace Pushpay.SemVerAnalyzer
{
	internal class Program
	{
		static IContainer _container;

		static async Task Main(string[] args)
		{
			await Parser.Default.ParseArguments<CompareCommand>(args)
				.WithParsedAsync(Compare);
		}

		static async Task Compare(CompareCommand command)
		{
			var config = BuildConfiguration(command.Configuration);
			ConfigureServices(config, command.AdditionalRulesPath);

			var validationResult = command.Validate();
			if (validationResult != null) {
				Console.WriteLine($"Error:\n\t{validationResult}");
				Environment.Exit(1);
			}

			var runner = _container.Resolve<CompareCommandRunner>();
			var report = await runner.Compare(command);

			if (!string.IsNullOrWhiteSpace(command.OutputPath))
				await File.WriteAllTextAsync(command.OutputPath, report);
			Console.WriteLine(report);
		}

		static IConfiguration BuildConfiguration(string configFilePath)
		{
			var builder = new ConfigurationBuilder()
				.AddJsonFile(Path.GetFullPath(configFilePath));

			return builder.Build();
		}

		static void ConfigureServices(IConfiguration config, string additionalRulesPath)
		{
			var builder = new ContainerBuilder();

			var appSettings = config.GetSection("settings").Get<AppSettings>() ??
			                  new AppSettings {RuleOverrides = new Dictionary<string, RuleOverrideType>()};
			builder.RegisterInstance(appSettings).AsSelf();
			var nugetConfig = config.GetSection("nuget").Get<NugetConfiguration>();
			if (nugetConfig?.RepositoryUrl == null)
			{
				Console.WriteLine("Nuget repository missing from configuration.");
				Environment.Exit(1);
			}
			builder.RegisterInstance(nugetConfig).AsSelf();

			builder.RegisterModule(new AppModule(appSettings));
			builder.RegisterModule(new ExternalRuleModule(appSettings, additionalRulesPath));

			_container = builder.Build();
		}
	}
}
