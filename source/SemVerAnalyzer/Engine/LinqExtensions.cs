using System;
using System.Collections.Generic;
using System.Linq;

namespace Pushpay.SemVerAnalyzer.Engine
{
	// Joins are confusing: https://www.diffen.com/difference/Inner_Join_vs_Outer_Join
	internal static class LinqExtensions
	{
		public static IEnumerable<TOut> LeftJoin<TA, TB, TKey, TOut>(this IEnumerable<TA> aItems,
																	 IEnumerable<TB> bItems,
																	 Func<TA, TKey> aKeySelector,
																	 Func<TB, TKey> bKeySelector,
																	 Func<TA, TB, TOut> selector)
		{
			return aItems.GroupJoin(bItems,
									aKeySelector,
									bKeySelector,
									(a, bSet) => new {a, bSet})
				.SelectMany(t => t.bSet.DefaultIfEmpty(),
							(t, b) => selector(t.a, b));
		}

		public static IEnumerable<TOut> LeftJoin<TA, TB, TKey, TOut>(this IEnumerable<TA> aItems,
																	 IEnumerable<TB> bItems,
																	 Func<TA, TKey> aKeySelector,
																	 Func<TB, TKey> bKeySelector,
																	 Func<TA, TB, TOut> selector,
																	 IEqualityComparer<TKey> comparer)
		{
			return aItems.GroupJoin(bItems,
									aKeySelector,
									bKeySelector,
									(a, bSet) => new {a, bSet},
									comparer)
				.SelectMany(t => t.bSet.DefaultIfEmpty(),
							(t, b) => selector(t.a, b));
		}

		public static IEnumerable<TOut> FullOuterJoin<TA, TB, TKey, TOut>(this IEnumerable<TA> aItems,
																	  IEnumerable<TB> bItems,
																	  Func<TA, TKey> aKeySelector,
																	  Func<TB, TKey> bKeySelector,
																	  Func<TA, TB, TOut> selector)
		{
			var fromA = LeftJoin(aItems,
								 bItems,
								 aKeySelector,
								 bKeySelector,
								 (a, b) => new {a, b});
			var fromB = LeftJoin(bItems,
								 aItems,
								 bKeySelector,
								 aKeySelector,
								 (b, a) => new {a, b});

			return fromA.Union(fromB).Select(x => selector(x.a, x.b));
		}

		public static IEnumerable<TOut> FullOuterJoin<TA, TB, TKey, TOut>(this IEnumerable<TA> aItems,
																	  IEnumerable<TB> bItems,
																	  Func<TA, TKey> aKeySelector,
																	  Func<TB, TKey> bKeySelector,
																	  Func<TA, TB, TOut> selector,
																	  IEqualityComparer<TKey> comparer)
		{
			var fromA = LeftJoin(aItems,
								 bItems,
								 aKeySelector,
								 bKeySelector,
								 (a, b) => new {a, b},
								 comparer);
			var fromB = LeftJoin(bItems,
								 aItems,
								 bKeySelector,
								 aKeySelector,
								 (b, a) => new {a, b},
								 comparer);

			return fromA.Union(fromB).Select(x => selector(x.a, x.b));
		}
	}
}
