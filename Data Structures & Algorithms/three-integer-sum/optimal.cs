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
 PATTERN : Sort + Two Pointers - fix an anchor, two-sum the rest
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  target          -nums[i]; the pair sum needed so that anchor + pair = 0
  anchor          nums[i], the fixed first value of every triplet in this call
  sum             nums[left] + nums[right], compared against target
  usedLeftValue   nums[left] of the triplet just added; left skips all its copies
  usedRightValue  nums[right] of the triplet just added; right skips all its copies
WHY THIS PATTERN
  The problem asks for all unique triplets that sum to zero. Fix one value and
  it becomes two-sum on the rest, with target = -nums[i]. On a sorted array,
  two-sum is solved by two pointers moving inward. This gives one linear scan
  per anchor. Sorting also puts equal values next to each other, so duplicates
  can be skipped by comparing a value with its neighbour. No hash set is needed
  for that.
BRUTE FORCE
  Try every i < j < k, keep the triplets whose sum is 0, and store each sorted
  triplet in a HashSet to remove duplicates. It is clearly correct, but it costs
  O(n^3) time and O(number of triplets) extra space for the set. It loses
  because the sorted order lets one pointer pass replace the whole inner pair of
  loops.
INVARIANT
  Inside FindPairsWithSum, no pair that uses an index outside [left, right] can
  still give a new triplet. If sum > target, nums[right] is too big even with
  the smallest value left, so right can be dropped. If sum < target, nums[left]
  is too small even with the largest value left, so left can be dropped. Each
  anchor value is used once (the nums[i] == nums[i - 1] skip), and each (left,
  right) value pair is added once. So every unique triplet is found exactly
  once.
WATCH OUT
  Array.Sort(nums) sorts the caller's array in place. The caller's input order
  is lost. The comment "Both pointers must clear their whole duplicate block"
  says more than is needed. Once left moves past usedLeftValue, the pair needs
  nums[right] = target - nums[left], which is a different value. So skipping on
  one side alone already stops the repeat. The second loop only saves some
  useless steps. The comments also ignore overflow: -nums[i] overflows when
  nums[i] is int.MinValue, and nums[left] + nums[right] can overflow for very
  large values. Nothing in the code guards against either one.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you solve 4Sum, or kSum in general?
     Add one anchor loop per extra element, with the same duplicate skip at each
     level, and end with this same two-pointer helper. Time is O(n^(k-1)).
     Recursion keeps the code short.
  2. Can you solve it without sorting or changing the input?
     For each i, run a hash-set two-sum over the rest of the array. Time is
     still O(n^2), but you use O(n) extra space, and you must remove duplicates
     yourself by storing sorted triplets in a set. Or you can sort a copy, which
     costs O(n) space.
  3. What if you must count triplets with sum < target (3Sum Smaller)?
     Use the same sort and pointers. When sum < target, every index from left+1
     to right pairs with left, so add right - left and move left. Do not skip
     duplicates here, because index triplets are counted, not value triplets.
  4. What about 3Sum Closest?
     Use the same loop, but track the sum with the smallest distance to the
     target instead of collecting exact matches. There is no duplicate skipping
     and no early break, and you can return at once if the distance is 0.
TRIGGER
  The problem asks for all unique groups of k numbers with a given sum, and
  sorting the input is allowed.
C# NOTE
  FindPairsWithSum appends straight into the shared triplets list passed in as a
  parameter. It does not build and return a new list for each anchor, so no
  extra lists are created or merged.
COMPLEXITY
  Time  : O(n^2)
  Space : O(log n)
================================================================================
*/
