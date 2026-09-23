// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// -  Two-pointer shrink while comparing to target   [two-pointer-sorted]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  Two pointers start at ends and move inward; each passes through array
// -  once in single pass.
// --------------------------------------------------------------------------

public class Solution
{
    public int[] TwoSum(int[] numbers, int target)
    {
        int left = 0;
        int right = numbers.Length - 1;

        while (left < right)
        {
            int sum = numbers[left] + numbers[right];

            if (sum == target)
                return new int[] { left + 1, right + 1 };   // problem wants 1-based

            if (sum > target)
                right--;      // shrink from the large end
            else
                left++;       // grow from the small end
        }

        return Array.Empty<int>();
    }
}

/*
================================================================================
 PATTERN : Two Pointers on a sorted array - converge from both ends
 SOURCE  : Reference solution - not one you solved yourself - your own
           annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  left   index of the smaller candidate, moves right
  right  index of the larger candidate, moves left
  sum    numbers[left] + numbers[right] for the current pair
WHY THIS PATTERN
  The input array numbers is already sorted in non-decreasing order, and we need
  one pair that adds to target. Sorted order means the sum is monotone in each
  index: moving left right can only raise sum, moving right left can only lower
  it. So a single scan with two pointers can steer sum toward target instead of
  testing every pair.
BRUTE FORCE
  The first thing most people write is a double loop over every pair (i, j) with
  i < j and a check numbers[i] + numbers[j] == target. That is O(n^2) time and
  O(1) space. It is correct but ignores the sorted order, so it re-tests pairs
  the two-pointer walk can rule out in one move.
INVARIANT
  At the top of each loop pass, every valid answer pair still lies inside the
  window [left, right]. If sum > target, then numbers[right] paired with any
  index from left to right-1 gives a sum at least as big, so right cannot be
  part of the answer and right-- discards nothing useful; the mirror argument
  justifies left++. Since each pass moves one pointer, the window shrinks by one
  every time, so the loop ends in at most n steps and the answer, if it exists,
  is found before left meets right.
ONE-BASED RETURN
  The return builds new int[] { left + 1, right + 1 }, not the raw indices. The
  comment says the problem wants 1-based positions. This +1 is the classic thing
  to forget, and the loop itself works entirely in 0-based indices, so the
  conversion happens only at the single return point.
WATCH OUT
  The fallback return Array.Empty<int>() gives a zero-length array, not null and
  not a two-element array, so a caller that immediately reads result[0] will
  throw IndexOutOfRangeException. If the problem guarantees exactly one
  solution, this branch is unreachable, but it silently changes the return shape
  if the guarantee is ever dropped. Also, left < right (not <=) is required:
  with <= the same element could pair with itself when left == right. If numbers
  is null or has length 0 the code still behaves - right becomes -1 and the loop
  never runs - but null throws on numbers.Length.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if the input is not sorted and you must return original indices?
     Sort loses the indices, so use a Dictionary<int, int> from value to index
     and check for target - numbers[i] in one pass. That is O(n) time but O(n)
     space, unlike this file's O(1).
  2. What if you must return all distinct pairs that sum to target, not just
  one?
     Keep the same two pointers, but on a match record the pair and then advance
     left and pull back right past duplicate values before continuing, so the
     same value pair is not emitted twice.
  3. How would you extend this to three numbers summing to target?
     Fix one index in an outer loop and run this exact two-pointer scan on the
     rest of the array, giving O(n^2) time and still O(1) extra space.
  4. The array is sorted - why not binary search?
     For each left you could binary search for target - numbers[left], which is
     O(n log n). It works but is slower than this O(n) walk, because the
     two-pointer version reuses the work from the previous step instead of
     restarting the search.
TRIGGER
  The input arrives already sorted and you are looking for a pair (or a window)
  whose sum or difference hits a target.
C# NOTE
  Array.Empty<int>() returns a shared cached zero-length array instead of
  allocating a new int[0] on every miss, which is the right idiom for a "no
  result" array return in C#.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
