using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orderly.Extensions
{
    public static class CollectionExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection == null) {
                throw new ArgumentNullException("collection");
            }

            if (action == null) {
                throw new ArgumentNullException("action");
            }

            foreach (T item in collection) {
                action(item);
            }
        }
    }
}
