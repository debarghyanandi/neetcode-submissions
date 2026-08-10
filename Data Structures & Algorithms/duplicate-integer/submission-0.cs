public class Solution {
    public bool hasDuplicate(int[] nums) {

       HashSet<int> seen = new HashSet<int>();
       foreach (var n in nums){
        if (!seen.Add(n))
            return true;
       }
       return false;
    }
}