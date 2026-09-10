public class KthLargest {
    //My solution
    private PriorityQueue<int, int> pq;
    private int k;

    public KthLargest(int k, int[] nums) {
        this.k = k;
        this.pq = new PriorityQueue<int, int>();
        foreach(int v in nums){
            pq.Enqueue(v, v);
            
            if(pq.Count > k)
                pq.Dequeue();
        }
    }
    
    public int Add(int val) 
    {
        pq.Enqueue(val, val);
            
        if(pq.Count > k)
        pq.Dequeue();

        return pq.Peek();
    }
    
}
