using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using dnlib.DotNet;

using Pushpay.SemVerAnalyzer.Engine;

namespace Pushpay.SemVerAnalyzer.Assembly
{
	public class AssemblyPublicInterface
	{
		public string Version { get; private set; }
		public IReadOnlyList<TypeDef> Types { get; private set; }
		public IReadOnlyList<AssemblyReference> References { get; private set; }

		public static AssemblyPublicInterface Load(byte[] assemblyBytes)
		{
			// Create a default assembly resolver and type resolver and pass it to Load().
			// If it's a .NET Core assembly, you'll need to disable GAC loading and add
			// .NET Core reference assembly search paths.
			var modCtx = ModuleDef.CreateModuleContext();
			var module = ModuleDefMD.Load(assemblyBytes, modCtx);

			return BuildAssemblyPublicInterface(module);
		}

		public static AssemblyPublicInterface Load(string path)
		{
			var data = File.ReadAllBytes(path);

			return Load(data);
		}

		static AssemblyPublicInterface BuildAssemblyPublicInterface(ModuleDefMD module)
		{
			var attributeData = module.Assembly.CustomAttributes;
			var informationalVersionAttribute = attributeData.FirstOrDefault(d => d.AttributeType.Name == nameof(AssemblyInformationalVersionAttribute));
			var informationVersion = informationalVersionAttribute.ConstructorArguments[0].Value.ToString().Split('+')[0];
			var assemblyReferenceCount = module.TablesStream.AssemblyRefTable.Rows;
			var references = new List<AssemblyReference>();
			// apparently the table is 1-indexed
			for (uint i = 1; i <= assemblyReferenceCount; i++) {
				var asmRef = module.ResolveAssemblyRef(i);
				references.Add(new AssemblyReference {
					Name = asmRef.Name,
					Version = $"{asmRef.Version.Major}.{asmRef.Version.Minor}.{asmRef.Version.Build}"
				});
			}

			var publicInterface = new AssemblyPublicInterface {
				Types = module.Types.Where(t => t.IsPublic).ToList(),
				Version = SemverExtensions.GetSemVer(informationVersion),
				References = references
			};

			return publicInterface;
		}
	}
}
