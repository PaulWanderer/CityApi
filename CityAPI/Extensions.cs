using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityAPI
{
    public static class Extensions
    {
        /// <summary>
        /// Extension of Linq.Where for practice
        /// </summary>
        /// <param name="source">source data</param>
        /// <param name="predicate">predicate to be satisfied</param>
        /// <returns></returns>
        public static IEnumerable<T> Where<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            foreach (var i in source)
            {
                if (predicate(i))
                {
                    yield return i;
                }
            }
        }

        public static bool Any<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            foreach (var i in source)
            {
                if (predicate(i))
                {
                    return true;
                }
            }
            return false;
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var enumerable = source as T[] ?? source.ToArray();
            foreach (var i in enumerable)
            {
                if (predicate(i))
                {
                    return i;
                }
            }
            return enumerable.ToArray()[0];
        }
    }
}
