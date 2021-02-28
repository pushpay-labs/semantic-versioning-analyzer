using System.Collections.Generic;
using Autofac;

using Microsoft.Extensions.Configuration;
using Pushpay.SemVerAnalyzer.Abstractions;
using Pushpay.SemVerAnalyzer.Engine;

namespace Pushpay.SemVerAnalyzer.Tests
{
	public class IntegrationFixture
	{
		IContainer _container;

		public ContainerBuilder Builder { get; }
		public AppSettings Settings { get; private set; }

		public IContainer Container => _container ??= Builder.Build();

		public IAssemblyVersionAnalyzer AssemblyVersionAnalyzer => Container.Resolve<IAssemblyVersionAnalyzer>();
		public CompareCommandRunner CompareCommandRunner => Container.Resolve<CompareCommandRunner>();
		public IEnumerable<IVersionAnalysisRule> Rules => Container.Resolve<IEnumerable<IVersionAnalysisRule>>();
		internal IEnumerable<IVersionRuleRunner> RuleRunners => Container.Resolve<IEnumerable<IVersionRuleRunner>>();

		public IntegrationFixture()
		{
			Builder = new ContainerBuilder();
			ConfigureServices(BuildConfiguration());
		}

		public void OverrideRegistration<T>(T service) where T : class
		{
			Builder.RegisterInstance(service).AsImplementedInterfaces().AsSelf();
		}

		static IConfiguration BuildConfiguration()
		{
			var builder = new ConfigurationBuilder()
				.AddJsonFile("testsettings.json");

			return builder.Build();
		}

		void ConfigureServices(IConfiguration config)
		{
			Settings = config.GetSection("settings").Get<AppSettings>() ??
			           new AppSettings {RuleOverrides = new Dictionary<string, RuleOverrideType>()};
			Builder.RegisterInstance(Settings).AsSelf();

			Builder.RegisterModule(new AppModule(Settings));

			Builder.RegisterType<FakeNugetClient>()
				.AsImplementedInterfaces()
				.AsSelf();
		}

		void ResetOptions()
		{
			Settings.AdditionalRulesPath = default;
			Settings.AssumeChanges = default;
			Settings.IncludeHeader = default;
			Settings.OmitDisclaimer = default;
			Settings.RuleOverrides.Clear();
		}

		public void ApplyOverrides(CompareCommand command)
		{
			ResetOptions();

			Settings.AdditionalRulesPath = command.AdditionalRulesPath ?? Settings.AdditionalRulesPath;
			Settings.IncludeHeader = command.IncludeHeader ?? Settings.IncludeHeader;
			Settings.OmitDisclaimer = command.OmitDisclaimer ?? Settings.OmitDisclaimer;
			Settings.AssumeChanges = command.AssumeChanges ?? Settings.AssumeChanges;
		}
	}
}
