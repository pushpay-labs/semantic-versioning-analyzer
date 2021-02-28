using System.IO;
using Autofac;
using Pushpay.SemVerAnalyzer.Abstractions;

namespace Pushpay.SemVerAnalyzer
{
	public class ExternalRuleModule : Module
	{
		readonly AppSettings _settings;

		public ExternalRuleModule(AppSettings settings)
		{
			_settings = settings;
		}

		protected override void Load(ContainerBuilder builder)
		{
			if (string.IsNullOrEmpty(_settings.AdditionalRulesPath)) return;

			if (Directory.Exists(_settings.AdditionalRulesPath))
			{
				LoadDirectory(builder);
			}
			else if (File.Exists(_settings.AdditionalRulesPath))
			{
				LoadFile(builder, _settings.AdditionalRulesPath);
			}
		}

		void LoadDirectory(ContainerBuilder builder)
		{
			var files = Directory.EnumerateFiles(_settings.AdditionalRulesPath, "*.dll");
			foreach (var file in files)
			{
				LoadFile(builder, file);
			}
		}

		void LoadFile(ContainerBuilder builder, string path)
		{
			var fullPath = Path.GetFullPath(path);
			var assembly = System.Reflection.Assembly.LoadFile(fullPath);

			builder.RegisterAssemblyTypes(assembly)
				.Where(t => t.IsAssignableTo<IVersionAnalysisRule>() && _settings.GetOverrideType(t) != RuleOverrideType.Ignore)
				.AsImplementedInterfaces()
				.AsSelf();
		}
	}
}
