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
 PROBLEM : Given an integer array nums and an integer k, return the k values
           that appear most often. The answer may be in any order, and it is
           unique. Example: nums = [1,1,1,2,2,3], k = 2 -> [1,2].
 PATTERN : Hash Map counting + Min-Heap of size k
================================================================================
IDEA
  First count each value in the dictionary occurrences. Then push every
  (value, count) pair into the min-heap smallestFrequencyFirst, where the
  priority is the count. When the heap holds more than k items, pop the top,
  which is the least frequent. This is correct because a value can only be
  evicted when k values with a count at least as high are kept. optimal.cs
  uses buckets by frequency instead of a heap.
EXAMPLE
  nums = [1,1,1,2,2,3], k = 2 -> occurrences {1:3, 2:2, 3:1}
  push 1(3), push 2(2), push 3(1) -> Count 3 > 2, Dequeue removes 3
  drain heap: Dequeue 2, then Dequeue 1 -> result = [2,1]
COMPLEXITY
  Time  O(n log k)  one counting pass, then one push/pop per distinct value on
                    a heap of k+1
  Space O(n)        the dictionary can hold up to n distinct values; the heap
                    holds k+1
WATCH OUT
  - The result comes out least frequent first ([2,1], not [1,2]). This is fine
    here, but reverse it if the interviewer asks for descending order.
  - The priority must be entry.Value (the count), not entry.Key.
  - If you push everything and never check Count > k, time becomes O(n log n).
  - If k is larger than the number of distinct values, Dequeue in step 3
    throws InvalidOperationException. The problem says this never happens.
================================================================================
*/
