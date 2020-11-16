using Autofac;

using Microsoft.Extensions.Configuration;

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
			Settings = config.GetSection("settings").Get<AppSettings>() ?? new AppSettings {DisabledRules = new string[0]};
			Builder.RegisterInstance(Settings).AsSelf();

			Builder.RegisterModule(new AppModule(Settings));

			Builder.RegisterType<FakeNugetClient>()
				.AsImplementedInterfaces()
				.AsSelf();
		}
	}
}
