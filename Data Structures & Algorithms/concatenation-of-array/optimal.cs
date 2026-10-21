// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public int[] GetConcatenation(int[] nums)
    {
        int n = nums.Length;
        int[] ans = new int[2 * n];
        for (int i = 0; i < n; i++)
        {
            ans[i] = ans[i + n] = nums[i];
        }
        return ans;
    }
}

/*
================================================================================
 PROBLEM : Given an int array nums of length n, return an array ans of length
           2n. ans[i] == nums[i] and ans[i + n] == nums[i] for every i < n. In
           short, ans is nums written twice in a row. Example: [1,2,1] ->
           [1,2,1,1,2,1].
 PATTERN : Array traversal with index offset (single pass)
================================================================================
IDEA
  Allocate ans with size 2 * n once, so it never needs to grow.
  Walk i from 0 to n - 1 and write nums[i] into two slots: ans[i] and
  ans[i + n]. The chained assignment does both writes in one statement.
  This is correct because each slot of ans has exactly one source index:
  i for the first half, and i - n for the second half.
EXAMPLE
  nums = [1,2,1], n = 3, ans = [0,0,0,0,0,0]
  i=0: ans[0]=ans[3]=1 i=1: ans[1]=ans[4]=2 i=2: ans[2]=ans[5]=1
  Answer: [1,2,1,1,2,1]
COMPLEXITY
  Time  O(n)  one loop of n steps, each with two O(1) writes
  Space O(1)  no extra memory besides the required output ans
PATH TO OPTIMAL
  List<int> + Add each value, loop twice, ToArray - O(n) / O(n) extra -
    the list may resize and ToArray copies everything again.
  Fixed array of 2n filled in one pass (this file) - O(n) / O(1) extra -
    one allocation, no resizing, half the loop count. No sibling file.
KEYWORDS
  array, concatenation, index offset, i + n, modulo indexing, one pass
WATCH OUT
  - Looping i up to 2n and reading nums[i] throws IndexOutOfRange.
    If you loop to 2n, read nums[i % n] instead.
  - Writing ans[2 * i] instead of ans[i + n] gives the wrong layout.
  - "O(1) space" is only true if the output does not count. Say so.
  - Empty nums is fine here: n = 0 gives an empty ans, the loop never runs.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it with built-in calls?
     -> Use Array.Copy(nums, 0, ans, 0, n) and Array.Copy(nums, 0, ans, n, n).
        Same O(n), but it may run faster thanks to block memory copy.
  2. What if you must repeat nums k times?
     -> Make ans of size k * n and set ans[j] = nums[j % n]. Time O(k * n),
        and there is no extra space beyond the output.
  3. What if the array is huge and you cannot allocate 2n?
     -> Do not build it. Expose a view where get(j) returns nums[j % n]. O(1)
        per read and O(1) memory, but callers must use the view.
  4. Why not use nums.Concat(nums).ToArray()?
     -> It works and is O(n), but LINQ adds iterator overhead and hidden
        copies. Interviewers want to see you control the indices.
TRIGGER
  When output position j maps directly to input index j % n or j - n,
  fill a preallocated array in one pass with an index offset.
================================================================================
*/
