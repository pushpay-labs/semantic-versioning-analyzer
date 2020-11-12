using Autofac;

using Microsoft.Extensions.Configuration;

using Pushpay.SemVerAnalyzer.Engine;

namespace Pushpay.SemVerAnalyzer.Tests
{
	public class IntegrationFixture
	{
		readonly ContainerBuilder _builder;
		IContainer _container;

		public IContainer Container => _container ??= _builder.Build();

		public IAssemblyVersionAnalyzer AssemblyVersionAnalyzer => Container.Resolve<IAssemblyVersionAnalyzer>();
		public CompareCommandRunner CompareCommandRunner => Container.Resolve<CompareCommandRunner>();

		public IntegrationFixture()
		{
			_builder = new ContainerBuilder();
			ConfigureServices(BuildConfiguration());
		}

		public void OverrideRegistration<T>(T service) where T : class
		{
			_builder.RegisterInstance(service).AsImplementedInterfaces().AsSelf();
		}

		static IConfiguration BuildConfiguration()
		{
			var builder = new ConfigurationBuilder()
				.AddJsonFile("testsettings.json");

			return builder.Build();
		}

		void ConfigureServices(IConfiguration config)
		{
			var appSettings = config.GetSection("settings").Get<AppSettings>() ?? new AppSettings {DisabledRules = new string[0]};
			_builder.RegisterInstance(appSettings).AsSelf();

			_builder.RegisterModule(new AppModule(appSettings));

			_builder.RegisterType<FakeNugetClient>()
				.AsImplementedInterfaces()
				.AsSelf();
		}
	}
}
