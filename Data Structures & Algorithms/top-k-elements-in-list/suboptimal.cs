// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n log k) time / O(n) space
// -  bounded min-heap of size k   [bounded-min-heap-topk]
// -  ranks below optimal.cs (O(n) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  counts frequencies into a dictionary, then maintains a size-k min-heap
// -  over distinct values, evicting the smallest-frequency entry whenever
// -  the heap exceeds k.
// --------------------------------------------------------------------------

public class Solution
{
    public int[] TopKFrequent(int[] nums, int k)
    {
        // Step 1: how often does each value occur?
        var occurrences = new Dictionary<int, int>();

        foreach (int number in nums)
        {
            occurrences.TryGetValue(number, out int currentCount);
            occurrences[number] = currentCount + 1;
        }

        // Step 2: a MIN-heap holding at most k entries.
        // Priority = frequency, so the least frequent survivor sits on top and
        // is the first thing evicted once the heap outgrows k.
        var smallestFrequencyFirst = new PriorityQueue<int, int>();

        foreach (var entry in occurrences)
        {
            smallestFrequencyFirst.Enqueue(entry.Key, entry.Value);

            if (smallestFrequencyFirst.Count > k)
                smallestFrequencyFirst.Dequeue();
        }

        // Step 3: whatever remains IS the top k.
        int[] result = new int[k];

        for (int i = 0; i < k; i++)
        {
            result[i] = smallestFrequencyFirst.Dequeue();
        }

        return result;
    }
}

/*
================================================================================
 PATTERN : Heap - Bounded Min-Heap of size k ("keep the best k")
 SOURCE  : NeetCode / other resource (submission-0 + submission-1 merged,
           refactored: TryGetValue, named heap variable)
 STATUS  : Sub-optimal (O(n log k) vs the O(n) bucket-sort version)
================================================================================

WHY THIS PATTERN
  "Top k of n" does not need the whole set ordered. Hold a min-heap capped at
  k: the weakest candidate is always at the root, so admitting a new element
  and evicting the current worst is O(log k). You never sort the other n - k.

BRUTE FORCE (and why it fails)
  Count, then sort all distinct values by frequency and take k:
  O(n log n). Better than nothing, but it fully orders elements that will be
  thrown away. The heap orders only k of them.

THE COUNTER-INTUITIVE BIT
  For "top k LARGEST" you use a MIN-heap, not a max-heap. The heap's job is
  not to hand you the answer - it is to cheaply identify and drop the WEAKEST
  survivor. A max-heap of size k would give you fast access to the best,
  which is precisely the element you never want to remove.

INVARIANT
  After each iteration the heap holds the k most frequent values seen so far
  (fewer than k only while still filling up).

ALGORITHM (NeetCode: "Min-Heap")
  1. Count frequencies into a dictionary.               O(n)
  2. Push each (value, frequency) pair onto a min-heap.
     After every push, if Count > k, pop the root.      O(d log k), d distinct
  3. Drain the heap into the result array.              O(k log k)

COMPLEXITY
  Time  : O(n + d log k), commonly written O(n log k) since d <= n.
  Space : O(n) for the dictionary + O(k) for the heap.

  When k is small and n is huge - "top 10 of 10 million" - O(n log k) is
  effectively O(n) and the heap wins on MEMORY, which is the real reason
  this pattern dominates in streaming and log-processing systems.

TRIGGER
  "Top k / k largest / k smallest / k closest" - especially with a stream or
  an input too large to hold. If everything fits in memory AND frequencies
  are bounded by n, bucket sort (optimal.cs) beats it.

C# NOTES
  - PriorityQueue<TElement, TPriority> arrived in .NET 6. It is a MIN-heap by
    default: lowest priority value dequeues first.
  - For a max-heap, either negate the priority or pass
    Comparer<int>.Create((a, b) => b.CompareTo(a)) to the constructor.
  - It is NOT stable - equal priorities dequeue in unspecified order. Fine
    here because ties are equally valid answers.
  - EnqueueDequeue() does both in one sift when the heap is already full -
    cheaper than Enqueue-then-Dequeue.

WATCH OUT
  - The result comes out LEAST-frequent first (min-heap drain order). The
    problem accepts any order; if it demanded descending, reverse the array.
================================================================================
*/
