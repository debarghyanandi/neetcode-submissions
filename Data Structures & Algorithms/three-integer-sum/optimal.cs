// ##########################################################################
// #  optimal.cs            O(n^2) time / O(log n) space
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
 PROBLEM : Given an int array nums, return every unique triplet [a, b, c] with
           a + b + c == 0. The three values must come from different indices.
           The answer must not contain the same triplet twice. Order does not
           matter. [-1,0,1,2,-1,-4] -> [[-1,-1,2],[-1,0,1]]
 PATTERN : Sort + Two Pointers (converging) per anchor
================================================================================
IDEA
  Sort nums. Then fix each nums[i] as the anchor and solve a two-sum for
  target -nums[i] on the window [i+1, end] with left and right pointers.
  If sum is too big, right-- makes it smaller; if too small, left++. Sorted
  order means each move throws away only pairs that cannot match, so no
  triplet is missed. Skipping equal anchors and equal left/right values
  keeps every triplet unique.
EXAMPLE
  sorted [-4,-1,-1,0,1,2]; i=0 anchor -4, target 4: no pair reaches 4
  i=1 anchor -1, target 1: -1+2 hit -> [-1,-1,2]; then 0+1 hit -> [-1,0,1]
  i=2 skipped (same -1), i=3 anchor 0: 1+2 > 0, pointers meet; i=4 is 1 > 0
  Answer: [[-1,-1,2],[-1,0,1]]
COMPLEXITY
  Time  O(n^2)    n anchors, each with an O(n) two-pointer scan; sort is only
                  n log n
  Space O(log n)  only the sort's recursion stack; output list not counted
PATH TO OPTIMAL
  Brute force, three nested loops + set of sorted triplets - O(n^3).
  Anchor + hash set for the third value - O(n^2) time, O(n) space; the
    inner loop becomes one pass, but dedup is messy and needs extra memory.
  Sort + two pointers (this file) - O(n^2) time, O(log n) space; sorting
    gives adjacent duplicates, so dedup is free and no hash set is needed.
KEYWORDS
  3Sum, two pointers, sorting, two-sum II, deduplication, k-sum, triplets
WATCH OUT
  - Break on nums[i] > 0, not >= 0: with [0,0,0] the anchor 0 is valid.
  - Skip anchors with nums[i] == nums[i-1], not nums[i] == nums[i+1];
    the second form wrongly drops [-1,-1,2].
  - After a hit, move BOTH pointers past their duplicate blocks. Otherwise
    the loop never ends or the same triplet is added again.
  - Array.Sort changes the caller's array. Copy it first if that matters.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. 3Sum Closest: return the sum nearest to a target?
     -> Same loops. Track the best |sum - target| and move pointers the same
        way. No dedup needed. Still O(n^2) time.
  2. 4Sum or general k-Sum?
     -> Recurse. Fix one value and call (k-1)-Sum until you reach this
        two-pointer base case. Time is O(n^(k-1)). Use long for sums.
  3. You may not sort or change the input?
     -> For each i, run a two-sum with a hash set on the rest. Store sorted
        triplets in a set to dedup. O(n^2) time, but O(n) extra space.
  4. Count triplets with sum < target (3Sum Smaller)?
     -> Sort. When nums[i]+nums[left]+nums[right] < target, add right-left to
        the count and left++. Otherwise right--. O(n^2) time.
TRIGGER
  You must find unique pairs or triplets hitting a target sum and the
  order of the input does not matter, so you can sort first.
================================================================================
*/
