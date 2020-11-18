using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using dnlib.DotNet;

namespace SemVerAnalyzer.Abstractions
{
	public static class CSharpNameGenerator
	{
		public static string GetName(this IMemberDef member)
		{
			return member switch {
				PropertyDef p => GetName(p),
				EventDef e => GetName(e),
				MethodDef m => GetName(m),
				TypeDef t => CSharpName(t),
				_ => null
			};
		}

		public static string GetName(this PropertyDef property)
		{
			return $"{property.DeclaringType.CSharpName()}.{property.Name}";
		}

		public static string GetName(this EventDef evt)
		{
			return $"{evt.DeclaringType.CSharpName()}.{evt.Name}";
		}

		public static string GetName(this MethodDef method)
		{
			var thisKeyword = method.CustomAttributes.Any(a => a.AttributeType.Name == nameof(ExtensionAttribute)) ? "this " : "";
			return $"{method.DeclaringType.CSharpName()}.{method.CSharpName()}({thisKeyword}{string.Join(", ", method.Parameters.Select(p => p.Type.TypeName))})";
		}

		static string CSharpName(this MethodDef method, StringBuilder sb = null)
		{
			var name = method.Name;
			if (!method.HasGenericParameters) {
				return name;
			}

			sb ??= new StringBuilder();
			sb.Append(name);
			sb.Append("<");
			sb.Append(string.Join(", ", method.GenericParameters
				                      .Select(t => t.Name)));
			sb.Append(">");
			return sb.ToString();
		}

		static string CSharpName(this TypeDef type, StringBuilder sb = null)
		{
			var name = type.Name;
			if (!type.HasGenericParameters) {
				return name;
			}

			sb ??= new StringBuilder();
			sb.Append(name.Substring(0, name.IndexOf('`')));
			sb.Append("<");
			sb.Append(string.Join(", ", type.GenericParameters
				                      .Select(t => t.Name)));
			sb.Append(">");
			return sb.ToString();
		}
	}
}
