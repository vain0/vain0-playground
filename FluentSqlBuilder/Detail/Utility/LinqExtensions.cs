﻿using System.Collections.Generic;
using System.Linq;

namespace FluentSqlBuilder.Detail
{
    public static class LinqExtensions
    {
        public static bool IsEmpty<X>(this IEnumerable<X> xs)
        {
            return !xs.Any();
        }

        public static IEnumerable<X> Concat<X>(this IEnumerable<IEnumerable<X>> xxs)
        {
            return xxs.SelectMany(xs => xs);
        }

        /// <summary>
        /// シーケンスの各要素の間に separator を挟んだシーケンスを取得します。
        /// </summary>
        public static IEnumerable<X> Intersperse<X>(this IEnumerable<X> xs, X separator)
        {
            var isFirst = true;
            foreach (var x in xs)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    yield return separator;
                }

                yield return x;
            }
        }

        public static IEnumerable<X> Intercalate<X>(
            this IEnumerable<IEnumerable<X>> xxs,
            IEnumerable<X> separator
        )
        {
            return xxs.Intersperse(separator).Concat();
        }

        public static IEnumerable<X> Enclose<X>(this IEnumerable<X> xs, X left, X right)
        {
            yield return left;
            foreach(var x in xs)
            {
                yield return x;
            }
            yield return right;
        }

        public static bool IsSingle<X>(this IEnumerable<X> xs, X x)
        {
            return
                xs.Any() && !xs.Skip(1).Any()
                    ? xs.First().Equals(x)
                    : false;
        }
    }
}
