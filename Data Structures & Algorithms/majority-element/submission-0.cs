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