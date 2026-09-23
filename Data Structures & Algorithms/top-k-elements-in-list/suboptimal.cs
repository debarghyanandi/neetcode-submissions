// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n log k) time / O(n) space
// -  Min-heap with size-k limit   [min-heap-k-limit]
// -  ranks below optimal.cs (O(n) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  Frequency map stores all unique elements (O(n) worst case); heap
// -  maintains at most k entries with O(log k) per insertion over unique
// -  elements.
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
 PATTERN : Hash map count + bounded min-heap of size k
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Suboptimal
================================================================================
VARIABLES
  occurrences            occurrences[value] = how many times value appears in nums
  smallestFrequencyFirst min-heap of values, priority = frequency, capped at k entries
  result                 the k surviving values, popped in increasing frequency order
WHY THIS PATTERN
  The problem asks only for the k most frequent values, not for a full ranking,
  so a full sort of all distinct values does more work than needed. Counting in
  a Dictionary is the natural first step because frequency is the only thing the
  answer depends on. After that, a min-heap keyed by frequency keeps exactly the
  k best seen so far: the weakest survivor is always on top, so evicting it is
  O(log k) and never throws away a value that belonged in the answer.
BETTER APPROACH
  The better approach here is bucket sort: build an array of lists indexed by
  frequency (frequency can never exceed nums.Length), then walk that array from
  the high end and take values until you have k. That is O(n) time with no heap
  and no comparisons. This file loses because every distinct value pays a log k
  heap insert; bucket sort pays a constant-time list append instead. Sorting all
  distinct values by count would be worse still, O(m log m) for m distinct
  values.
INVARIANT
  After each Enqueue plus the conditional Dequeue, smallestFrequencyFirst holds
  exactly min(k, entries processed so far) values, and those are the most
  frequent among all entries processed so far. The eviction is safe because the
  element removed has the smallest frequency in the heap, so it is beaten by k
  other values and can never be in the final top k. When the loop ends every
  distinct value has been offered, so the k survivors are the global top k.
WATCH OUT
  The result comes out in increasing frequency order, since Dequeue pops the
  smallest first; if the problem or a test expects most-frequent-first you must
  reverse it. If k is larger than the number of distinct values, the heap holds
  fewer than k items and the final loop throws InvalidOperationException on an
  empty queue - the code trusts that k is valid without checking. The comment
  "whatever remains IS the top k" is only true under that same assumption. Also
  note ties in frequency are broken arbitrarily by heap order, so with equal
  counts the chosen values are not deterministic.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you get O(n) overall?
     Bucket sort by count: buckets[f] is a list of values with frequency f, f
     ranges 1..nums.Length. Walk buckets downward and collect k values. Cost is
     an extra array of size n+1, and you lose the small memory footprint of a
     k-sized heap.
  2. What if nums arrives as a stream too large to hold in memory?
     The Dictionary is the problem, not the heap - you still need one counter
     per distinct value. Use approximate counting (count-min sketch) plus this
     same size-k heap, and accept that the result may be slightly wrong near the
     cutoff.
  3. What if you must return the k values sorted from most to least frequent?
     Either reverse result after the final loop, or fill it backwards with
     result[k-1-i], both free. A max-heap over all m distinct values would also
     work but costs O(m) space instead of O(k).
  4. Can you avoid the heap entirely while keeping worst-case sub-n-log-n?
     Quickselect on the (value, count) pairs by count gives expected O(m) and
     partitions the top k in place, but the worst case degrades to O(m^2) and
     the output order is unspecified.
TRIGGER
  The question asks for the top k or bottom k by some score, not for a full
  ordering - keep a heap of size k with the opposite polarity of what you want.
C# NOTE
  PriorityQueue<TElement, TPriority> is a min-heap by default, which is why no
  custom IComparer is needed here; the element is the value and the priority is
  the count. TryGetValue with out int currentCount avoids a second hash lookup
  and relies on the out parameter being 0 when the key is missing, so no
  ContainsKey check is needed.
COMPLEXITY
  Time  : O(n log k)
  Space : O(n)
================================================================================
*/
