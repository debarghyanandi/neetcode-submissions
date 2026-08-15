public class Solution {
    public int[] TopKFrequent(int[] nums, int k) {
        //Init a Dict of value and freq
        var count = new Dictionary<int, int>();
        foreach (var num in nums)
        {
            if(!count.TryGetValue(num, out int v)){
                count[num] = 0;
            }
          count[num]++;
        }
        //Init a Bucket
        List<int>[] freq = new List<int>[nums.Length + 1];
        for(int i = 0; i<freq.Length; i++){
            freq[i] = new List<int>();
        }
        foreach(var entry in count){
            //Index of the freq array is the frequency
            //Each index in freq array is a List(int)
            freq[entry.Value].Add(entry.Key);
        }

        //Now we have to get k elements from the freq starting from end.
        int[]res = new int[k];
        int index = 0;
        for  (int i = freq.Length-1; i > 0 && index < k; i--){
            foreach (int n in freq[i]){
                res[index] = n;
                index++;
                //in a scenario where k+1 elements hold same freq
                //this will crash as max size of res is k.
                //so the below check to get only first k elemnets.
                if (index==k)
                break;
            }
        }
        return res;
    }
}
