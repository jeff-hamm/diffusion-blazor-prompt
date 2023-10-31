namespace ButtsBlazor.Client.Services;

public static class RandomExtensions
{
    public static T NextRequired<T>(this Random random, T[] array) where T : notnull => array.Length > 0
        ? array[random.Next(array.Length)]
        : throw new InvalidOperationException($"Array was empty");
    public static T? Next<T>(this Random random, T[] array) where T:notnull => array.Length > 0 ? array[random.Next(array.Length)] : default;
    public static T? Next<T>(this Random random, IList<T> array) where T : notnull => array.Count > 0 ? array[random.Next(array.Count)] : default;

    public static IEnumerable<T> RemoveNext<T>(this Random random, IList<T> array, int? count=null) where T : IEquatable<T>
    {
        if(count == null || count > array.Count)
            count = array.Count;
        for (int i = 0; i < count; i++)
        {
            var ix = random.Next(array.Count);
            var item = array[ix];
            array.RemoveAt(ix);
            yield return item;
        }
    }
    public static bool NextChance(this Random random, double chance) =>  random.NextDouble() < chance;
}