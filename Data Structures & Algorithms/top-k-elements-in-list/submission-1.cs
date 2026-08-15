public class Solution {
    public int[] TopKFrequent(int[] nums, int k) {
       var count = new Dictionary<int, int>();

       foreach (var num in nums)
        {
            if(!count.TryGetValue(num, out int v)){
                count[num] = 0;
            }
          count[num]++;
        }

        var heap = new PriorityQueue<int, int>();
        foreach (var entry in count)
        {
            heap.Enqueue(entry.Key,entry.Value);
            if (heap.Count > k){
                heap.Dequeue();
            }
        }

        var res = new int[k];
        for ( int i=0; i < k; i++){
            res[i] = heap.Dequeue();
        }
        return res;
    }
}
