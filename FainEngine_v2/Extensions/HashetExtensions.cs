namespace FainEngine_v2.Extensions;

public static class HashSetExtensions
{
    /// <summary>
    /// Takes the count entries with the smallest scores (according to func),
    /// removes them from the source dictionary, and returns them as a list
    /// sorted by increasing score.
    /// </summary>
    public static List<T> TakeLowest<T>(this HashSet<T> set, Func<T, uint> func, int count) where T : notnull
    {
        // Max-heap implemented by storing negative scores in a min-heap
        var heap = new PriorityQueue<T, long>();

        foreach (var val in set)
        {
            uint score = func.Invoke(val);
            // enqueue with priority = –score so that the largest score in the heap
            // is the one with the smallest priority (i.e. most negative)
            heap.Enqueue(val, -(long)score);

            // if we exceed count items, remove the one with the highest score so far
            if (heap.Count > count)
                heap.Dequeue();
        }

        // Extract the survivors (they are the count smallest by score),
        // but in reverse order (largest-to-smallest), so we reverse at the end.
        var result = new List<T>(heap.Count);
        while (heap.Count > 0)
            result.Add(heap.Dequeue());

        result.Reverse(); // now sorted by increasing score

        // Finally, remove them from the original dictionary
        foreach (var val in result)
            set.Remove(val);

        return result;
    }
}