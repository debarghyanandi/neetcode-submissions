public class Solution
{
    public int RemoveDuplicates(int[] nums)
    {
        int k = 0; // Next position to write

        for (int i = 0; i < nums.Length; i++) // Read each number
        {
            if (k < 2 || nums[i] != nums[k - 2])
            {
                nums[k] = nums[i];
                k++;
            }
        }

        return k;
    }
}