using System;
using System.IO;
using System.Threading.Tasks;

using Autofac;

using CommandLine;

using Microsoft.Extensions.Configuration;

namespace Pushpay.SemVerAnalyzer
{
	class Program
	{
		static IContainer container;

		static async Task Main(string[] args)
		{
			var config = BuildConfiguration();
			ConfigureServices(config);

			await Parser.Default.ParseArguments<CompareCommand>(args)
				.WithParsedAsync(Compare);
		}

		static async Task Compare(CompareCommand command)
		{
			var validationResult = command.Validate();
			if (validationResult != null) {
				Console.WriteLine($"Error:\n\t{validationResult}");
				Environment.Exit(1);
			}

			var runner = container.Resolve<CompareCommandRunner>();
			var report = await runner.Compare(command);

			await File.WriteAllTextAsync(command.OutputPath, report);
			Console.WriteLine(report);
		}

		static IConfiguration BuildConfiguration()
		{
			var builder = new ConfigurationBuilder()
				.AddJsonFile("config.json");

			return builder.Build();
		}

		static void ConfigureServices(IConfiguration config)
		{
			var builder = new ContainerBuilder();

			var appSettings = config.GetSection("settings").Get<AppSettings>();
			builder.RegisterInstance(appSettings).AsSelf();
			var nugetConfig = config.GetSection("nuget").Get<NugetConfiguration>();
			builder.RegisterInstance(nugetConfig).AsSelf();

			builder.RegisterModule(new AppModule(appSettings));

			container = builder.Build();
		}
	}
}
