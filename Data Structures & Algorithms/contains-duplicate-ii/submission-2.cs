public class Solution {
    public bool ContainsNearbyDuplicate(int[] nums, int k) {
        HashSet<int> window = new HashSet<int>();

        for (int right = 0; right < nums.Length; right ++)
        {
           if (window.Contains(nums[right]))
           return true;

           window.Add(nums[right]);

           if (window.Count > k)
            {
                window.Remove(nums[right - k]);
            }

        }

        return false;
    }
}