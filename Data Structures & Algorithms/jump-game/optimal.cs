// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public bool CanJump(int[] nums)
    {
        //Greedy
        int n = nums.Length;
        int maxLength = 0;

        for (int i = 0; i < n; i++)
        {

            // you cant reached this index. MaxLength is less.
            if (i > maxLength)
                return false;

            maxLength = Math.Max(maxLength, i + nums[i]);
        }
        return true;
    }
}

/*
================================================================================
 PROBLEM : You get an int array nums. You start at index 0. nums[i] is the MAX
           jump length from index i, so you may jump any 0..nums[i] steps
           forward. Return true if you can reach the last index, else false.
           Example: [2,3,1,1,4] -> true, [3,2,1,0,4] -> false.
 PATTERN : Greedy + running max reach
================================================================================
IDEA
  Walk left to right and keep maxLength, the farthest index reachable so far.
  If i > maxLength, no earlier index can reach i, so return false.
  Otherwise extend maxLength with i + nums[i].
  It is correct because every index up to maxLength is reachable: any jump
  may be shorter than its max, so the reachable set has no gaps.
EXAMPLE
  nums = [3,2,1,0,4]
  i=0: max=3; i=1: max(3,3)=3; i=2: max(3,3)=3; i=3: max(3,3)=3 (zero trap)
  i=4: 4 > maxLength 3 -> return false
COMPLEXITY
  Time  O(n)  one pass, each index checked once
  Space O(1)  only maxLength and i, no extra array
PATH TO OPTIMAL
  Brute force DFS, try every jump from every index - exponential - baseline.
  Memoized DFS / bottom-up DP of "can reach end from i" - O(n^2) - no repeats.
  Greedy max reach (this file) - O(n), O(1) - one number replaces the table.
  No sibling file exists for the earlier steps.
KEYWORDS
  greedy, max reach, array, dynamic programming, reachability, jump game
WATCH OUT
  - Check i > maxLength BEFORE updating. If you update first, a stuck index
    "reaches" itself and the zero trap [3,2,1,0,4] wrongly returns true.
  - A zero is only a trap if maxLength cannot pass it. [2,0,0] is true.
  - i + nums[i] can overflow int if values are near int.MaxValue.
  - No early exit: you could return true once maxLength >= n - 1.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Jump Game II: minimum number of jumps to reach the end?
     -> Greedy BFS by levels: track curEnd and farthest; when i hits curEnd,
        jumps++ and curEnd = farthest. Still O(n) time, O(1) space.
  2. Can you solve it scanning backward?
     -> Keep goal = n-1; going right to left, if i + nums[i] >= goal set goal
        = i. Return goal == 0. Same O(n) / O(1), some find it easier to prove.
  3. Jump Game III: from i jump to i+nums[i] or i-nums[i]; reach a value 0?
     -> Max reach fails since moves go both ways. Use BFS/DFS with a visited
        set: O(n) time, O(n) space.
  4. Why is greedy safe here, not DP?
     -> Reachable indices form one prefix [0..maxLength], so one number holds
        all the state. DP tracks each index for no gain.
TRIGGER
  When you only need yes/no reachability and each step lets you cover a whole
  range forward, track one running farthest-reach value.
================================================================================
*/
