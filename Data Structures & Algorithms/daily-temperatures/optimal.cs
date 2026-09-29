// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
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
 PROBLEM : Given an array temperatures of daily temperatures, return an array
           res. res[i] is how many days you wait after day i to get a strictly
           warmer day. If no warmer day ever comes, res[i] is 0. Example:
           [73,74,71,71,75,70] -> [1,3,2,1,0,0]
 PATTERN : Monotonic Stack (decreasing, stores indices)
================================================================================
IDEA
  Walk left to right and keep a stack of indices still waiting for a warmer
  day.
  Their temperatures never increase from bottom to top.
  When curr is warmer than the top, pop val and set res[val] = i - val, since
  day i is the first warmer day for val. Then push i.
  It is correct because a day stays on the stack only until the first warmer
  day. So when it is popped, i is the nearest such day.
EXAMPLE
  [73,74,71,71,75,70]: i=1 pops 0 (res0=1); i=2 and i=3 push (71 not > 71)
  i=4 (75) pops 3 (res3=1), 2 (res2=2), 1 (res1=3); stack becomes [4,5]
  Indices 4 and 5 are never popped, so they keep 0 -> [1,3,2,1,0,0]
COMPLEXITY
  Time  O(n)  each index is pushed once and popped at most once (amortized
              O(1))
  Space O(n)  res plus a stack that can hold all indices (falling
              temperatures)
PATH TO OPTIMAL
  Brute force: for each i, scan right for a warmer day - O(n^2) time.
  Monotonic stack (optimal.cs) - O(n) - each day is settled once, no rescans.
KEYWORDS
  monotonic stack, next greater element, amortized O(1), stack of indices
WATCH OUT
  - Use strict > in the while. With >=, equal days pop each other early
    (in the example, index 2 would wrongly get 1).
  - Store indices, not temperatures. You need i - val for the distance.
  - Days left on the stack at the end need no work. C# already fills res with
    0.
  - Check stack.Count > 0 before Peek, or an empty stack throws.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it in O(1) extra space besides res?
     -> Go right to left. To jump forward from j, use j += res[j] until you
        find a warmer day or res[j] is 0. Still O(n) amortized, with no stack.
  2. Return the warmer temperature instead of the wait in days?
     -> Same loop, but set res[val] = curr. It is the classic next greater
        element.
  3. What if the array is circular (Next Greater Element II)?
     -> Loop i from 0 to 2n-1 and use i % n. Push only when i < n. O(n) time.
  4. Why is the nested while loop still O(n)?
     -> Total pops can never be more than total pushes, and there are n
        pushes.
TRIGGER
  You need, for each element, the nearest element to the right (or left) that
  is greater or smaller.
================================================================================
*/
