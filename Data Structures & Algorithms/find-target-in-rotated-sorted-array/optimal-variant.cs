// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(log n) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public int Search(int[] nums, int target)
    {
        int l = 0, r = nums.Length - 1;

        while (l < r)
        {
            int m = (l + r) / 2;
            if (nums[m] > nums[r])
            {
                l = m + 1;
            }
            else
            {
                r = m;
            }
        }

        int pivot = l;

        int result = BinarySearch(nums, target, 0, pivot - 1);
        if (result != -1)
        {
            return result;
        }

        return BinarySearch(nums, target, pivot, nums.Length - 1);
    }

    public int BinarySearch(int[] nums, int target, int left, int right)
    {
        while (left <= right)
        {
            int mid = (left + right) / 2;
            if (nums[mid] == target)
            {
                return mid;
            }
            else if (nums[mid] < target)
            {
                left = mid + 1;
            }
            else
            {
                right = mid - 1;
            }
        }
        return -1;
    }
}

/*
================================================================================
 PROBLEM : A sorted array of distinct ints was rotated at an unknown index,
           and target is an int. Return the index of target in nums, or -1 if
           absent. Example: nums = [3,4,5,6,1,2], target = 1 -> 4.
 PATTERN : Binary Search (find pivot, then search one half)
================================================================================
IDEA
  Unlike the one-pass optimal.cs, this runs binary search in two phases.
  Phase 1 finds pivot, the index of the minimum. If nums[m] > nums[r], the
  drop is right of m, so l = m + 1. Otherwise the min is at m or left, so
  r = m. Phase 2 does a normal BinarySearch on [0, pivot-1], then on
  [pivot, n-1]. Each part is sorted, so plain binary search is correct.
EXAMPLE
  nums = [3,4,5,6,1,2], target = 1
  pivot: (l,r,m) (0,5,2) 5>2 l=3; (3,5,4) 1<=2 r=4; (3,4,3) 6>1 l=4
  BinarySearch(0,3): mid 1 -> 4>1, mid 0 -> 3>1, returns -1
  BinarySearch(4,5): mid 4 -> nums[4]=1 hit, answer 4
COMPLEXITY
  Time  O(log n)  pivot loop halves l..r, then at most two halving searches
  Space O(1)      only a few int indices, no recursion or copies
WATCH OUT
  - Compare nums[m] with nums[r], not nums[l]. With nums[l] an unrotated
    array like [1,2,3] sends l the wrong way.
  - Pivot loop must be l < r with r = m (not m - 1), or it skips the min.
  - Duplicates break it: [1,1,1,0,1] gives nums[m] == nums[r] too often.
  - Two searches still run when not rotated; pivot = 0 makes range 0..-1
    empty. Compare target with nums[^1] to search only one half.
================================================================================
*/
