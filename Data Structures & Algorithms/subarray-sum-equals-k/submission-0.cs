public class Solution {
    public int SubarraySum(int[] nums, int k) {
        Dictionary <int, int> map =  new Dictionary<int, int>();//val->count
        map[0] = 1;
        
        int res = 0, preSum = 0;

        foreach (int num in nums) {
            preSum += num;
            
            int remove = preSum - k;

            if(map.TryGetValue(remove, out int count)) // we can omit if
            {
                res += count;
            }
            
            map.TryGetValue(preSum, out int freq);
            map[preSum] = freq + 1;
            
        }

        return res;
    }
}