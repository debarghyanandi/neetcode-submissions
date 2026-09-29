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
 PATTERN : Monotonic Stack - pending indices, cooler on top
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  res      res[i] = days to wait after day i for a warmer day (0 if none)
  stack    indices of days still waiting for a warmer day
  curr     temperature of today, temperatures[i]
  val      index of a waiting day that today resolves
WHY THIS PATTERN
  The problem asks, for each day, for the "next greater element" to its right.
  That wording points to a monotonic stack. Each day waits in stack until a
  warmer day arrives. When curr is warmer than the day on top, that day's answer
  is found, so it is popped and res[val] = i - val is written.
BRUTE FORCE
  For each day i, scan forward with j from i+1 until temperatures[j] >
  temperatures[i], then record j - i. This is correct and needs no extra memory.
  But it is O(n^2) time, for example on a strictly falling sequence where every
  scan runs to the end. The stack version avoids this because each index is
  pushed once and popped at most once.
INVARIANT
  From bottom to top, the temperatures at the indices in stack never go up
  (non-increasing), and no day in stack has found a warmer day yet. When day i
  arrives, all smaller temperatures on top are popped, and i is the first warmer
  day for each of them, since any earlier warmer day would have popped them
  already. Indices still in stack at the end never saw a warmer day, so their
  res value stays at the default 0, which is correct.
WATCH OUT
  The comparison must stay strict (curr > temperatures[stack.Peek()]). If you
  change it to >=, a day of equal temperature would wrongly count as "warmer."
  The code sets no answer for days left in stack. It depends on new int[]
  filling res with 0, so if res is ever reused or filled with a sentinel (a
  special marker value such as -1), those days would get wrong answers.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you solve it with O(1) extra space besides the output?
     Yes. Walk from right to left, keeping the hottest value seen so far. For
     day i, start at j = i+1 and jump with j += res[j] until temperatures[j] >
     temperatures[i], or stop when res[j] == 0 (no warmer day exists). This
     removes the stack, and the jumps still add up to amortized O(n), but the
     code is harder to prove correct.
  2. What if you need the previous warmer day, not the next one?
     Use the same stack. After popping every index with a temperature <= curr,
     the index still on top is the previous warmer day. Record it before you
     push i.
  3. What if the input is a stream and you cannot see future days?
     The same loop works online. Each new day resolves the waiting days it
     beats. Days still in stack have no answer yet, and their final answer is 0
     only when the stream ends.
TRIGGER
  The problem asks for "the next (or previous) greater or smaller element" for
  every position in an array.
C# NOTE
  Stack<int> stores the indices as ints with no boxing, and Peek/Pop throw on an
  empty stack, so the check stack.Count > 0 must come first in the while
  condition. An int[] with a top pointer would also work, and it avoids the
  collection wrapper.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
