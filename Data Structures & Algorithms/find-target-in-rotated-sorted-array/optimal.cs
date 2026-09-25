// ##########################################################################
// #  optimal.cs            O(log n) time / O(1) space
// #  Binary search with rotation awareness   [binary-search-rotated-direct]
// #  ties with optimal-variant.cs on O(log n) time / O(1) space
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Each iteration determines which half is sorted and eliminates half the
// #  search space by comparing mid with right boundary.
// ##########################################################################

public class Solution
{
    public int Search(int[] nums, int target)
    {
        // my solution
        int l = 0;
        int r = nums.Length - 1;
        while (l <= r)
        {
            int mid = l + (r - l) / 2;
            if (target == nums[mid])
            {
                return mid;
            }
            //which part is sorted.
            if (nums[mid] > nums[r])
            {
                //left is sorted
                if (nums[l] <= target && target < nums[mid])
                    r = mid - 1;
                else
                    l = mid + 1;
            }

            else
            {
                // right half is sorted
                if (nums[mid] < target && target <= nums[r])
                    l = mid + 1;
                else
                    r = mid - 1;
            }

        }
        return -1;
    }
}

/*
================================================================================
 PATTERN : Modified Binary Search - pick the sorted half
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  l    left edge of the window that could still contain target
  r    right edge of that window; nums[r] is also the anchor for the pivot test
  mid  probe index, and the split point between the sorted half and the rotated half
WHY THIS PATTERN
  The array is sorted but rotated once, so it is not globally sorted, yet every
  split at mid leaves at least one half that IS fully sorted. Comparing
  nums[mid] with nums[r] tells you which half that is, and inside a sorted half
  a simple range test on target says whether target can live there. That gives
  one discard per step, exactly like plain binary search, which is what the
  sorted-but-shifted shape of the input invites.
BRUTE FORCE
  The first correct thing most people write is a for loop over nums returning
  the first index where nums[i] == target. That is O(n) time and always right,
  but it throws away the fact that the data is almost sorted. Since each step
  here can drop half the remaining window, the loop is the clear loser once the
  array is large.
INVARIANT
  At the top of every iteration, if target exists in nums then its index is
  inside [l, r]. Each branch only moves l or r past a range that has been proved
  not to hold target: either the sorted half whose endpoints bracket target is
  kept, or that half is dropped because target falls outside its low..high
  range. The window strictly shrinks every pass (mid-1 or mid+1), so the loop
  ends either on a returned index or with l > r, meaning target was never in the
  array.
WHY COMPARE AGAINST NUMS[R], NOT NUMS[L]
  The pivot test uses nums[mid] > nums[r]. With distinct values, nums[mid] ==
  nums[r] can only happen when mid == r, and that case falls into the else
  branch where the "right half" is the single element nums[mid] - still
  correctly sorted, so the range test target <= nums[r] behaves. The nums[l]
  version needs an extra >= and careful thought when l == mid, so this form has
  fewer edge cases.
WATCH OUT
  Duplicates break the pivot test: with input like [3,1,3,3,3] and mid landing
  on a 3 equal to nums[r], the code cannot tell which side is sorted and may
  discard the half that holds target. The bounds are asymmetric on purpose -
  nums[l] <= target && target < nums[mid] on one side, nums[mid] < target &&
  target <= nums[r] on the other - because mid was already tested for equality;
  flipping one of those strict signs is a silent correctness change, not a
  crash. An empty nums is safe by accident: r becomes -1, the while body never
  runs, and -1 comes back.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Could you do this in two passes instead of one clever loop?
     Yes - first binary search for the rotation point (the smallest element),
     then binary search the one sorted stretch that can contain target. Same
     O(log n), easier to explain, but two loops and an extra index to keep
     straight.
  2. What if the array may contain duplicates?
     Add a case for nums[mid] == nums[r]: shrink with r-- and retry. It stays
     correct but degrades to O(n) in the worst case, for example an array of all
     equal values with one odd element.
  3. Write it recursively.
     Pass l and r as arguments and tail-call into the chosen half; the logic is
     unchanged but you pay stack frames, so the iterative form here is the
     better answer when asked for O(1) extra space.
  4. The caller wants the number of times the array was rotated instead of an
  index.
     Search for the minimum with the same halving idea - compare nums[mid] to
     nums[r] and keep the side that can hold the dip; the index of the minimum
     is the rotation count.
TRIGGER
  Input described as sorted and then rotated or shifted, with a lookup asked for
  in better than linear time.
C# NOTE
  Array.BinarySearch(nums, target) is tempting and wrong here, since it assumes
  a fully sorted array - the hand-written loop is required. Also note mid uses l
  + (r - l) / 2 rather than (l + r) / 2, which keeps the sum from overflowing
  C#'s 32-bit int when both indices are large.
COMPLEXITY
  Time  : O(log n)
  Space : O(1)
================================================================================
*/
