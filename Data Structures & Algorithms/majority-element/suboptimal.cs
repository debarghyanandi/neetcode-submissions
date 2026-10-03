// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n) time / O(n) space
// --------------------------------------------------------------------------

public class Solution
{
    public int MajorityElement(int[] nums)
    {
        Dictionary<int, int> count = new Dictionary<int, int>();
        int res = 0, maxCount = 0;

        foreach (int num in nums)
        {
            if (!count.ContainsKey(num))
            {
                count[num] = 0;
            }
            count[num]++;

            if (count[num] > maxCount)
            {
                res = num;
                maxCount = count[num];
            }
        }
        return res;
    }
}

/*
================================================================================
 PROBLEM : Given an int array nums of size n, return the majority element. The
           majority element appears more than n/2 times (strictly more). You
           may assume it always exists. Example: [3,2,3] -> 3
 PATTERN : Hash Map frequency count + running max
================================================================================
IDEA
  Walk nums once and count each value in the dictionary count.
  After each increment, if count[num] beats maxCount, num becomes res.
  The majority value ends with the highest count of all, so it is the
  final leader in res. This works because it appears more than n/2 times.
  Unlike optimal.cs (Boyer-Moore voting), it keeps a counter per value.
EXAMPLE
  nums = [2,2,1,1,1,2,2]
  2:1 res=2 max=1 | 2:2 res=2 max=2 | 1:1, 1:2 (2 > 2 false, no change)
  1:3 res=1 max=3 | 2:3 (not > 3, keep 1) | 2:4 res=2 max=4
  Answer: 2 (the leader switched to 1 and back to 2)
COMPLEXITY
  Time  O(n)  one pass, each dictionary update is O(1) on average
  Space O(n)  count may hold up to about n/2 distinct keys
WATCH OUT
  - The test is a strict >, so on a tie the value that reached the count
    first stays in res. This is fine only because a majority is guaranteed.
  - If no majority exists, it silently returns the most frequent value.
    For an empty nums it returns 0. It never checks count > n/2.
  - ContainsKey plus two count[num] reads is three lookups per element.
    Use TryGetValue to make it one lookup.
  - Faster exit: return num as soon as count[num] > nums.Length / 2.
================================================================================
*/
