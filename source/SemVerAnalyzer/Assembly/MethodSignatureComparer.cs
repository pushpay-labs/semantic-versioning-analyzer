using System.Collections.Generic;
using System.Linq;

using dnlib.DotNet;

namespace Pushpay.SemVerAnalyzer.Assembly
{
	public class MethodSignatureComparer : IEqualityComparer<MethodDef>
	{
		public static readonly MethodSignatureComparer Instance = new MethodSignatureComparer();

		public bool Equals(MethodDef x, MethodDef y)
		{
			if (x == null && y == null) {
				return true;
			}

			if (x == null || y == null) {
				return false;
			}

			return EqualNames(x, y) &&
				   EqualGenericParameters(x, y) &&
				   EqualTypes(x.ReturnType, y.ReturnType) &&
				   EqualParameters(x, y);
		}

		public int GetHashCode(MethodDef obj)
		{
			return obj.Name.GetHashCode() ^ obj.Parameters.Count; // everything else would be too cumbersome
		}

		static bool EqualGenericParameters(MethodDef x, MethodDef y)
		{
			if (x.HasGenericParameters != y.HasGenericParameters) {
				return false;
			}

			if (x.HasGenericParameters) {
				var xArgs = x.GenericParameters;
				var yArgs = y.GenericParameters;

				if (xArgs.Count != yArgs.Count) {
					return false;
				}

				for (var i = 0; i < xArgs.Count; ++i) {
					if (xArgs[i].Name != yArgs[i].Name) {
						return false;
					}
					// TODO: check constraints
				}
			}

			return true;
		}

		static bool EqualParameters(MethodDef x, MethodDef y)
		{
			var xArgs = x.Parameters.Where(p => p.IsNormalMethodParameter).ToList();
			var yArgs = y.Parameters.Where(p => p.IsNormalMethodParameter).ToList();

			if (xArgs.Count != yArgs.Count) {
				return false;
			}

			for (var i = 0; i < xArgs.Count; ++i) {
				if (!EqualTypes(xArgs[i].Type, yArgs[i].Type)) {
					return false;
				}
			}

			return true;
		}

		static bool EqualTypes(TypeSig x, TypeSig y)
		{
			if (x == null || y == null) {
				return false;
			}

			if (x.IsGenericInstanceType != y.IsGenericInstanceType) {
				return false;
			}

			if (x.IsGenericInstanceType) {
				var xArgs = ((GenericInstSig)x).GenericArguments;
				var yArgs = ((GenericInstSig)y).GenericArguments;

				if (xArgs.Count != yArgs.Count) {
					return false;
				}

				for (var i = 0; i < xArgs.Count; ++i) {
					if (!EqualTypes(xArgs[i], yArgs[i])) {
						return false;
					}
				}
			} else {
				if (x.FullName != y.FullName) {
					return false;
				}
			}

			return true;
		}

		static bool EqualNames(MethodDef x, MethodDef y)
		{
			return x.Name == y.Name;
		}
	}
}
