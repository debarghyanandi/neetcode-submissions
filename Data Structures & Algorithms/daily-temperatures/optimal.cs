// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  monotonic decreasing stack of indices   [monotonic-stack]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself (from submission-0)
// -
// -  each index is pushed and popped at most once, stack holds indices with
// -  non-increasing temperatures
// --------------------------------------------------------------------------

public class Solution {
    public int[] DailyTemperatures(int[] temperatures) {
        int[] res = new int[temperatures.Length];
        var stack = new Stack<int>(); //indices

        for(int i = 0; i < temperatures.Length; i++){
            int curr = temperatures[i];
            while (stack.Count > 0 && curr > temperatures[stack.Peek()])
            {
                int val =  stack.Pop();
                res[val] = i - val;
            }
            stack.Push(i);
        }
        return res;
    }
}

/*
================================================================================
 PATTERN : Monotonic decreasing stack of unresolved indices
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
BRUTE FORCE, AND WHY IT LOSES
  The obvious version: for each i, scan j = i+1 forward until temperatures[j] >
  temperatures[i], write j - i. That rescans the same cold plateau once per
  element, so a descending array like [90, 89, 88, ..., 1] makes every scan run
  to the end. The stack version replaces the repeated forward scan with a single
  backward-looking structure: each index is pushed exactly once and popped at
  most once, so the total work across the whole while loop is bounded by n pops,
  not n scans.
INVARIANT
  At the top of every iteration of the outer for loop, stack holds indices,
  bottom to top, in strictly decreasing order of temperatures[index], and those
  are exactly the indices whose answer has not been decided yet.

  Strictly decreasing follows from the while loop: before pushing i, everything
  with a temperature strictly below temperatures[i] has already been popped, so
  the new top-of-stack neighbor is >= curr. Unresolved follows because res[val]
  is written at the same moment val leaves the stack, and never touched again.
WHY THE POPPED INDEX GETS THE RIGHT ANSWER
  When curr > temperatures[stack.Peek()] fires, i is the FIRST warmer day for
  val, not merely a warmer day. Every index strictly between val and i was
  pushed after val and popped before i arrived, and each of those was popped
  only because something later beat it - meaning none of them was warmer than
  temperatures[val], or val itself would have been popped at that earlier moment
  (the stack is decreasing, so val sits below them and is warmer). So nothing
  between val and i qualifies, and i - val is the true day gap.
TRACE
  temperatures = [73, 74, 75, 71, 69, 72, 76, 73]

  i=0 push 0 stack [0]
  i=1 74>73 pop 0 res[0]=1 stack [1]
  i=2 75>74 pop 1 res[1]=1 stack [2]
  i=3 71<75 push stack [2,3]
  i=4 69<71 push stack [2,3,4]
  i=5 72>69 pop 4 res[4]=1; 72>71 pop 3 res[3]=2; 72<75 stop stack [2,5]
  i=6 76>72 pop 5 res[5]=1; 76>75 pop 2 res[2]=4 stack [6]
  i=7 73<76 push stack [6,7]

  Leftover 6 and 7 are never popped. res = [1,1,4,2,1,1,0,0].
WATCH OUT
  1. The stack stores INDICES, not temperatures. That is the whole reason
  res[val] = i - val can compute a distance; a stack of values would tell you
  what was warmer but not how far back it sat.

  2. The comparison is strict: curr > temperatures[stack.Peek()]. With >= you
  would resolve an equal temperature, but the problem asks for a strictly warmer
  day. On [70, 70, 75] strict gives res[0]=2, res[1]=1; >= would wrongly give
  res[0]=1.

  3. Indices still on the stack at the end are correct as 0 only because C#
  zero-initializes new int[temperatures.Length]. There is no drain loop after
  the for loop - that default IS the "no warmer day" answer. Port this to a
  language without zeroed allocation and you must fill explicitly.

  4. curr is read once into a local before the while loop; the loop compares it
  against temperatures[stack.Peek()], which changes as the stack unwinds. Do not
  accidentally re-index temperatures[i] as the moving side.
TRIGGER
  Reach for this shape whenever the question is "for each element, find the
  nearest element to the right (or left) that is greater/smaller." Next Greater
  Element I and II, Largest Rectangle in Histogram, Trapping Rain Water, Sum of
  Subarray Minimums, Online Stock Span. The knobs are only: direction of the
  sweep, > vs < (which decides increasing or decreasing stack), strict vs
  non-strict, and whether you store index or value. Here it is left-to-right,
  strict >, decreasing, indices.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
