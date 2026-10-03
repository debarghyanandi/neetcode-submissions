public class Solution {
    public int MajorityElement(int[] nums) {
        Dictionary<int, int> count = new Dictionary<int, int>();
        int res = 0, maxCount = 0;

        foreach(int num in nums)
        {
            if(!count.ContainsKey(num))
            {
                count[num] = 0;
            }
            count[num]++;

            if(count[num] > maxCount){
                res = num;
                maxCount = count[num];
            }
        }
        return res;
    }
}