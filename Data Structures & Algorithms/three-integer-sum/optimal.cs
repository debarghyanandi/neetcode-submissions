// ##########################################################################
// #  optimal.cs            O(n^2) time / O(log n) space
// #  Sorting with two-pointer scan   [sort-two-pointer]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Sorting O(n log n); nested iteration with two-pointer traversal across
// #  all anchors is O(n²).
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
 PATTERN : Sort + Two Pointers - fix anchor, two-sum the suffix
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  triplets        the answer list; filled in place by the helper, never merged
  anchor          nums[i], the fixed smallest member of the triplet being built
  target          -nums[i]; the sum the pair must hit so the triplet is zero
  left, right     the shrinking window [left, right] inside nums, always right of i
  sum             nums[left] + nums[right] for the current window
  usedLeftValue   the nums[left] value just consumed, kept so the skip loop knows what to clear
  usedRightValue  same idea for nums[right]
WHY THIS PATTERN
  The problem asks for all unique triplets summing to zero, so order of the
  input does not matter and we are free to sort. Once sorted, fixing one element
  nums[i] turns the rest into a two-sum on a sorted subarray, which a left/right
  pointer walk solves in one pass instead of a nested loop. Sorting also puts
  equal values next to each other, which is the only reason the cheap duplicate
  checks (nums[i] == nums[i-1], and the usedLeftValue / usedRightValue loops)
  are enough to make the output unique without a hash set.
BRUTE FORCE
  Three nested loops over i < j < k, test nums[i]+nums[j]+nums[k] == 0, and push
  each hit into a HashSet of sorted triples to kill duplicates. That is O(n^3)
  time plus the cost of hashing every found triple. It loses because the
  innermost loop re-scans the whole tail for every pair, while the sorted
  two-pointer scan gets the same information in one linear sweep per anchor.
INVARIANT
  When the helper is called for anchor index i, every triplet whose smallest
  element sits at index i or earlier has already been emitted, and the window
  [i+1, nums.Length-1] holds only candidates larger than or equal to nums[i].
  Inside the loop, all pairs summing to target that lie outside [left, right]
  have been ruled out: if sum > target the true partner of nums[right] must be
  smaller, so right-- discards nothing valid, and symmetrically for left++.
  Because each triplet has exactly one smallest element, and each anchor value
  is used only once, every valid triplet is produced exactly once.
SKIP BACKWARD, NOT FORWARD
  The anchor dedup compares nums[i] to nums[i-1], not to nums[i+1]. That keeps
  the FIRST copy of a repeated value and drops later copies. This matters for
  inputs like [-2,-2,0,2]: the first -2 still gets a full window containing the
  second -2, so a triplet that legitimately uses two equal values is not lost.
  Skipping forward instead would shrink the window and miss those.
WATCH OUT
  Array.Sort(nums) rewrites the caller's array in place; if the caller needed
  the original order it is gone. The return type is List<List<int>> while
  LeetCode's signature is IList<IList<int>>, so this will not compile against
  the stub as pasted. sum is an int add: with values near int.MaxValue or
  int.MinValue it silently overflows, and -nums[i] overflows for int.MinValue -
  fine for this problem's typical values, but say so out loud if asked. The
  helper takes both target and anchor even though target is just -anchor, which
  is harmless but invites the two arguments to drift apart if someone edits one
  call site.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it without sorting?
     Yes - for each anchor i, walk j > i and keep a HashSet of values seen,
     looking for -(nums[i]+nums[j]). Still O(n^2) time but O(n) extra space, and
     you now need a HashSet of normalized triples to dedup, which sorting gave
     you for free.
  2. Generalize to k-sum.
     Recurse: peel off one anchor at a time down to k == 2, then run this same
     two-pointer base case. Time becomes O(n^(k-1)), and the anchor-duplicate
     skip has to be repeated at every recursion level.
  3. Only the COUNT of triplets is needed, not the triplets themselves.
     Drop triplets entirely and add the size of each duplicate block product
     instead of allocating a list - same scan, O(1) extra space beyond the sort,
     and no per-hit allocation.
  4. 3Sum Closest instead of exactly zero.
     Keep the same sort and scan, but never break early on nums[i] > 0 and never
     skip on equality of sum; track the best absolute difference and move the
     pointer that reduces the gap. The duplicate-skip machinery disappears
     because the answer is a single number.
TRIGGER
  Reach for this when a problem asks for all unique combinations that hit a
  fixed sum and the input order is irrelevant - sorting buys you both the
  monotonic pointer move and adjacent duplicates.
C# NOTE
  Each hit allocates a fresh List<int> with three items via a collection
  initializer, which grows its internal array as it fills; new List<int>(3) {
  ... } or an int[] wrapper avoids that resize. Array.Sort on int[] uses the
  unmanaged-free introsort path and is the source of the O(log n) stack space in
  the complexity line.
COMPLEXITY
  Time  : O(n^2)
  Space : O(log n)
================================================================================
*/
