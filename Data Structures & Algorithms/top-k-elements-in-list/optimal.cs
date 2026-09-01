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
 PATTERN : Bucket sort by frequency - count, bucket, walk down
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
WHY BUCKETS, NOT A HEAP
  The whole trick is a bound: a value in nums can occur at most nums.Length
  times, so a frequency is a small non-negative integer, not an arbitrary sort
  key. That lets frequency be used as an ARRAY INDEX instead of something to
  compare. Once valuesByFrequency is indexed by count, the values are already
  grouped in count order for free, and the ordering step disappears - no sort of
  occurrences by value, no size-k min-heap over the distinct counts. If you can
  only recall one line weeks later, recall that the key you want to order by is
  bounded by the input length.
INVARIANT
  After the second foreach: for every f, valuesByFrequency[f] holds exactly
  those values that occur exactly f times, and every key of occurrences lands in
  exactly one bucket. The buckets are a partition of the distinct values.
  Because the walk in Step 3 goes from index nums.Length downward, values are
  emitted in non-increasing frequency order, so any prefix of that emission - in
  particular the first k written into result - is a valid top-k answer.
THE COUNTING IDIOM
  occurrences.TryGetValue(number, out int currentCount) sets currentCount to 0
  on a miss (default for int), so currentCount + 1 is 1 for a first sighting and
  the following indexer assignment inserts it. This is the reason the loop needs
  no ContainsKey branch. Note what you cannot write instead:
  occurrences[number]++ throws KeyNotFoundException on the first sighting,
  because the read side of the compound assignment hits the missing key. The
  idiom here does a lookup plus a store per element;
  CollectionsMarshal.GetValueRefOrAddDefault would fold those into one, at the
  cost of readability.
THE DESCENDING WALK
  The for starts at valuesByFrequency.Length - 1, which is nums.Length - the
  largest count any value could have. It stops at frequency > 0 rather than >= 0
  because bucket 0 is provably empty: every key in occurrences was seen at least
  once, so nothing ever gets added at index 0. The nested for/foreach looks
  quadratic but is not: the buckets partition the distinct values, so across the
  entire outer loop each value is visited at most once.
THE BREAK GUARD
  More than k values can tie at the same top frequency, while result has room
  for exactly k. Two separate conditions cooperate here and BOTH are
  load-bearing:
  1. if (filled == k) break exits the inner foreach mid-bucket, before
  result[filled] is written out of range.
  2. filled < k in the outer for header stops the descent entirely.
  Drop the outer condition and keep the break: the loop moves to the next lower
  non-empty bucket and the foreach writes result[k] immediately -
  IndexOutOfRangeException. Drop the break and keep the outer condition: the
  current bucket keeps writing past the end - same exception. The break only
  leaves one foreach, it does not leave the for.
WATCH OUT
  The initialization loop fills all nums.Length + 1 slots with real List
  instances before a single value is inserted. For nums = [1,1,1,1,1] that is
  six Lists allocated so that one of them can hold a single value; the empties
  are pure waste. The payoff is that Step 2 and Step 3 need no null checks
  anywhere. Lazy allocation (create the List on first Add, null-check on read)
  is the trade if the eager fill is ever challenged.

  Second trap: result is sized k unconditionally. If k exceeded the number of
  distinct values, the walk would run out of buckets and the tail of result
  would silently stay 0 rather than throw. The code relies on the problem
  constraint that k is at most the distinct count.
INTERVIEW FOLLOW-UPS
  Ties: the order of values inside one bucket is Dictionary enumeration order,
  which is unspecified. This solution therefore breaks ties arbitrarily. If
  asked for a deterministic tie-break (smallest value first, say), you must sort
  each bucket as you consume it - the bucket structure itself gives you no
  ordering within a frequency.

  When buckets lose: they need the nums.Length bound known up front and
  materialized as an array. For a stream, or when the count range is unbounded
  or enormous relative to the number of distinct values, go back to a size-k
  min-heap over occurrences.

  Why no comparison sort is needed at all: the values being ordered are the
  counts, and counts are dense small integers in [1, nums.Length] - the same
  reason counting sort escapes the comparison lower bound.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
