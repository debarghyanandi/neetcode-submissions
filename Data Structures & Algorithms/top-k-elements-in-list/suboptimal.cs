// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n log k) time / O(n) space
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
 PATTERN : Heap / Top-K - min-heap of size k keyed by frequency
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Suboptimal
================================================================================
VARIABLES
  occurrences             occurrences[x] = how many times x appears in nums
  currentCount            count of number so far; 0 when number is not yet a key
  smallestFrequencyFirst  min-heap of values; priority = frequency; least frequent on top
WHY THIS PATTERN
  The problem asks for the "k most frequent" values. That means we only need the
  top k, not a full ordering. First, occurrences turns the task into "pick the k
  largest counts". The min-heap smallestFrequencyFirst keeps only the k best
  seen so far. Its weakest member sits on top, so it is cheap to throw it out
  when a better value arrives.
BETTER APPROACH
  The better approach is bucket sort by frequency. Make an array of lists, where
  buckets[f] holds every value that appears exactly f times. A frequency can
  never be larger than nums.Length, so the array has at most nums.Length + 1
  slots. Then walk from the highest f down and collect values until you have k.
  This takes O(n) time. This file loses because every Enqueue and Dequeue on
  smallestFrequencyFirst costs log k. Bucket sort uses the fact that frequencies
  are small integers, so it needs no comparisons at all.
INVARIANT
  After each entry of occurrences is handled, smallestFrequencyFirst holds the k
  most frequent values seen so far (or all of them, if fewer than k have been
  seen). Here is why. A new entry is enqueued first. Then, if Count > k, the
  minimum is removed. The removed value has the lowest frequency among k + 1
  candidates, so it cannot be in the top k. If the new entry is itself the
  weakest, it is the one removed. When the loop ends, every distinct value has
  been compared this way, so the heap holds the true top k.
WATCH OUT
  The comment says the heap holds "at most k entries". That is not true: right
  after Enqueue it holds k + 1 for a moment, before Dequeue brings it back to k.
  The result comes out in ascending frequency order, because Dequeue pops the
  least frequent first. So result[0] is the k-th most frequent value, not the
  most frequent. If the caller expects descending order, fill result from index
  k - 1 down to 0. If k is larger than the number of distinct values, the Step 3
  loop calls Dequeue on an empty queue, and that throws
  InvalidOperationException. If two values tie at the cut-off frequency, which
  one stays depends on the heap's internal order.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you get average O(n) without bucket sort?
     Yes. Use quickselect (the partition step from quicksort) on the distinct
     values, ordered by occurrences[x], to find the k-th largest count. It is
     O(n) on average but O(n^2) in the worst case, and it reorders the array in
     place.
  2. What if nums arrives as a stream that is too large to keep?
     The exact occurrences map needs memory for every distinct value. If that is
     too much, use an approximate counter such as Count-Min Sketch (a small
     fixed-size table of hashed counters) or the Misra-Gries algorithm (keeps a
     limited number of counters). You get less memory in exchange for
     approximate answers.
  3. What if k is close to the number of distinct values?
     Flip it. Keep a max-heap of the (distinct - k) least frequent values, and
     return everything else. The heap stays small, and the log factor shrinks to
     log(distinct - k).
TRIGGER
  The problem asks for the "k largest / most frequent / closest" items, and a
  full sort would do more work than needed.
C# NOTE
  When Count == k, PriorityQueue.EnqueueDequeue(entry.Key, entry.Value) does the
  push and pop as one call. It returns the new item right away if its priority
  is not larger than the current minimum. This replaces the separate Enqueue
  then Dequeue pair.
COMPLEXITY
  Time  : O(n log k)
  Space : O(n)
================================================================================
*/
