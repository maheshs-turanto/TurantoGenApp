using System.Collections.Generic;
namespace System.Linq
{
/// <summary>A linq extensions.</summary>
public static class LinqExtensions
{
    /// <summary>Enumerates group adjacent by in this collection.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="source">   Source collection.</param>
    /// <param name="predicate">The predicate.</param>
    ///
    /// <returns>An enumerator that allows foreach to be used to process group adjacent by in this
    /// collection.</returns>
    
    public static IEnumerable<IEnumerable<T>> GroupAdjacentBy<T>(
        this IEnumerable<T> source, Func<T, T, bool> predicate)
    {
        using(var e = source.GetEnumerator())
        {
            if(e.MoveNext())
            {
                var list = new List<T> { e.Current };
                var pred = e.Current;
                while(e.MoveNext())
                {
                    if(predicate(pred, e.Current))
                    {
                        list.Add(e.Current);
                    }
                    else
                    {
                        yield return list;
                        list = new List<T> { e.Current };
                    }
                    pred = e.Current;
                }
                yield return list;
            }
        }
    }
    
    /// <summary>This method extends the LINQ methods to flatten a collection of items that have a
    /// property of children of the same type.</summary>
    ///
    /// <typeparam name="T">Item type.</typeparam>
    /// <param name="source">               Source collection.</param>
    /// <param name="childPropertySelector">    Child property selector delegate of each item.
    ///                                         IEnumerable'T' childPropertySelector(T itemBeingFlattened)</param>
    ///
    /// <returns>Returns a one level list of elements of type T.</returns>
    
    public static IEnumerable<T> Flatten<T>(
        this IEnumerable<T> source,
        Func<T, IEnumerable<T>> childPropertySelector)
    {
        return source
               .Flatten((itemBeingFlattened, objectsBeingFlattened) =>
                        childPropertySelector(itemBeingFlattened));
    }
    
    /// <summary>This method extends the LINQ methods to flatten a collection of items that have a
    /// property of children of the same type.</summary>
    ///
    /// <typeparam name="T">Item type.</typeparam>
    /// <param name="source">               Source collection.</param>
    /// <param name="childPropertySelector">    Child property selector delegate of each item.
    ///                                         IEnumerable'T' childPropertySelector (T
    ///                                         itemBeingFlattened, IEnumerable'T' objectsBeingFlattened)</param>
    ///
    /// <returns>Returns a one level list of elements of type T.</returns>
    
    public static IEnumerable<T> Flatten<T>(
        this IEnumerable<T> source,
        Func<T, IEnumerable<T>, IEnumerable<T>> childPropertySelector)
    {
        return source
               .Concat(source
                       .Where(item => childPropertySelector(item, source) != null)
                       .SelectMany(itemBeingFlattened =>
                                   childPropertySelector(itemBeingFlattened, source)
                                   .Flatten(childPropertySelector)));
    }
}
}