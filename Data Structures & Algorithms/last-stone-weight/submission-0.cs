public class Solution {
    public int LastStoneWeight(int[] stones) {
        //My solution
        PriorityQueue<int, int> pq = new PriorityQueue<int, int>();

        for (int i=0; i < stones.Length; i++)
        {
            pq.Enqueue(stones[i], -stones[i]);
        }

        while(pq.Count > 1)
        {
            int first = pq.Dequeue();
            int second = pq.Dequeue();

            if(first == second)
                continue;
            else
            {
                var newStone = Math.Abs(first-second);
                pq.Enqueue(newStone, -newStone);
            }     
        }
        bool hasElement = pq.TryPeek(out int element, out _);
        return hasElement ? element : 0;
    }
}
