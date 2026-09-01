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
 PATTERN : Frequency map + bounded min-heap of size k
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Suboptimal
================================================================================
WHY THIS PATTERN
  Two independent questions get two separate structures. "How many times does
  each value appear?" is a pure counting question, answered by the occurrences
  dictionary in one pass. "Which k counts are the largest?" is a selection
  question over the distinct values, answered by smallestFrequencyFirst. Nothing
  about the counting pass needs to know k, and nothing about the selection pass
  needs to touch nums again - once occurrences is built, the original array is
  dead weight.
WHY A MIN-HEAP
  The instinct is a max-heap: you want the largest frequencies, so put the
  largest on top. That is backwards for a bounded heap. You never need fast
  access to the winner - you need fast access to the current worst survivor,
  because that is the one you throw away when the heap outgrows k.
  PriorityQueue<int,int> is a min-heap by priority, and the priority passed to
  Enqueue is entry.Value, the frequency. So the root is always the least
  frequent element currently held, and Dequeue() evicts exactly the right
  candidate. A max-heap would put the wrong end on top and force you to scan for
  the minimum.
INVARIANT
  After each iteration of the foreach over occurrences, smallestFrequencyFirst
  contains exactly min(k, entries processed so far) values, and they are the
  most frequent among the entries processed so far. Count never exceeds k,
  because the check runs immediately after every Enqueue and removes at most the
  one element that pushed it to k+1.
CORRECTNESS ARGUMENT
  The eviction is safe because of a strict dominance argument, not because of
  ordering luck. When Count hits k+1 and you Dequeue, the removed element has
  the smallest frequency among those k+1 - meaning there are k other distinct
  values already known to have frequency greater than or equal to it. A value
  with k values at least as frequent as it cannot belong in the final answer,
  and adding more entries later only adds more competitors, never fewer. So
  discarding it is permanent and correct. Ties are broken arbitrarily by heap
  order, which is fine: the problem accepts any valid top-k set, and any tied
  element is interchangeable with the one that displaced it.
WHY THIS LOSES
  The log k factor is avoidable. Frequencies of an n-element array are bounded
  by n, so they can be bucketed: build an array buckets of length nums.Length +
  1, put each key into buckets[count], then walk the buckets from high index
  down, collecting keys until you have k. That is counting sort on a bounded key
  range - linear, no comparisons, no heap. The heap version here is still worth
  knowing because it is the one that generalizes: bucketing only works because
  frequency is a small non-negative integer with a known ceiling. Replace
  "frequency" with a float score or a stream of unknown length and the bounded
  min-heap is the only one of the two that still applies.
WATCH OUT
  1. The final loop calls Dequeue() exactly k times with no guard. If k exceeds
  occurrences.Count, the heap holds fewer than k entries and Dequeue throws
  InvalidOperationException. This leans entirely on the problem constraint that
  k is at most the number of distinct values.
  2. result comes out in ascending frequency order - result[0] is the least
  frequent of the winners, result[k-1] the most frequent. If a variant of the
  problem asks for descending order, fill the array backwards instead of adding
  a sort.
  3. occurrences.TryGetValue(number, out int currentCount) is being used for its
  zero-default on a miss, not for its bool return, which is deliberately
  ignored. Reading it as a normal lookup misses the point.
  4. Enqueue-then-conditional-Dequeue briefly grows the heap to k+1.
  PriorityQueue exposes EnqueueDequeue, which does the same job in one operation
  once Count is already k; the current form is clearer but does strictly more
  work per entry.
TRIGGER
  Reach for a bounded min-heap when the ask is "the k best of n" with k much
  smaller than n, and you only ever need to compare against the current worst
  survivor. The tell is that you can afford to make an irreversible discard
  decision per element: if seeing a future element could resurrect one you
  already dropped, this pattern does not apply and you need a full sort or a
  different structure.
COMPLEXITY
  Time  : O(n log k)
  Space : O(n)
================================================================================
*/
