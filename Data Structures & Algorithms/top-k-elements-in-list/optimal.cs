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
 PATTERN : Bucket Sort by Frequency - count, then index by count
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  occurrences        occurrences[v] = how many times v appears in nums
  currentCount       count of number so far (0 if not seen yet)
  valuesByFrequency  valuesByFrequency[f] = all values that appear exactly f times
  filled             how many slots of result are used so far
WHY THIS PATTERN
  The problem asks for the k values with the highest counts. It does not ask for
  the other values in order. A count can never be larger than nums.Length, so
  the count itself can be used as an array index. Putting each value into
  valuesByFrequency[count] sorts the values by count without any comparisons.
  Walking the array from the top gives the most frequent values first.
BRUTE FORCE
  Build the same count map. Then sort the distinct values by count, largest
  first, and take the first k. This is correct, but the sort costs O(d log d),
  where d is the number of distinct values. Bucketing removes that log factor.
INVARIANT
  After step 2, every distinct value sits in exactly one bucket, the one equal
  to its true count. In step 3, frequency moves strictly downward. So when a
  value is written to result, every value with a higher count has already been
  written. The first k values written are therefore a correct top-k.
WATCH OUT
  If k is larger than the number of distinct values, the loop ends early. The
  unused slots of result stay 0, and 0 looks like a real answer. This happens
  silently, with no error. Also, when several values share the same count at the
  cut-off point, which ones get picked depends on Dictionary enumeration order.
  That order is not guaranteed, so do not rely on a specific tie result. Bucket
  0 is allocated but can never hold a value, because every value in occurrences
  appears at least once.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it without the n+1 buckets?
     Use a min-heap (a priority queue where the smallest item is removed first)
     of size k, keyed on count. Pop the smallest whenever the size goes above k.
     This is O(n log k) time and O(d + k) space. It is slower in theory, but it
     only needs k extra slots on top of the map.
  2. Can you get the top k in average linear time with no bucket array?
     Use quickselect on the distinct values, compared by count. It partitions
     around position d - k. Average time is O(d) after counting, but the worst
     case is O(d^2) unless the pivot choice is randomized.
  3. What if nums is a stream that is too large to keep a full count map in
  memory?
     Exact top-k is not possible in bounded memory. Use an approximate method
     such as Misra-Gries or Count-Min Sketch plus a small heap. The trade-off is
     less memory for counts that may be slightly wrong.
  4. What if ties must be broken in a fixed order, for example the smaller value
  first?
     Sort each bucket before you read it, or read the buckets through a
     SortedSet. This adds a log factor only inside the buckets you actually
     read.
TRIGGER
  You need a ranking by a count that cannot be larger than n, and you only need
  the top k, not a full sort.
C# NOTE
  TryGetValue followed by the indexer set does two hash lookups for each number.
  CollectionsMarshal.GetValueRefOrAddDefault(occurrences, number, out _)++ does
  the increment with a single lookup.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
