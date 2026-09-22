// ##########################################################################
// #  optimal.cs            O(n log n) time / O(n) space
// #  max-heap greedy simulation   [max-heap-greedy]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Each enqueue and dequeue operation on the heap is O(log n), and O(n)
// #  such operations are performed during initialization and simulation.
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
 PATTERN : Max-heap simulation - repeatedly smash the two heaviest
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  pq          min-heap holding every current stone; element = weight, priority = -weight
  first       heaviest stone this round
  second      second heaviest stone this round
  newStone    Math.Abs(first - second), the leftover piece put back
  hasElement  false only when the heap ran empty, meaning all stones cancelled
WHY THIS PATTERN
  The problem always asks for the two largest values, then changes the set and
  asks again. A sorted array would have to be re-sorted after every smash, but a
  heap gives the maximum in O(log n) and takes the leftover back in O(log n). pq
  is exactly the live multiset of stones, so the loop is a direct simulation of
  the rules, not a clever reformulation.
BRUTE FORCE
  Keep the stones in a List, sort it each round, take the last two, remove them,
  insert the difference. That is O(n) rounds times O(n log n) per sort, so O(n^2
  log n), and the insert shifts elements too. It loses because a full sort
  recomputes order for stones that never moved, while a heap only repairs the
  one path touched by the change.
INVARIANT
  At the top of every while iteration, pq contains exactly the stones that still
  exist, with the heaviest on top because each priority is the negated weight.
  One iteration removes two stones and adds at most one, so Count strictly drops
  and the loop must end. When it ends, zero or one stone is left, which is the
  definition of the answer.
NEGATED PRIORITY
  C# PriorityQueue dequeues the SMALLEST priority first, so it is a min-heap.
  Enqueuing weight w with priority -w flips the order and makes Dequeue return
  the heaviest stone. The value and the priority are two separate arguments
  here, so both the initial Enqueue and the newStone Enqueue must remember to
  negate; forgetting once silently returns the lightest stone instead.
WATCH OUT
  The element and its priority are stored twice and nothing enforces that they
  stay opposites - a later edit that pushes newStone with priority newStone
  breaks the heap with no error. The if/else with continue is dead weight: if
  first == second then Math.Abs is 0, and pushing a 0 stone would still
  terminate and still give the same final answer, so the branch is an
  optimisation, not a correctness guard. A null stones array throws a
  NullReferenceException on stones.Length before anything else. TryPeek handles
  the empty-array and all-cancelled cases and returns 0, so there is no separate
  Count check.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you build the heap without n separate Enqueue calls?
     pq.EnqueueRange(stones.Select(s => (s, -s))) hands all items over at once,
     which lets the queue heapify in one pass instead of n sift-ups. Cost is the
     LINQ allocation and slightly less obvious code.
  2. What if the rule became "smash the three heaviest each round"?
     Same shape - Dequeue three times, push back whatever the new rule produces.
     The heap does not care how many you pull; only the termination argument
     changes, since you must still remove more than you add.
  3. What if you must also report every pair that was smashed, in order?
     Record (first, second) into a List inside the loop before the Enqueue. No
     change to complexity, but memory grows to O(n) extra entries.
  4. Last Stone Weight II asks for the smallest possible remaining weight
  instead. Does this code adapt?
     No. That version is a subset-sum partition problem solved with a boolean DP
     over half the total sum; greedily taking the two largest does not minimise
     the leftover.
TRIGGER
  The state changes after every step and each step needs the current maximum (or
  minimum) of a shrinking set.
C# NOTE
  Because the value and the key are the same number, PriorityQueue<int, int>
  with a reversed comparer - new PriorityQueue<int,
  int>(Comparer<int>.Create((a, b) => b - a)) - would let you Enqueue(s, s) and
  drop the negation entirely, at the cost of a comparer call per comparison.
COMPLEXITY
  Time  : O(n log n)
  Space : O(n)
================================================================================
*/
