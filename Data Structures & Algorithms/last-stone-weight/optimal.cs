// ##########################################################################
// #  optimal.cs            O(n log n) time / O(n) space
// ##########################################################################

public class Solution
{
    public int LastStoneWeight(int[] stones)
    {
        //My solution
        PriorityQueue<int, int> pq = new PriorityQueue<int, int>();

        for (int i = 0; i < stones.Length; i++)
        {
            pq.Enqueue(stones[i], -stones[i]);
        }

        while (pq.Count > 1)
        {
            int first = pq.Dequeue();
            int second = pq.Dequeue();

            if (first == second)
                continue;
            else
            {
                var newStone = Math.Abs(first - second);
                pq.Enqueue(newStone, -newStone);
            }
        }
        bool hasElement = pq.TryPeek(out int element, out _);
        return hasElement ? element : 0;
    }
}

/*
================================================================================
 PROBLEM : You get an array stones of positive weights. Each turn, smash the
           two heaviest stones: if equal, both vanish; if not, the lighter
           vanishes and the heavier becomes (heavier - lighter). Return the
           weight of the last stone, or 0 if none is left. Example:
           [2,7,4,1,8,1] -> 1.
 PATTERN : Heap (max-heap simulation)
================================================================================
IDEA
  Put every stone in pq with priority -stone, so Dequeue gives the largest.
  While pq holds 2+ stones, pop first and second (the two heaviest). If they
  differ, push back newStone = first - second; if equal, push nothing.
  At the end, TryPeek returns the survivor or we return 0.
  Correct because the heap always hands us exactly the two heaviest stones.
EXAMPLE
  stones = [2,7,4,1,8,1]
  8,7 -> push 1; 4,2 -> push 2; 2,1 -> push 1; 1,1 -> equal, push nothing
  pq = [1], answer 1
COMPLEXITY
  Time  O(n log n)  n Enqueues, then at most n-1 rounds of O(log n)
                    Dequeue/Enqueue
  Space O(n)        pq holds at most n stones
PATH TO OPTIMAL
  Re-sort the array every round - O(n^2 log n) - simple but slow.
  Keep a sorted list, insert newStone by binary search - O(n^2) - no
  re-sort, but each list insert shifts elements.
  Max-heap (this file) - O(n log n) - pop and push are only O(log n).
KEYWORDS
  heap, priority queue, max-heap, simulation, greedy, top two elements
WATCH OUT
  - C# PriorityQueue is a MIN-heap. Forget the -stones[i] priority and you
    smash the lightest stones, which gives a wrong answer.
  - Math.Abs is not needed: first >= second always. It is harmless, but an
    interviewer may ask why it is there.
  - Empty result: [2,2] leaves pq empty. Dequeue or Peek would throw here;
    TryPeek with the 0 fallback handles it.
  - The Enqueue loop costs O(n log n). pq.EnqueueRange builds the heap in
    O(n).
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Last Stone Weight II: you may smash ANY two stones, minimize the result?
     -> Split the stones into two groups with sums as close as possible. Use
        0/1 knapsack (subset sum) DP up to total/2: O(n * sum) time, O(sum)
        space.
  2. Weights are small integers (bounded by some max W)?
     -> Use a count array (bucket sort) and walk down from the top: O(n + W).
        Beats the heap only when W is not much larger than n.
  3. Why is greedy on the two heaviest correct here?
     -> The rules force that choice; nothing is decided by us. So this is pure
        simulation, and the heap only makes finding the max fast.
  4. No heap library allowed?
     -> Write a binary max-heap on an array with sift-up and sift-down. Same
        O(n log n) time, O(1) extra space if you heapify stones in place.
TRIGGER
  The problem keeps asking for "the largest (or smallest) items right now"
  while elements are removed and new ones are added.
================================================================================
*/
