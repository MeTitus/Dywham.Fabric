using System.Collections.Generic;
using System.Linq;

namespace Dywham.Fabric.Utils.Collections
{
    public static class CollectionExtensionMethods
    {
        public static IEnumerable<IEnumerable<T>> SplitIntoSets<T> (this IEnumerable<T> source, int itemsPerSet)
        {
            var sourceList = source as List<T> ?? source.ToList();
            
            for (var index = 0; index < sourceList.Count; index += itemsPerSet)
            {
                yield return sourceList.Skip(index).Take(itemsPerSet);
            }
        }
    }
}