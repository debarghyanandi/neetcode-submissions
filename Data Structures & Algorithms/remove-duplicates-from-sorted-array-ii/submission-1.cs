public class Solution
{
    public int RemoveDuplicates(int[] nums)
    {
        int write = 0;

        foreach (int num in nums)
        {
            // Keep the first two numbers, or a number
            // different from the one two kept positions back.
            if (write < 2 || num != nums[write - 2])
            {
                nums[write] = num;
                write++;
            }
        }

        return write;
    }
}