public class Solution
{
    public int MinSubArrayLen(int target, int[] nums)
    {
        //My solution
        int[] prefix = new int[nums.Length + 1];

        for (int i = 0; i < nums.Length; i++)
        {
            prefix[i + 1] = prefix[i] + nums[i];
        }

        int left = 0;
        int result = int.MaxValue;

        for (int right = 0; right < nums.Length; right++)
        {
            while (prefix[right + 1] - prefix[left] >= target)
            {
                result = Math.Min(result, right - left + 1);
                left++;
            }
        }

        return result == int.MaxValue ? 0 : result;
    }
}