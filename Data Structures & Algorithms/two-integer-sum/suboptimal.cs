// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n^2) time / O(1) space
// -  brute force nested loop pair check   [brute-force-nested-loop]
// -  ranks below optimal.cs (O(n) time / O(n) space)
// -
// -  No '//My solution' marker in the source (from submission-42)
// -
// -  two nested loops check every pair (i,j) with j>i for the target sum,
// -  no extra storage beyond loop indices
// --------------------------------------------------------------------------

public class Solution
{
    public int[] TwoSum(int[] nums, int target)
    {
        for (int i = 0; i < nums.Length; i++)
        {
            for (int j = i + 1; j < nums.Length; j++)
            {
                if (nums[i] + nums[j] == target)
                {
                    return new int[] { i, j };
                }
            }
        }
        return new int[0];
    }
}
