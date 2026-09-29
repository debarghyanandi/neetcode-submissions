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
 PATTERN : Max-Heap Simulation - smash the two heaviest stones
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  pq          max-heap of stone weights; priority = -weight, so the heaviest comes out first
  first       the heaviest stone left in this round
  second      the second heaviest stone, so second <= first
  newStone    the piece that survives when first != second, equal to first - second
  hasElement  true if one stone is left at the end, false if none is left
  element     the weight of the last stone, if there is one
WHY THIS PATTERN
  The problem says to take "the two heaviest stones" again and again, and each
  smash can add a new stone. So we need the largest value many times from a set
  that keeps changing. That is exactly what a max-heap does. pq gives first and
  second in O(log n) each, and newStone goes back in O(log n), so the rules are
  followed step by step.
BRUTE FORCE
  Keep the stones in a list. In each round, scan the whole list to find the two
  largest (or sort it again), remove them, and add the difference. There can be
  up to n rounds and each round costs O(n) (O(n log n) if you sort), so the
  total is O(n^2) or worse. It loses because it searches the whole list every
  round to find the maximum, while the heap keeps it ready.
INVARIANT
  At the start of every loop pass, pq holds exactly the stones still alive,
  ordered by -weight. So the first two Dequeue calls always return the two
  heaviest. Each pass removes two stones and adds at most one. So pq.Count goes
  down by at least 1 every pass, and the loop must end with 0 or 1 stone. This
  is the answer the rules produce.
WATCH OUT
  Math.Abs(first - second) is not needed. first comes out before second, so
  first >= second, and first - second is never negative. Math.Abs only hides
  that fact. The priority trick -stones[i] works only because weights are
  positive. A weight of int.MinValue would overflow when negated and break the
  ordering. The case of zero stones at the end (or an empty stones array) is
  handled correctly by TryPeek, which returns 0. A plain Peek would throw an
  exception there.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it with O(1) extra space?
     Yes. Build a max-heap inside the stones array itself with a hand-written
     heapify and sift-down. You then only keep a heap size counter. The time
     stays the same, but you write more code and change the input array.
  2. What if weights are small whole numbers with a known upper bound?
     Use a count array indexed by weight and walk it down from the largest
     weight. Each smash updates two counts. This can beat O(n log n) when the
     bound is small, but it wastes time and memory when the bound is large.
  3. Last Stone Weight II: you may pick ANY two stones. What is the smallest
  possible final weight?
     The heap and the greedy choice no longer work. The answer is to split the
     stones into two groups with sums as close as possible. That is a subset-sum
     DP over sums up to total/2, so the cost depends on the total weight, not on
     n log n.
TRIGGER
  When a problem keeps removing the largest (or smallest) items and puts new
  items back, reach for a heap.
C# NOTE
  PriorityQueue<int,int> is a min-heap, so the code negates the priority.
  Another way is new PriorityQueue<int,int>(Comparer<int>.Create((a, b) =>
  b.CompareTo(a))), which avoids the negation and its overflow risk. You can
  also pass all the (stone, priority) pairs to the constructor, which builds the
  heap in one pass instead of calling Enqueue n times.
COMPLEXITY
  Time  : O(n log n)
  Space : O(n)
================================================================================
*/
