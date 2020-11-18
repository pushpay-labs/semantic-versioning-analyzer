using System.IO;
using Autofac;
using Pushpay.SemVerAnalyzer.Abstractions;
using Pushpay.SemVerAnalyzer.Engine;

namespace Pushpay.SemVerAnalyzer
{
	public class ExternalRuleModule : Module
	{
		readonly AppSettings _settings;
		readonly string _additionalRulesPath;

		public ExternalRuleModule(AppSettings settings, string additionalRulesPath)
		{
			_settings = settings;
			_additionalRulesPath = additionalRulesPath;
		}

		protected override void Load(ContainerBuilder builder)
		{
			if (string.IsNullOrEmpty(_additionalRulesPath)) return;

			if (Directory.Exists(_additionalRulesPath))
			{
				LoadDirectory(builder);
			}
			else if (File.Exists(_additionalRulesPath))
			{
				LoadFile(builder, _additionalRulesPath);
			}
		}

		void LoadDirectory(ContainerBuilder builder)
		{
			var files = Directory.EnumerateFiles(_additionalRulesPath, "*.dll");
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
