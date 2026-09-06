// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  monotonic decreasing stack of indices   [monotonic-stack]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  each index is pushed and popped at most once, so total work across the
// -  while loop is bounded by n
// --------------------------------------------------------------------------

public class Solution {
    public int[] DailyTemperatures(int[] temperatures)
    {
        int[] result = new int[temperatures.Length];
        var stack = new Stack<int>(); //indices

        for (int i = 0; i < temperatures.Length; i++)
        {
            int currentTemp = temperatures[i];
            while (stack.Count > 0 && currentTemp > temperatures[stack.Peek()])
            {
                int index = stack.Pop();
                result[index] = i - index;
            }
            stack.Push(i);
        }
        return result;
    }
}

/*
================================================================================
 PATTERN : Monotonic Stack - decreasing stack of unresolved indices
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The question "how many days until a warmer temperature" is
  next-greater-element in disguise: for each i you want the smallest j > i with
  temperatures[j] > temperatures[i], then report j - i. The brute force scans
  forward from every i until it finds a warmer day, which re-walks the same
  suffix over and over. The insight that kills the rescan: while you are
  scanning forward for day i, you also pass every day between i and its answer -
  and those days are exactly the ones that are colder than or equal to
  temperatures[i], so they are still unanswered too. One left-to-right pass can
  serve all of them at once if you park them somewhere. The stack is that
  parking lot.
INVARIANT
  At the top of every iteration of the outer loop, stack holds the indices of
  all days in [0, i) whose answer is not yet known, in increasing index order
  from bottom to top, and their temperatures are strictly decreasing from bottom
  to top.

  Both halves matter. "Not yet known" is why an index leaving the stack is the
  moment to write result[index]. "Strictly decreasing" is why the inner while
  loop is allowed to stop at the first non-match: once currentTemp is not
  greater than temperatures[stack.Peek()], it cannot be greater than anything
  deeper either, so nothing below is resolvable by day i.

  The push at the end of each iteration preserves the invariant: after the while
  loop drains everything with temperature < currentTemp, the new top is >=
  currentTemp, so pushing i keeps the strict decrease.
ALGORITHM
  1. Allocate result of the same length as temperatures. Its zero-fill is
  load-bearing - see WATCH OUT.
  2. For each i, read currentTemp = temperatures[i].
  3. While the stack is non-empty and currentTemp > temperatures[stack.Peek()],
  pop that index and set result[index] = i - index. Day i is the answer for that
  day.
  4. Push i, whether or not anything was popped. Day i now has an unknown answer
  of its own.
  5. Return result. Whatever remains on the stack is left as-is.
WHY IT IS CORRECT
  The claim to defend is that when index is popped at step i, day i really is
  the nearest warmer day, not just some warmer day.

  Warmer: the while condition tested currentTemp > temperatures[index] before
  popping.

  Nearest: suppose some j with index < j < i had temperatures[j] >
  temperatures[index]. When the loop reached that j, index was still on the
  stack (it is only removed by being popped, and it was popped at i > j), and by
  the decreasing invariant index was at or below the top with everything above
  it colder than temperatures[index] - hence colder than temperatures[j] too. So
  j's while loop would have drained down to index and popped it at time j.
  Contradiction: no such j exists.

  Note the stack stores indices, not temperatures. That is what makes both
  temperatures[stack.Peek()] and the distance i - index available at pop time;
  storing values would lose the distance.
WATCH OUT
  Strict > is required in the while condition. Equal temperatures do not count
  as warmer, so a day must stay parked when it ties. Turn it into >= and [73,
  73, 74] returns result[0] = 1 instead of 2, because day 1 would evict day 0
  without being warmer than it. This is the single most likely typo in the file
  and the first thing an interviewer probes.

  Indices left on the stack at the end are days with no warmer future day. They
  are never popped, so result[index] is never assigned and keeps the 0 that new
  int[temperatures.Length] gave it - which is the required answer. This is
  correct but implicit; if you ported this to a language without
  zero-initialized arrays, or refactored to a reused buffer, you would have to
  fill zeros explicitly.
FOLLOW-UP: THE NESTED LOOP IS NOT QUADRATIC
  The while inside the for looks like it might rescan, and the interviewer will
  ask. Count pushes instead of iterations: each index i is pushed exactly once,
  at the bottom of the outer loop, and each pop removes it permanently - popped
  indices are never re-pushed. So the total number of while-body executions
  across the whole run is bounded by the number of pushes, one per element, not
  by the length of any single inner loop. A single iteration can pop many
  indices (a big warm day drains the whole stack), but that iteration is paid
  for by all the cheap earlier ones that only pushed.
TRIGGER
  Reach for this shape when a problem asks, for every element, about the next
  (or previous) element that is strictly greater or strictly smaller - "days
  until warmer", "next greater element", "span of stock price", "largest
  rectangle in histogram". The tell is that the answer for element i depends on
  a forward scan whose stopping condition is a comparison against
  temperatures[i]. Then decide two things: whether the stack should be
  decreasing (next greater, as here) or increasing (next smaller), and whether
  the comparison is strict (ties must survive) or not.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
