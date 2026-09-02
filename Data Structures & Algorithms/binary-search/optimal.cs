// ##########################################################################
// #  optimal.cs            O(log n) time / O(log n) space
// #  recursive binary search   [binary-search-recursive]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF (from submission-1)
// #
// #  Halves the search range each call; recursion stack depth is O(log n)
// #  since it's not tail-call optimized in C#.
// ##########################################################################

public class Solution {
    public int Search(int[] nums, int target)
    {
        // My Solution
        return Search(0, nums.Length - 1, target, nums);
    }

    private int Search(int left, int right, int searchTarget, int[] nums)
    {
        if (left > right)
            return -1;

        int mid = left + (right - left) / 2;

        if (nums[mid] == searchTarget)
            return mid;

        if (searchTarget < nums[mid])
            return Search(left, mid - 1, searchTarget, nums);

        return Search(mid + 1, right, searchTarget, nums);
    }
}

/*
================================================================================
 PATTERN : Binary Search - recursive halving of a closed range
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-1.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  nums is sorted ascending, so a single comparison against nums[mid] is not just
  an equality test - it classifies the whole array. If searchTarget < nums[mid],
  every index from mid onward holds a value >= nums[mid] > searchTarget, so the
  entire right half is eliminated without being read. That
  one-comparison-kills-half property is what sortedness buys, and it is the only
  assumption the code makes about the input.
INVARIANT
  At every call, the claim is: if searchTarget exists in nums at all, its index
  lies inside the CLOSED range [left, right]. The top-level call Search(0,
  nums.Length - 1, ...) establishes it trivially. Each recursive step preserves
  it by the argument above - the discarded half provably cannot contain
  searchTarget. So when the invariant range goes empty (left > right), the value
  is not in the array anywhere, and returning -1 is sound rather than a guess.
TERMINATION
  mid = left + (right - left) / 2 always lands in [left, right] when left <=
  right. Both recursive calls EXCLUDE mid: one passes mid - 1 as the new right,
  the other passes mid + 1 as the new left. So the range width right - left + 1
  strictly decreases on every call - it can never stall on the same window. That
  is the argument that this cannot infinite-recurse, and it is exactly the
  argument that breaks if you ever write Search(left, mid, ...) or Search(mid,
  right, ...) with the closed-range convention.
THE MID TRAP
  left + (right - left) / 2 is not stylistic noise; it is the fix for (left +
  right) / 2, which overflows int once left + right exceeds int.MaxValue. Since
  right - left is bounded by the array length, the subtraction form cannot
  overflow for any array you can actually allocate. Interviewers ask about this
  line specifically. Note it also biases mid toward left on even widths, which
  is why the left branch gets mid - 1 and never revisits mid.
EDGE CASES
  Empty array: nums.Length - 1 is -1, so the first call is Search(0, -1, ...),
  the left > right guard fires immediately, -1 comes back with zero array
  accesses. No length check is needed because the base case already covers it.
  Single element: left == right == mid == 0, so it either matches or one branch
  recurses into an empty range. Target smaller than nums[0] or larger than the
  last element walks down to an empty range rather than indexing out of bounds,
  because mid is always clamped inside [left, right].
DUPLICATES CAVEAT
  The equality check returns mid immediately, so with repeated values this
  returns SOME matching index, not the first or last one. LeetCode 704
  guarantees distinct values, so it is correct here - but do not reuse this
  shape for lower-bound / first-occurrence problems. There you must not return
  on equality: on nums[mid] == target you record mid as a candidate and keep
  searching left with right = mid - 1, letting the loop run to exhaustion.
FOLLOW-UP
  The obvious ask is to remove the recursion. Every recursive call here is in
  tail position and the only things that change are left and right, so it
  converts mechanically: while (left <= right) { compute mid; return on match;
  else reassign left = mid + 1 or right = mid - 1; } and return -1 after the
  loop. Same comparisons in the same order, but no stack frames - that is the
  one concrete thing the iterative form wins. The second ask is usually
  rotated-sorted-array search, where nums[mid] alone no longer classifies both
  halves and you must first decide which half is sorted.
COMPLEXITY
  Time  : O(log n)
  Space : O(log n)
================================================================================
*/
