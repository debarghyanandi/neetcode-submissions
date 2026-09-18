// ##########################################################################
// #  optimal.cs            O(n log n) time / O(n) space
// #  max-heap via negated-priority min-heap   [heap-simulate-collisions]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  each of n stones is enqueued once and the smash loop
// #  dequeues/re-enqueues O(n) times total, each heap operation costing
// #  O(log n)
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
WHY A HEAP
  The problem hands you the greedy rather than asking you to discover it: the
  rules state that the two heaviest stones collide each turn, so there is no
  exchange argument to make. What the code has to supply is a structure that
  answers "what is the current maximum" after every mutation, where the mutation
  is awkward - the residue newStone lands back somewhere in the middle of the
  ordering, not at either end. A sorted array would need a shift per round to
  reinsert; re-sorting per round is worse still. Two Dequeues and at most one
  Enqueue per round is exactly the operation set a binary heap is built for.
THE NEGATION TRICK
  PriorityQueue<int,int> in .NET is a min-heap, and this file never passes a
  comparer to the constructor. The max-heap comes from splitting element and
  priority: Enqueue(stones[i], -stones[i]) and Enqueue(newStone, -newStone)
  store the positive weight as the payload and its negation as the sort key.
  Dequeue hands back the element, so first and second are ordinary positive
  weights and Math.Abs works on real magnitudes. The equivalent alternative is
  new PriorityQueue<int,int>(Comparer<int>.Create((a, b) => b - a)) with (w, w)
  enqueued. Negation is safe at these weights - the constraint floor of 1 means
  -stones[i] never approaches int.MinValue.
LOOP INVARIANT
  At the top of every while iteration the heap holds exactly the multiset of
  stones still standing - no placeholders, no stale entries. Heap order
  therefore guarantees first is the heaviest survivor and second the next, so
  first >= second holds unconditionally. Each pass removes two entries and
  pushes back at most one, so Count strictly decreases every iteration and the
  loop cannot spin; it exits with the heap holding one stone or none.
THE EQUAL-PAIR SHORTCUT
  When first == second both stones are annihilated and the continue pushes
  nothing back. Enqueueing Math.Abs(first - second), which is 0, would also give
  the right answer: a 0 carries priority 0 and sorts behind every real stone in
  this max-heap, so it only resurfaces once nothing real is left, and abs(x - 0)
  = x leaves a genuine stone untouched. The continue just refuses to circulate
  those phantoms. Note the consequence that the next section depends on - this
  branch is the only way the heap can drain to empty.
WHY TRYPEEK AND NOT PEEK
  The input is guaranteed non-empty, so it is tempting to end with return
  pq.Peek(). On an input like [1,1] or [2,7,4,1,8,1] the cancellation
  path empties the heap, and Peek throws InvalidOperationException on an empty
  PriorityQueue. TryPeek collapses both terminal states into one expression:
  hasElement false yields 0, which is precisely the value the problem specifies
  when no stone remains. The discarded second out parameter is the priority,
  i.e. -element.
WATCH OUT
  Writing Enqueue(newStone, newStone) - forgetting the minus on just the
  re-insert - compiles, runs, and silently reverts that entry to min-heap
  ordering. It still returns a number, so the bug shows up only as a wrong
  answer on inputs where the residue matters. Second, Math.Abs never actually
  fires: heap order already makes first - second non-negative. Keep it as
  documentation of intent if you like, but do not read it as evidence that
  dequeue order is interchangeable, because taking second before first is not
  something this code can do.
FOLLOW-UP
  An interviewer can ask why the result is well defined at all, since "the two
  heaviest" is ambiguous when several stones share a weight and the heap breaks
  those ties arbitrarily (PriorityQueue is not stable). The answer is that equal
  weights are interchangeable: swapping which of two identical stones is called
  first produces an identical multiset afterward, so every tie-breaking order
  reaches the same final heap. That is also why this solution can ignore
  insertion order entirely and store nothing but the weight.
COMPLEXITY
  Time  : O(n log n)
  Space : O(n)
================================================================================
*/
