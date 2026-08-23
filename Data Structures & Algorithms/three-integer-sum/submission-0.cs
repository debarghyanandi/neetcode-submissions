public class Solution
{
    public List<List<int>> ThreeSum(int[] nums)
    {
        //My Solution.
        var res = new List<List<int>>();

        Array.Sort(nums);

        int i = 0;

        foreach (int num in nums)
        {
            if (num > 0)
                break;

            if (i > 0 && num == nums[i - 1])
            {
                i++;
                continue;
            }

            var twoSum = TwoSum(nums[(i + 1)..], -num);
            i++;

            if (twoSum.Count == 0)
                continue;

            foreach (var entry in twoSum)
            {
                res.Add(new List<int> { num, entry[0], entry[1] });
            }
        }

        return res;
    }

    private List<List<int>> TwoSum(int[] numbers, int target)
    {
        int l = 0;
        int r = numbers.Length - 1;

        var res = new List<List<int>>();

        while (l < r)
        {
            int sum = numbers[l] + numbers[r];

            if (sum == target)
            {
                res.Add(new List<int> { numbers[l], numbers[r] });

                int dupl = numbers[l];
                int dupr = numbers[r];

                while (l < r && numbers[l] == dupl)
                    l++;

                while (l < r && numbers[r] == dupr)
                    r--;
            }
            else if (sum > target)
            {
                r--;
            }
            else
            {
                l++;
            }
        }

        return res;
    }
}