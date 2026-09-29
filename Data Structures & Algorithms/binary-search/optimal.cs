// ##########################################################################
// #  optimal.cs            O(log n) time / O(log n) space
// #  Binary search, recursive   [binary-search-recursive]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Each recursive call halves the search space; the call stack depth
// #  reaches logarithmic maximum.
// ##########################################################################

public class Solution
{
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
 PATTERN : Binary Search - recursive halving of a sorted range
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-1.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  left          first index of the range still being searched
  right         last index of the range still being searched (inclusive)
  mid           middle index of [left, right], the one element compared each call
  searchTarget  the target value, passed down unchanged through every call
WHY THIS PATTERN
  The problem gives a sorted array and asks for the index of one value. Because
  the array is sorted, one comparison with nums[mid] tells you which half cannot
  hold the target. You can throw that half away. Each call then searches only
  [left, mid - 1] or [mid + 1, right].
BRUTE FORCE
  A linear scan checks nums[0], nums[1], and so on, and returns the first index
  that equals target. It takes O(n) time and O(1) space. It is correct, but it
  ignores the sorted order. It looks at every element, while binary search looks
  at only about log2(n) of them.
INVARIANT
  If the target is anywhere in nums, it is inside [left, right]. At the start
  this is true, because the range is 0 to nums.Length - 1. If searchTarget <
  nums[mid], then every index from mid up holds a value that is too large, so
  the recursion keeps [left, mid - 1]. In the other branch every index from mid
  down is too small, so it keeps [mid + 1, right]. The range gets smaller on
  every call. So either the code finds nums[mid] == searchTarget, or the range
  becomes empty (left > right) and returning -1 is correct.
SAFE MIDPOINT
  The code computes mid = left + (right - left) / 2, not (left + right) / 2.
  When left and right are both large, left + right can go past int.MaxValue.
  Then it overflows to a negative number and gives a bad index. The subtraction
  form never goes above right, so it cannot overflow.
WATCH OUT
  The code only works if nums is sorted in ascending order. On unsorted input it
  can return -1 even when the target is in the array. If the value appears more
  than once, the code returns whatever matching index it hits first, which is
  not always the first or the last copy. An empty array works: right becomes -1,
  so left > right on the first call and the answer is -1. The private helper has
  the same name, Search, as the public method. It is an overload (same name,
  different parameters). This is legal, but it is easy to mix up the two when
  you read the code.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it without recursion?
     Yes. Use a while (left <= right) loop that moves left or right instead of
     calling itself. The logic stays the same, and extra space drops from the
     call stack to O(1).
  2. What if there are duplicates and you need the first index of target?
     Search for the lower bound instead. When nums[mid] >= target, set right =
     mid and keep going. Do not return early. Stop when left == right, then
     check whether nums[left] == target.
  3. What if the sorted array was rotated at some unknown pivot?
     At each mid, one half, [left, mid] or [mid, right], is still sorted. Check
     whether the target falls inside that sorted half's range. If it does,
     search that half. If not, search the other half. It is still O(log n).
  4. What if you do not know the array's length, for example a stream or a
  reader interface?
     Start with right = 1 and double it until the value there is at least
     target, or until you go out of bounds. Then binary search inside [right /
     2, right]. This costs O(log p), where p is the target's position.
TRIGGER
  The input is sorted (or you can check something that only goes one way, like
  true for small values and false for large ones), and you need to find a value
  or a boundary faster than a linear scan.
C# NOTE
  Array.BinarySearch(nums, target) does the same job in one line. When the value
  is not found, it returns a negative number, not -1: the bitwise complement (~)
  of the index where the value would be inserted. So map any negative result to
  -1 if the problem asks for -1.
COMPLEXITY
  Time  : O(log n)
  Space : O(log n)
================================================================================
*/
