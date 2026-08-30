// ##########################################################################
// #  YOU SOLVED THIS YOURSELF  (submission-0, then submission-2 - both marked)
// #  merged with submission-3 - your helper-method form is the one kept
// ##########################################################################

public class Solution
{
    public List<List<int>> ThreeSum(int[] nums)
    {
        var triplets = new List<List<int>>();

        // Sorting enables BOTH halves of this solution:
        //   - the two-pointer scan needs monotonic order
        //   - duplicate skipping needs equal values to sit adjacent
        Array.Sort(nums);

        for (int i = 0; i < nums.Length; i++)
        {
            // Sorted ascending: once the anchor is positive, the two larger
            // values behind it are positive too, so no sum can reach zero.
            if (nums[i] > 0)
                break;

            // Skip a repeated ANCHOR - it would regenerate identical triplets.
            // i > 0 guard: index 0 has no predecessor to compare against.
            if (i > 0 && nums[i] == nums[i - 1])
                continue;

            FindPairsWithSum(nums, i + 1, nums.Length - 1, -nums[i], nums[i], triplets);
        }

        return triplets;
    }

    // Classic sorted two-pointer two-sum, restricted to the window [left, right],
    // appending every matching triplet directly into `triplets`.
    private void FindPairsWithSum(int[] nums, int left, int right, int target, int anchor, List<List<int>> triplets)
    {
        while (left < right)
        {
            int sum = nums[left] + nums[right];

            if (sum == target)
            {
                triplets.Add(new List<int> { anchor, nums[left], nums[right] });

                // Both pointers must clear their whole duplicate block, or the
                // very next iteration re-emits the same triplet.
                int usedLeftValue = nums[left];
                int usedRightValue = nums[right];

                while (left < right && nums[left] == usedLeftValue)
                    left++;

                while (left < right && nums[right] == usedRightValue)
                    right--;
            }
            else if (sum > target)
            {
                right--;
            }
            else
            {
                left++;
            }
        }
    }
}

/*
================================================================================
 PATTERN : Sort + Fix One + Two Pointers (k-sum reduction)
 SOURCE  : YOUR OWN SOLUTION (submission-0 '//My Solution.' and submission-2
           '//Same My solution but used for loop.'), merged with submission-3
           - the array-slicing helper and the manual index counter are gone;
           your second attempt's helper-method form is kept, it is the
           clearest of the three
 STATUS  : Optimal
================================================================================

WHY THIS PATTERN
  This is the general k-sum reduction: FIX ONE ELEMENT and the problem drops
  to (k-1)-sum on the remaining suffix. 3-sum becomes n separate 2-sums.
  Since the array is sorted, each 2-sum is the O(n) two-pointer scan from
  two-integer-sum-ii - which is exactly why solving that one first matters.
  4-sum is this same trick applied twice.

BRUTE FORCE (and why it fails)
  Three nested loops: O(n^3), plus a set to dedupe the results. At n = 3000
  that is 2.7 * 10^10 iterations.

WHY SORTING IS NOT A COST HERE
  O(n log n) sorting is dominated by the O(n^2) scan, so it is free in Big-O
  terms - and it buys two things at once: the two-pointer steering rule, and
  adjacency of equal values, which is what makes deduping O(1) instead of
  requiring a HashSet of triplets. Note the trade: sorting destroys the
  original indices. This problem asks for VALUES, so that is acceptable;
  two-integer-sum asks for indices, which is why it uses a hash map instead.

THE THREE PRUNES, AND WHAT EACH IS FOR
  1. `nums[i] > 0` -> break.  Speed only. Everything after is positive too.
  2. `nums[i] == nums[i-1]` -> continue.  CORRECTNESS. Without it,
     [-1,-1,0,1] emits [-1,0,1] twice.
  3. Skipping duplicate left/right values after a hit.  CORRECTNESS. Without
     it, [-2,0,0,2,2] emits [-2,0,2] more than once.
  Prune 1 is optional; 2 and 3 are not. Know which is which.

INVARIANT
  When the anchor is nums[i], every valid pair completing it lies in
  [left, right], and triplets already contains no duplicates.

ALGORITHM (NeetCode: "Two Pointers")
  1. Sort ascending.
  2. For each index i as the anchor:
       - break once nums[i] > 0
       - skip if nums[i] repeats the previous anchor
       - two-pointer scan of (i+1 .. n-1) for a pair summing to -nums[i]
       - on a hit, record the triplet and advance both pointers past their
         duplicate blocks
  3. Return all triplets.

COMPLEXITY
  Time  : O(n^2) - n anchors, each driving an O(n) two-pointer scan.
          The O(n log n) sort is dominated.
  Space : O(1) auxiliary if the output does not count (Array.Sort is in place).
          O(number of triplets) for the result itself.

TRIGGER
  "Find all k-tuples summing to a target, no duplicate tuples, values not
  indices." The moment DUPLICATE TUPLES MUST BE EXCLUDED and the input is
  unsorted, reach for sort + fix + two pointers rather than hashing.

C# NOTES
  - Array.Sort(int[]) is introsort, in place, O(n log n), no allocation.
  - The earlier attempt used `nums[(i + 1)..]` - range syntax on an ARRAY
    ALLOCATES A COPY every iteration, turning O(1) space into O(n) per call.
    `nums.AsSpan(i + 1)` would have been the zero-copy equivalent. This is
    the single most useful C# performance fact in this file: ranges over
    arrays copy, ranges over Span<T> do not.
  - Passing `triplets` into the helper and appending in place avoids building
    and merging intermediate lists.

WATCH OUT
  - The dedup while-loops need `left < right` in their own condition, not
    just the outer loop's - otherwise a run of equal values walks past.
  - `i > 0 &&` must come first in the anchor-dedup check; without it index 0
    reads nums[-1].
  - The earlier attempt incremented `i` by hand inside a foreach over the
    same array. It happened to work, but a manual counter shadowing the
    iteration variable is how silent off-by-ones get in. The for-loop version
    has one source of truth for the index.
================================================================================
*/
