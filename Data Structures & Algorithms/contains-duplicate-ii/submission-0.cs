public class Solution {
    public bool ContainsNearbyDuplicate(int[] nums, int k) {
        //My solution
        Dictionary <int, int> map = new Dictionary <int, int> (); //num / index

        for(int i = 0; i < nums.Length; i++)
        {
            if (map.ContainsKey(nums[i]) && (i - map[nums[i]]) <= k)
            {
                return true;
            }
            // If dictionary contains a duplicate value but the <= k is false
            // then we should Update
            map[nums[i]] = i;  
        }
        return false;
    }
}