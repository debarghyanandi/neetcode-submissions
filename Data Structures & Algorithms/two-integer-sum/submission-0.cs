public class Solution {
    public int[] TwoSum(int[] nums, int target) {

        Dictionary <int,int> sums = new Dictionary <int,int>();
        int index = 0;
        foreach (var n in nums){
            
            if(sums.ContainsKey(target-n))
            return new int[] {sums[target-n],index};
            
            sums[n] = index;
            index++;

        }
        return Array.Empty<int>();

    }
}
