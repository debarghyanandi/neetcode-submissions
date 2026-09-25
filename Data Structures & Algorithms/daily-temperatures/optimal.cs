// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  Monotonic stack, pop while warmer   [monotonic-stack]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  Each element is pushed and popped from the stack exactly once, giving
// -  O(n) total operations despite nested loops.
// --------------------------------------------------------------------------

public class Solution
{
    public int[] DailyTemperatures(int[] temperatures)
    {
        int[] res = new int[temperatures.Length];
        var stack = new Stack<int>(); //indices

        for (int i = 0; i < temperatures.Length; i++)
        {
            int curr = temperatures[i];
            while (stack.Count > 0 && curr > temperatures[stack.Peek()])
            {
                int val = stack.Pop();
                res[val] = i - val;
            }
            stack.Push(i);
        }
        return res;
    }
}

/*
================================================================================
 PATTERN : Monotonic Decreasing Stack - next greater element
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  res      res[i] = days from day i until a warmer day, 0 if none
  stack    indices of days still waiting for a warmer day
  curr     temperatures[i], today's temperature
  val      index popped off the stack, the day that just got its answer
WHY THIS PATTERN
  The question asks, for each day, how far ahead the first warmer day is. That
  is a "next greater element" query for every position. A day can only be
  answered by a later day, and once a warmer day arrives it answers every
  earlier day that is colder than it, in order from nearest to farthest. A stack
  of indices kept in decreasing temperature order gives exactly that ordering,
  and res[val] = i - val turns the two indices into a day count.
BRUTE FORCE
  For each i, walk forward with a second loop until temperatures[j] >
  temperatures[i], write j - i, else 0. That is correct and easy to write, but
  it is O(n^2) time on a long non-increasing run, since every day scans the
  whole tail. The stack version pays the scan once: each index is pushed once
  and popped at most once.
INVARIANT
  After each iteration, the temperatures at the indices in stack are strictly
  decreasing from bottom to top, and every index still in the stack has no
  warmer day at or before i. So when curr is greater than
  temperatures[stack.Peek()], day i really is the FIRST warmer day for that
  popped index - any nearer day would have popped it earlier. Indices never
  popped keep res at its default 0, which is the correct "no warmer day" answer.
WATCH OUT
  The comparison must stay strict: curr > temperatures[stack.Peek()]. If it
  became >=, an equal temperature would pop and record a wrong answer, because
  equal is not warmer. res is never explicitly filled with 0 - it relies on new
  int[] zero-initializing, so if you ever rewrite this with a rented or reused
  buffer you must clear it yourself. An empty temperatures array is safe here:
  the loop never runs and a zero-length array comes back. Also note the answer
  depends on comparing temperatures[stack.Peek()], not on any value stored in
  the stack, so pushing temperatures instead of indices would break the i - val
  subtraction.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it without a stack data structure, using res itself?
     Yes - walk i from the end to the start and jump: from j = i + 1, while
     temperatures[j] <= temperatures[i] and res[j] > 0, set j += res[j]. Same
     O(n) amortized feel in practice, O(1) extra space, but the jump logic is
     harder to defend in an interview than the stack.
  2. What if the input is a stream and you cannot hold all days in memory?
     The stack approach already works online - each new temperature resolves and
     emits answers for the days it pops. You only need to buffer the pending
     indices, which is the stack, but a long non-increasing stream still forces
     the stack to grow to the number of unresolved days.
  3. What if you want the previous warmer day instead of the next one?
     Run the same loop from the last index down to 0, and write res[val] = val -
     i. The monotonic structure is unchanged; only the direction and the
     subtraction order flip.
  4. What if temperatures are bounded to a small range, say 30 to 100?
     Keep an array nextIndex[101] of the last seen index per temperature and
     scan from the right, taking the minimum index over all temperatures warmer
     than the current one. That is O(n * range) time with O(range) extra space -
     better memory, worse constant.
TRIGGER
  Each element needs the first later element that beats it by some comparison,
  and the answer is a distance or that element itself.
C# NOTE
  Stack<int> of value-type int avoids boxing, and Peek plus Pop on it are plain
  array-backed index operations; you do not need Count > 0 guarded twice because
  the while condition already short-circuits before Peek runs.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
