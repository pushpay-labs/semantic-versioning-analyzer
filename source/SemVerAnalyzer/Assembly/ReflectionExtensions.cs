using System;
using System.Collections.Generic;
using System.Linq;

using dnlib.DotNet;

using Pushpay.SemVerAnalyzer.Engine;

namespace Pushpay.SemVerAnalyzer.Assembly
{
	internal static class ReflectionExtensions
	{
		public static IEnumerable<(EventDef Online, EventDef Local)> MatchEvents(this TypeDef online, TypeDef local)
		{
			return online.Events.FilterToPublic()
				.FullOuterJoin(local.Events.FilterToPublic(),
							   a => a.Name,
							   b => b.Name,
							   (a, b) => (a, b));
		}

		public static IEnumerable<(MethodDef Online, MethodDef Local)> MatchMethods(this TypeDef online, TypeDef local)
		{
			return online.Methods.FilterToPublic()
				.Where(m => !m.IsSetter && !m.IsGetter)
				.FullOuterJoin(local.Methods.FilterToPublic()
								   .Where(m => !m.IsSetter && !m.IsGetter),
							   a => a,
							   b => b,
							   (a, b) => (a, b),
							   MethodSignatureComparer.Instance);
		}

		public static IEnumerable<(PropertyDef Online, PropertyDef Local)> MatchProperties(this TypeDef online, TypeDef local)
		{
			return online.Properties.FilterToPublic()
				.FullOuterJoin(local.Properties.FilterToPublic(),
							   a => a.Name,
							   b => b.Name,
							   (a, b) => (a, b));
		}

		public static MethodDef GetUnderlyingMethodInfo(this IMemberDef member)
		{
			return member switch {
				MethodDef method => method,
				PropertyDef property => property.GetMethod ?? property.SetMethod,
				EventDef evt => evt.AddMethod ?? evt.RemoveMethod,
				_ => throw new NotSupportedException()
			};
		}

		static IEnumerable<T> FilterToPublic<T>(this IEnumerable<T> collection)
			where T : IMemberDef
		{
			return collection.Where(e => e.GetUnderlyingMethodInfo().IsPublic);
		}

	}
}
