using System;
using System.Collections.Generic;

namespace ESFA.DC.FundingClaims.AtomFeed.Services
{
    public static class HierarchyExtensions
    {
        public static IEnumerable<T> SelectRecursive<T>(this T obj, Func<T, IEnumerable<T>> selector)
        {
            var processingQueue = new Queue<T>();

            processingQueue.Enqueue(obj);

            while (processingQueue.Count > 0)
            {
                var item = processingQueue.Dequeue();

                yield return item;

                var children = selector(item);

                if (children != null)
                {
                    foreach (var child in selector(item))
                    {
                        processingQueue.Enqueue(child);
                    }
                }
            }
        }
    }
}
