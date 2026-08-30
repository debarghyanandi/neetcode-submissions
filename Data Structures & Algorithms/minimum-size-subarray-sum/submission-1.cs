public class Solution
{
    public int MinSubArrayLen(int target, int[] nums)
    {
        //My solution but instead of prefix sum we can use a sum and add when right++
        // and substruct when left++
        
        int sum = 0;
        int left = 0;
        int result = int.MaxValue;

        for (int right = 0; right < nums.Length; right++)
        {
            sum = sum + nums[right]; // First add the element
            while (sum >= target)
            {
                result = Math.Min(result, right - left + 1);
                sum = sum - nums[left];  // when left++ remove that left value value.
                left++;
            }
            
        }

        return result == int.MaxValue ? 0 : result;
    }
}