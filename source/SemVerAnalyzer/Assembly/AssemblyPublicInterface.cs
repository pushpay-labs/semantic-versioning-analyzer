using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using dnlib.DotNet;

using Pushpay.SemVerAnalyzer.Engine;

namespace Pushpay.SemVerAnalyzer.Assembly
{
	public class AssemblyPublicInterface
	{
		public string Version { get; private set; }
		public string Framework{ get; private set; }
		public IReadOnlyList<TypeDef> Types { get; private set; }
		public IReadOnlyList<AssemblyReference> References { get; private set; }

		public static AssemblyPublicInterface Load(byte[] assemblyBytes)
		{
			var modCtx = ModuleDef.CreateModuleContext();
			var module = ModuleDefMD.Load(assemblyBytes, modCtx);

			return BuildAssemblyPublicInterface(module);
		}

		public static AssemblyPublicInterface Load(string path)
		{
			var data = File.ReadAllBytes(path);

			return Load(data);
		}

		internal static AssemblyPublicInterface CreateForTest(params Type[] types)
		{
			if (types.Select(t => t.AssemblyQualifiedName).Distinct().Count() != 1)
				throw new NotSupportedException("This test method assumes that all types are from the same assembly.");

			var api = Load(types.First().Assembly.Location);

			api.Types = api.Types.Join(types,
					td => td.AssemblyQualifiedName,
					t => t.AssemblyQualifiedName,
					(td, t) => td)
				.ToList();

			return api;
		}

		static AssemblyPublicInterface BuildAssemblyPublicInterface(ModuleDefMD module)
		{
			var attributeData = module.Assembly.CustomAttributes;
			var informationalVersionAttribute = attributeData.FirstOrDefault(d => d.AttributeType.Name == nameof(AssemblyInformationalVersionAttribute));
			var informationVersion = informationalVersionAttribute.ConstructorArguments[0].Value.ToString().Split('+')[0];
			var assemblyReferenceCount = module.TablesStream.AssemblyRefTable.Rows;
			var targetFrameworkAttribute = attributeData.FirstOrDefault(d => d.AttributeType.Name == nameof(TargetFrameworkAttribute));
			var targetFramework = targetFrameworkAttribute.ConstructorArguments[0].Value.ToString();
			var references = new List<AssemblyReference>();
			// apparently the table is 1-indexed
			// https://github.com/0xd4d/dnlib/blob/master/src/DotNet/ModuleDefMD.cs#L588
			for (uint i = 1; i <= assemblyReferenceCount; i++) {
				var asmRef = module.ResolveAssemblyRef(i);
				references.Add(new AssemblyReference {
					Name = asmRef.Name,
					Version = $"{asmRef.Version.Major}.{asmRef.Version.Minor}.{asmRef.Version.Build}"
				});
			}

			var publicInterface = new AssemblyPublicInterface {
				Types = module.Types.Where(t => t.IsPublic).ToList(),
				Framework = targetFramework,
				Version = SemverExtensions.GetSemVer(informationVersion),
				References = references
			};

			return publicInterface;
		}
	}
}
