public class Solution {
    public int[] TwoSum(int[] numbers, int target) {
        int l = 0, r = numbers.Length - 1;
        while(l<r){
            if(numbers[l] + numbers[r] == target)
            return new[]{l+1,r+1};
            
            while((l<r) && (numbers[l] + numbers[r] > target)){
                r--;
            }
            while((r>l) && (numbers[l] + numbers[r] < target)){
                l++;
            }
            
        }
        return Array.Empty<int>();
    }
}
