// ##########################################################################
// #  optimal.cs            O(n log n) time / O(n) space
// #  max-heap simulated via min-heap on negated priority
// #  [heap-simulate-collisions]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF (from submission-0)
// #
// #  each of n stones is pushed once and popped/re-pushed O(n) times total,
// #  each heap op is O(log n)
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
 PATTERN : Max-heap simulation with negated priorities
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
WHY A HEAP
  Every turn needs the two current largest stones, and one turn can insert a
  brand-new value (Math.Abs(first - second)) that must be ranked against
  everything still standing. A sorted array would give you the two largest for
  free but re-inserting newStone costs a shift; sorting once and re-sorting each
  turn is worse still. A heap is the structure that answers "give me the max"
  and "absorb this new value" with the same logarithmic cost, and nothing here
  ever needs the middle of the order, so a heap's weaker-than-sorted guarantee
  is exactly enough.
THE NEGATED PRIORITY
  System.Collections.Generic.PriorityQueue is a MIN-heap and there is no max
  flag. Enqueue(stones[i], -stones[i]) stores the real weight as the element and
  its negation as the sort key, so the heaviest stone has the most negative key
  and dequeues first. The element/priority pair is doing two jobs: the priority
  orders, the element carries the value you actually want back. That is why
  Dequeue() returns the true positive weight and never needs re-negating. The
  same trick reappears on the push inside the loop: newStone goes in with
  -newStone. If you forget the minus on either Enqueue you get a min-heap that
  silently smashes the two lightest stones and still returns a plausible-looking
  number. Alternative worth remembering: new PriorityQueue<int,
  int>(Comparer<int>.Create((a, b) => b.CompareTo(a))) inverts the comparer
  instead, which avoids negation entirely - relevant if weights could ever reach
  int.MinValue, where -x overflows back to itself.
TERMINATION
  Each pass of the while loop removes exactly two entries and pushes back at
  most one, so pq.Count strictly decreases every iteration. It cannot stall, and
  it exits with Count equal to 0 or 1 - never more, because the guard is Count >
  1. That two-out-one-in accounting is the whole termination argument; there is
  no bound on the values that needs checking.
WHY CONTINUE IS CORRECT
  When first == second both stones are destroyed and nothing survives, so
  skipping the push is the literal rule. Pushing Math.Abs(0) = 0 instead would
  also terminate and also give the right answer, since a 0 sits at the bottom of
  the max-heap and gets consumed last - but it leaves dead zero entries
  inflating pq.Count and forces extra loop passes to grind them away. The
  continue keeps the heap holding only real stones, which makes the invariant
  clean: the multiset in pq is always exactly the set of stones currently on the
  table.
THE EMPTY CASE
  TryPeek(out int element, out _) is not decoration. An even-length input can
  smash down to zero stones - [2,2] leaves the heap empty - and Peek() would
  throw InvalidOperationException there. The hasElement ? element : 0 fallback
  covers that and also covers an empty stones array. The second out is discarded
  because the stored priority is just the negation you already know; you want
  the element.
CORRECTNESS CHECK
  The parity of the total weight never changes. The equal branch removes 2 *
  first, an even amount. The unequal branch replaces first and second with first
  - second, dropping 2 * second, also even. So the returned value has the same
  parity as the sum of the input array - a cheap sanity check on any answer, and
  a good thing to state out loud before an interviewer asks. It also bounds the
  result: the answer never exceeds the largest input stone.
THE FOLLOW-UP TRAP
  An interviewer will ask whether always smashing the two heaviest MINIMIZES the
  final stone. It does not - that is Last Stone Weight II, where you may pair
  stones freely and the answer is the minimum achievable difference between two
  subset sums, solved by a subset-sum DP over sum/2, not by a heap. This
  solution is correct because the problem MANDATES the two-heaviest rule; it is
  a faithful simulation, not a greedy optimization, and defending it as "greedy
  works here" is the wrong answer.
SMALL CLEANUPS
  Math.Abs on first - second is defensive but unreachable in the negative
  direction: first was dequeued before second from a max-ordered heap, so first
  >= second always and the subtraction is already non-negative. Keeping Abs
  costs nothing and documents intent, but know why it never fires. Separately,
  the n-iteration Enqueue warm-up loop can be replaced by the constructor
  overload taking an IEnumerable of (element, priority) tuples, or by
  EnqueueRange, which builds the heap in a single pass rather than n independent
  sift-ups - the overall bound is unchanged because the smash loop dominates,
  but it is one less loop to read.
COMPLEXITY
  Time  : O(n log n)
  Space : O(n)
================================================================================
*/
