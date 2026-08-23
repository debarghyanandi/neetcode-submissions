public class Solution
{
    public List<List<int>> ThreeSum(int[] nums)
    {
        //Same My solution but used for loop.
        //Manual increament is fragile and not worth here.
        //also populated the res alongside 2Sum operation.
        //No extra space req.
        var res = new List<List<int>>();
        Array.Sort(nums);

        for (int i=0; i < nums.Length; i++)
        {
            if (nums[i] > 0)
                break;

            if (i > 0 && nums[i] == nums[i - 1])
                continue;
            
            TwoSum(nums, i + 1, nums.Length - 1, -nums[i], nums[i], res);
        }

        return res;
    }

    private void TwoSum(int[] nums, int l, int r, int target, int fixedNum, List<List<int>> res)
    {
        while (l < r)
        {
            int sum = nums[l] + nums[r];

            if (sum == target)
            {
                res.Add(new List<int> { fixedNum, nums[l], nums[r] });

                int dupl = nums[l], dupr = nums[r];

                while (l < r && nums[l] == dupl) l++;

                while (l < r && nums[r] == dupr) r--;
            }
            else if (sum > target)
                r--;
            else
                l++;
        }
    }
}