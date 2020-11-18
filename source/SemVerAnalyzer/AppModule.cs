using Autofac;
using Pushpay.SemVerAnalyzer.Abstractions;
using Pushpay.SemVerAnalyzer.Engine;
using Pushpay.SemVerAnalyzer.Nuget;

namespace Pushpay.SemVerAnalyzer
{
	public class AppModule : Module
	{
		readonly AppSettings _settings;

		public AppModule(AppSettings settings)
		{
			_settings = settings;
		}

		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterAssemblyTypes(ThisAssembly)
				.Where(t => t.IsAssignableTo<IVersionRuleRunner>())
				.AsImplementedInterfaces()
				.AsSelf();

			builder.RegisterAssemblyTypes(ThisAssembly)
				.Where(t => t.IsAssignableTo<IVersionAnalysisRule>() && _settings.GetOverrideType(t) != RuleOverrideType.Ignore)
				.AsImplementedInterfaces()
				.AsSelf();

			builder.RegisterType<NugetClient>()
				.AsImplementedInterfaces()
				.AsSelf();
			builder.RegisterType<AssemblyVersionAnalyzer>()
				.AsImplementedInterfaces()
				.AsSelf();
			builder.RegisterType<CompareCommandRunner>()
				.AsSelf();
		}
	}
}
