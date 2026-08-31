// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  bucket sort by frequency   [bucket-sort-frequency]
// -  ranks above suboptimal.cs (O(n log k) time / O(n) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  counts frequencies into a dictionary, then places values into
// -  frequency-indexed buckets (bounded by n) and scans buckets from high
// -  to low, avoiding any comparison sort.
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
 PATTERN : Bucket Sort by Frequency (counting sort on a bounded key)
 SOURCE  : NeetCode / other resource (submission-2)
 STATUS  : Optimal
================================================================================

WHY THIS PATTERN
  Comparison sorting is bounded below by O(n log n) - but only for COMPARISON
  sorting. When the sort key is an integer from a small known range, you can
  place each item directly at its index and skip comparisons entirely.
  Here the key is a frequency, and a frequency cannot exceed n. Bounded key
  -> bucket sort -> O(n).

BRUTE FORCE (and why it fails)
  Count then sort by frequency: O(n log n).
  Count then bounded min-heap: O(n log k)  - see suboptimal.cs.
  Count then bucket by frequency: O(n)     - this file.

THE KEY INSIGHT
  Invert the mapping. The dictionary answers "given a value, what frequency?"
  The bucket array answers "given a frequency, which values?" - and because
  frequency is a small dense integer, that inverse map is just an array.
  Recognising when a lookup key is bounded is the whole skill here.

INVARIANT
  valuesByFrequency[f] contains exactly the distinct values occurring f times.
  Scanning f downward therefore visits values in descending frequency order
  without ever sorting.

ALGORITHM (NeetCode: "Bucket Sort")
  1. Count frequencies into a dictionary.                     O(n)
  2. Allocate n + 1 buckets, one per possible frequency.      O(n)
  3. Drop each value into the bucket for its frequency.       O(d)
  4. Walk buckets from n down to 1, collecting until k found. O(n)

COMPLEXITY
  Time  : O(n) - every step is a linear pass, no sorting anywhere.
  Space : O(n) - the dictionary plus n + 1 bucket lists.

  Note this trades MEMORY for speed against the heap version: the heap holds
  O(k), this holds O(n). At "top 10 of 10 million" that difference matters
  and the heap is the better engineering choice despite the worse Big-O.
  Say that out loud in an interview - it is the answer they are listening for.

TRIGGER
  "Top k by frequency" or any ranking where the SORT KEY IS A BOUNDED SMALL
  INTEGER (frequency, age, score out of 100, day of year). The tell is a key
  whose range is O(n) or smaller.

C# NOTES
  - List<int>[] is an array OF LISTS; each element starts null and must be
    initialised, unlike int[] which the CLR zero-fills.
  - Index 0 is deliberately skipped in the scan (`frequency > 0`) - a value
    with frequency 0 is not present at all.
  - Iterating a Dictionary yields KeyValuePair<int,int>; `var` keeps it
    readable. Enumeration order is unspecified - never rely on it.

WATCH OUT
  - The `if (filled == k) break;` guard is REQUIRED, not defensive padding.
    nums = [1,2,3], k = 2 puts three values in bucket 1; without the break
    the third write throws IndexOutOfRangeException.
  - The break exits only the inner foreach; the outer loop's `filled < k`
    condition then stops the scan.
================================================================================
*/
