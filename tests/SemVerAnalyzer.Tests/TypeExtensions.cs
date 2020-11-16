using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;

namespace Pushpay.SemVerAnalyzer.Tests
{
	public static class TypeExtensions
	{
		static readonly List<TypeDef> _types;

		static TypeExtensions()
		{
			var module = typeof(TypeExtensions).Module;
			var context = ModuleDef.CreateModuleContext();
			var moduleDef = ModuleDefMD.Load(module, context);
			_types = moduleDef.Types.ToList();
			var nestedTypes = _types.SelectMany(t => t.NestedTypes ?? Enumerable.Empty<TypeDef>()).ToList();
			_types.AddRange(nestedTypes);
		}

		public static TypeDef GetTypeDef(this Type type)
		{
			// Reflection uses a '+' to denote nested types, but dnlib uses a '/'.
			return _types.FirstOrDefault(t => t.FullName == type.FullName.Replace('+', '/'));
		}
	}
}
