// ##########################################################################
// #  suboptimal.cs         O(n) time / O(n) space
// ##########################################################################

public class Solution
{
    public bool ContainsNearbyDuplicate(int[] nums, int k)
    {
        //My solution
        Dictionary<int, int> map = new Dictionary<int, int>(); //num / index

        for (int i = 0; i < nums.Length; i++)
        {
            if (map.ContainsKey(nums[i]) && (i - map[nums[i]]) <= k)
            {
                return true;
            }
            // If dictionary contains a duplicate value but the <= k is false
            // then we should Update
            map[nums[i]] = i;
        }
        return false;
    }
}

/*
================================================================================
 PROBLEM : Given an int array nums and an int k, return true if two different
           indices i and j have nums[i] == nums[j] and |i - j| <= k. Otherwise
           return false. Example: nums = [1,2,3,1], k = 3 -> true (the two 1s
           are 3 apart).
 PATTERN : Hash Map (value -> last seen index)
================================================================================
IDEA
  Scan nums once. The map stores, for each value, the last index where it
  was seen. At index i, if map has nums[i] and i - map[nums[i]] <= k, return
  true. In every case, then set map[nums[i]] = i. This is correct because the
  latest earlier index is always the closest one to i, so checking it alone
  is enough. Unlike optimal.cs, the map is never shrunk to a window of k.
EXAMPLE
  nums = [1,0,1,1], k = 1
  i=0: map{1:0}. i=1: map{1:0,0:1}. i=2: 1 seen at 0, gap 2 > 1, update 1:2.
  i=3: 1 seen at 2, gap 1 <= 1 -> return true.
COMPLEXITY
  Time  O(n)  one pass, and each map lookup or write is O(1) on average
  Space O(n)  the map can hold every distinct value of nums
WATCH OUT
  - If you skip the update when the gap is too big, the map keeps the oldest
    index. [1,0,1,1], k=1 then wrongly returns false.
  - Use map[nums[i]] = i, not map.Add. Add throws on a key that already
    exists.
  - The && order matters. ContainsKey must run first, or map[nums[i]] throws
    KeyNotFoundException for a new value.
  - No Math.Abs is needed: i only grows, so i - map[nums[i]] is always > 0.
================================================================================
*/
