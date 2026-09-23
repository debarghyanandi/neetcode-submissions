// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  Frequency buckets, highest-first traversal   [bucket-by-frequency]
// -  ranks above suboptimal.cs (O(n log k) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  Bucket array sized by frequency range avoids sorting; single pass
// -  through descending frequencies collects k values in linear time.
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
 PATTERN : Bucket Sort by Frequency - count, then scan buckets down
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  occurrences         occurrences[value] = how many times value appears in nums
  valuesByFrequency   valuesByFrequency[f] = list of every value seen exactly f times
  filled              how many slots of result are already written
  frequency           the bucket index being read, used as a real frequency count
WHY THIS PATTERN
  The problem asks for the k most frequent values, not for a full ordering of
  all values. A frequency can never exceed nums.Length, so frequencies are small
  integers in a known range - that is exactly the condition for bucket sort,
  where you use the value itself as an array index instead of comparing items.
  So occurrences gives each value its count, valuesByFrequency puts each value
  in the slot named by its count, and one downward scan from the last index
  reaches the top k without any comparison sort.
BRUTE FORCE
  The first thing most people write: build the same occurrences dictionary, copy
  it to a list, sort by count descending, take the first k keys. That is O(n log
  n) time because of the sort. A heap of size k is the usual middle step at O(n
  log k). Both lose here because sorting or heap-ordering the counts is wasted
  work when the counts are already bounded by nums.Length and can index an array
  directly.
INVARIANT
  When the downward loop is at index frequency, every value whose count is
  strictly greater than frequency has already been visited, and filled holds how
  many of them were copied into result. So the values written into result are
  always taken in non-increasing order of count. The loop stops the moment
  filled == k, which means result holds k values none of which has a smaller
  count than a value left behind.
WHY INDEX 0 IS SKIPPED
  The loop condition is frequency > 0, not frequency >= 0. Bucket 0 is allocated
  and always stays empty, because a value only lands in occurrences if it
  appeared at least once. Stopping at 1 costs nothing and makes it clear that a
  count of zero is not a real entry.
WATCH OUT
  The inner break only leaves the foreach; the outer for then re-checks filled <
  k and exits, so the guard works, but it is two exits, not one - do not move
  the break logic around carelessly. If k is larger than the number of distinct
  values in nums, the loop runs out of buckets and returns a result array with
  trailing zeros instead of throwing - the code assumes k is valid. Allocating
  nums.Length + 1 List objects up front means one empty List per bucket even
  when only a handful of distinct values exist; for a long nums with few
  distinct values that is a lot of dead allocation. If nums is empty,
  valuesByFrequency has length 1, the loop body never runs, and you get back an
  array of k zeros.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you cut the memory used by the empty buckets?
     Allocate the List lazily - leave the array entries null and create a List
     only in the second foreach when a value first lands in that bucket. The
     scan loop then needs a null check per index, trading a branch for far fewer
     allocations.
  2. What if nums does not fit in memory and arrives as a stream?
     Bucket sort needs the full count table, so switch to a min-heap of size k
     keyed on count, evicting the smallest. That is O(n log k) time but only
     O(k) extra space beyond the counts.
  3. What if ties at the boundary frequency must be broken by value, smallest
  first?
     Sort just the one bucket where the cut happens before copying from it. Only
     that bucket needs ordering, so the cost is O(m log m) for m values sharing
     that frequency, not a full sort.
  4. What if you must return the k least frequent values instead?
     Walk the same buckets upward from index 1 with the same filled guard;
     nothing else changes.
TRIGGER
  You need the top k by a count, and that count is bounded by the input length -
  index an array by the count instead of sorting.
C# NOTE
  TryGetValue(number, out int currentCount) sets currentCount to 0 when the key
  is absent, so the increment works for both first and later sightings with one
  lookup plus one write - cleaner than ContainsKey followed by a second lookup.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
