using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FainEngine_v2.Extensions
{
    public static class ListExtensions
    {
        /// <summary>
        /// Sorts the list based on a key selector that returns a numeric value.
        /// </summary>
        public static void SortBy<T, TKey>(this List<T> list, Func<T, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            list.Sort((a, b) => keySelector(a).CompareTo(keySelector(b)));
        }

        /// <summary>
        /// Sorts the list based on a key selector in descending order.
        /// </summary>
        public static void SortByDescending<T, TKey>(this List<T> list, Func<T, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            list.Sort((a, b) => keySelector(b).CompareTo(keySelector(a)));
        }

        /// <summary>
        /// Tries to dequeue (remove and return) the first element of the list.
        /// </summary>
        /// <typeparam name="T">Type of the list elements</typeparam>
        /// <param name="list">The list instance</param>
        /// <param name="item">The dequeued item if successful</param>
        /// <returns>True if an item was dequeued, false if the list is empty</returns>
        public static bool TryDequeueFirst<T>(this List<T> list, out T item)
        {
            if (list.Count > 0)
            {
                item = list[0];
                list.RemoveAt(0);
                return true;
            }

            item = default!;
            return false;
        }

        /// <summary>
        /// Tries to dequeue (remove and return) the last element of the list.
        /// </summary>
        /// <typeparam name="T">Type of the list elements</typeparam>
        /// <param name="list">The list instance</param>
        /// <param name="item">The dequeued item if successful</param>
        /// <returns>True if an item was dequeued, false if the list is empty</returns>
        public static bool TryDequeueLast<T>(this List<T> list, out T item)
        {
            int lastIndex = list.Count - 1;
            if (lastIndex >= 0)
            {
                item = list[lastIndex];
                list.RemoveAt(lastIndex);
                return true;
            }

            item = default!;
            return false;
        }
    }
}
