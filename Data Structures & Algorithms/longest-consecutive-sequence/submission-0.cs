public class Solution
{
    public int LongestConsecutive(int[] nums)
    {
        var set = new HashSet<int>();

        foreach (int num in nums)
        {
            set.Add(num);
        }

        int res = 0;

        foreach (int x in set)
        {
            int runLength = 0;

            if (set.Contains(x - 1))
                continue;

            while (set.Contains(x + runLength))
            {
                runLength++;
            }

            if (runLength > res)
                res = runLength;
        }

        return res;
    }
}