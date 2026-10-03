// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public int MajorityElement(int[] nums)
    {
        int res = 0;
        int count = 0;

        // If count becomes 0, choose the current number
        // as the new majority candidate.
        //
        // If the current number matches the candidate,
        // increase count.
        //
        // If it is different, decrease count.
        //
        // Since the majority element appears more than n / 2 times,
        // it survives all the cancellations.

        foreach (int num in nums)
        {
            if (count == 0)
                res = num;

            count += (num == res) ? 1 : -1;
        }

        return res;
    }
}

/*
================================================================================
 PROBLEM : Given an integer array nums, return the majority element: the value
           that appears more than n / 2 times. You may assume it always
           exists. Example: [3,2,3] -> 3.
 PATTERN : Boyer-Moore Voting (candidate + counter)
================================================================================
IDEA
  Keep one candidate in res and a vote counter in count.
  When count is 0, the current num becomes the new res.
  A num equal to res adds a vote. Any other num removes one.
  Each removal cancels one res against one different value. The majority
  has more copies than all other values together, so it is never fully
  cancelled and is res at the end.
EXAMPLE
  nums = [2,2,1,1,1,2,2] (the candidate changes twice)
  2:res=2,c=1 2:c=2 1:c=1 1:c=0 1:res=1,c=1 2:c=0 2:res=2,c=1
  Answer: 2. Note that 1 led for a while but was cancelled out.
COMPLEXITY
  Time  O(n)  one foreach pass over nums, O(1) work per element
  Space O(1)  only two ints, res and count
PATH TO OPTIMAL
  Count each value with a nested loop - O(n^2) / O(1) - the simplest start.
  Sort, return nums[n/2] - O(n log n) / O(1) - the middle slot is majority.
  Hash map of counts - O(n) / O(n) - one pass, no sort (suboptimal.cs).
  Boyer-Moore voting - O(n) / O(1) - same time, no map (this file).
KEYWORDS
  majority element, Boyer-Moore voting, candidate cancellation, hash map
WATCH OUT
  - The order matters: check count == 0 and reset res BEFORE you update
    count. If you swap them, res is set to the wrong element.
  - If no majority is guaranteed, the code still returns some value.
    [1,2,3] returns 3, which is wrong. You need a second pass to verify.
  - The comment says the majority "survives all the cancellations". That
    is only true when a majority really exists.
  - Do not read res in the middle of the loop. It can hold a value that
    is not the answer, like 1 in the example above.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if a majority element may not exist?
     -> After the vote, count how often res appears in a second pass. Return
        it only if that count is > n / 2. This is still O(n) / O(1).
  2. Find all elements that appear more than n / 3 times (Majority II).
     -> Keep two candidates with two counters. When a num matches neither,
        decrement both. Then verify both. O(n) / O(1). It extends to k-1.
  3. Can you prove it works?
     -> Each decrement removes one pair of different values. The majority has
        more than half the copies, so pairing cannot remove all of them.
  4. Is there another O(1)-space method?
     -> Count each of the 32 bits. Set a bit in the answer if more than n/2
        numbers have it. O(32n) time, so it is slower in practice.
TRIGGER
  You need the value that appears more than n/k times, using O(1) memory.
================================================================================
*/
