// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// --------------------------------------------------------------------------

public class Solution
{
    public bool hasDuplicate(int[] nums)
    {
        // Build the whole set first, then compare sizes.
        // A HashSet silently drops repeats, so a smaller Count means duplicates existed.
        return new HashSet<int>(nums).Count < nums.Length;
    }
}

/*
================================================================================
 PROBLEM : Given an integer array nums, return true if any value appears at
           least twice, and false if every value is distinct. Only a yes/no
           answer is needed, not which value repeats. Example: [1,2,3,3] ->
           true.
 PATTERN : Hash Set (dedupe and compare sizes)
================================================================================
IDEA
  Put every value of nums into a new HashSet<int> in one step. A set keeps
  only one copy of each value, so each repeat makes the set smaller than the
  array. The code returns Count < nums.Length. This is correct because the
  two sizes match exactly when no value repeats. Unlike optimal.cs, it never
  stops early: it always builds the full set first.
EXAMPLE
  nums = [1,2,3,1]: set = {1,2,3}, Count 3 < Length 4 -> true
  nums = [7]: set = {7}, Count 1 < Length 1 is false -> false
  nums = []: set = {}, 0 < 0 is false -> false (empty input is safe)
COMPLEXITY
  Time  O(n)  each element is hashed and inserted once, average O(1) per
              insert
  Space O(n)  the set can hold all n values when none repeat
WATCH OUT
  - No early exit: for [5,5,1,2,...] it still hashes the whole array. If the
    interviewer asks you to stop at the first repeat, loop with Add() instead.
  - A null nums throws ArgumentNullException in the HashSet constructor.
    Check for null first if the input can be null.
  - Keep "<" (or "!="). Do not write "Count > Length". It is never true, so
    the method would always return false.
================================================================================
*/
