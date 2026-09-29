// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// --------------------------------------------------------------------------

public class Solution
{
    public int[] TopKFrequent(int[] nums, int k)
    {
        // Step 1: value -> how many times it occurs.
        var occurrences = new Dictionary<int, int>();

        foreach (int number in nums)
        {
            occurrences.TryGetValue(number, out int currentCount);
            occurrences[number] = currentCount + 1;
        }

        // Step 2: buckets INDEXED BY FREQUENCY.
        // valuesByFrequency[f] = every value that occurs exactly f times.
        // A value can occur at most nums.Length times, so that many buckets
        // is enough - this bound is what makes the sort unnecessary.
        List<int>[] valuesByFrequency = new List<int>[nums.Length + 1];

        for (int frequency = 0; frequency < valuesByFrequency.Length; frequency++)
        {
            valuesByFrequency[frequency] = new List<int>();
        }

        foreach (var entry in occurrences)
        {
            valuesByFrequency[entry.Value].Add(entry.Key);
        }

        // Step 3: walk buckets from the highest frequency down, taking k values.
        int[] result = new int[k];
        int filled = 0;

        for (int frequency = valuesByFrequency.Length - 1; frequency > 0 && filled < k; frequency--)
        {
            foreach (int value in valuesByFrequency[frequency])
            {
                result[filled] = value;
                filled++;

                // More than k values can share the top frequency, and result
                // only has room for k. Without this guard: IndexOutOfRange.
                if (filled == k)
                    break;
            }
        }

        return result;
    }
}

/*
================================================================================
 PROBLEM : Given an int array nums and an int k, return the k values that
           appear most often. The answer may be in any order. The problem
           promises the answer is unique. Example: nums=[1,1,1,2,2,3], k=2 ->
           [1,2].
 PATTERN : Bucket Sort by frequency (hash map counting)
================================================================================
IDEA
  First, count each value in occurrences (value -> count). No count can be
  bigger than nums.Length. So valuesByFrequency[f] holds every value that
  occurs exactly f times. Walk f from high to low and copy values into
  result until filled == k. This is correct because bucket order is
  frequency order, so no sort is needed.
EXAMPLE
  nums=[1,1,1,2,2,3,3], k=2 -> occurrences {1:3, 2:2, 3:2}
  buckets: [3]=[1], [2]=[2,3], [1]=[], f=7..4 empty
  f=3: take 1 (filled=1); f=2: take 2 (filled=2), break before taking 3
  answer [1,2]. Value 3 ties with 2 and is dropped by the filled==k guard.
COMPLEXITY
  Time  O(n)  one pass to count, one pass over n+1 buckets and distinct values
  Space O(n)  the map plus n+1 bucket lists hold at most n entries in total
PATH TO OPTIMAL
  Sort (value,count) pairs by count - O(n log n) - simple baseline.
  Min-heap of size k over the counts - O(n log k) - in suboptimal.cs.
  It is better when k is small.
  Buckets indexed by count - O(n) - count is bounded by n, so no compare.
KEYWORDS
  top k frequent, hash map, frequency count, bucket sort, min-heap,
  quickselect
WATCH OUT
  - Without the filled == k check in the inner loop, a tie in the last
    bucket writes past result: IndexOutOfRange.
  - That break only leaves the foreach. The outer loop condition
    filled < k is what stops it. Keep both checks.
  - The array needs nums.Length + 1 buckets, not nums.Length. A value that
    fills all of nums has count n. Bucket 0 is never used.
  - If k is more than the number of distinct values, result ends in 0s.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it without O(n) buckets, or with less extra memory?
     -> Use quickselect on the distinct values by count. It is O(n) on average
        and O(n^2) in the worst case. It works in place on the key list.
  2. What if nums is a stream that never ends?
     -> Keep a count map and a min-heap of size k. Each update is O(log k).
        For very large streams, use Count-Min Sketch, which is approximate.
  3. What if ties must be broken, for example by smaller value first?
     -> Sort each bucket, or use a heap with a (count, value) comparator. This
        adds a log factor for that ordering.
  4. Why is the bucket version O(n) when sorting is n log n?
     -> The keys are counts in 1..n, a small integer range. Indexing by the
        key replaces comparisons, like counting sort.
TRIGGER
  When you need the "top k by count" and the counts are bounded by n,
  index buckets by count instead of sorting.
================================================================================
*/
