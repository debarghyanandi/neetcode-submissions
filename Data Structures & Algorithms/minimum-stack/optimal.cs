// ##########################################################################
// #  optimal.cs            O(1) time / O(n) space
// #  parallel stack tracking minimum   [parallel-stack-min]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Each operation (push, pop, top, getMin) performs constant work by
// #  delegating to synchronized stack operations.
// ##########################################################################

public class MinStack
{
    // The real stack: every value, in order.
    public Stack<int> stack;

    // Parallel stack. minStack.Peek() is always the minimum of everything
    // currently in `stack` - one entry pushed per push, one popped per pop.
    public Stack<int> minStack;

    public MinStack()
    {
        stack = new Stack<int>();
        minStack = new Stack<int>();
    }

    public void Push(int val)
    {
        stack.Push(val);

        // The new running minimum is the smaller of (this value, the old
        // running minimum). On an empty minStack the value is its own min.
        val = Math.Min(val, minStack.Count == 0 ? val : minStack.Peek());
        minStack.Push(val);
    }

    public void Pop()
    {
        stack.Pop();
        minStack.Pop();     // heights stay equal, so this is always valid
    }

    public int Top()
    {
        return stack.Peek();
    }

    public int GetMin()
    {
        return minStack.Peek();
    }
}

/*
================================================================================
 PATTERN : Two Stacks - parallel stack of running minimums
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  stack     every pushed value, in push order
  minStack  minStack at height h = min of the bottom h values in stack
  val       in Push, reused: first the new value, then the new running min
WHY THIS PATTERN
  The problem asks for push, pop, top and getMin, each in constant time. A stack
  only removes from the top. So the minimum of the first h values never changes
  while those values stay on the stack. That means we can compute the minimum
  once for each height and store it in minStack. GetMin then just reads
  minStack.Peek().
BRUTE FORCE
  The simplest correct approach uses one stack. GetMin scans every element and
  returns the smallest, so GetMin is O(n) time and the other operations are
  O(1). It loses because getMin must be constant time, and a series of n GetMin
  calls would cost O(n^2).
INVARIANT
  stack and minStack always have the same height. The top of minStack is the
  minimum of all values now in stack. Push keeps this true: the new minimum is
  Math.Min(val, old top). Pop keeps it true: removing both tops brings back the
  exact minimum stored for the lower height. That minimum is still correct,
  because the values below it did not change.
WATCH OUT
  The comment on Pop says the call is "always valid". That is only true when the
  stack is not empty. On an empty MinStack, stack.Pop() throws
  InvalidOperationException, and so do Top and GetMin. Also, stack and minStack
  are public fields. Any outside code can push to one stack and not the other,
  and then the equal-height rule breaks.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you use less extra memory?
     Push to minStack only when val <= minStack.Peek(). In Pop, pop minStack
     only when the popped value equals minStack.Peek(). You must use <= and not
     <, or duplicate minimums get lost. This saves memory when the minimum
     rarely changes, but in the worst case (values going down) it still uses
     O(n).
  2. Can you do it with one stack and one extra number?
     Keep a variable min. When val < min, push 2*val - min and set min = val. On
     Pop, if the top is below min, get the old min back as 2*min - top. It uses
     O(1) extra memory, but 2*val - min can overflow int, so you need long
     values.
  3. How would you build a queue that returns its minimum in O(1)?
     Build the queue from two of these min-stacks, one for input and one for
     output. Move items to the output stack only when it is empty. The minimum
     is the smaller of the two stack minimums. Each operation is O(1) amortized,
     which means O(1) on average over many calls.
TRIGGER
  The problem is a stack-like data structure (you only add or remove at one end)
  that must also answer a whole-content query like min or max in O(1).
C# NOTE
  A single Stack<(int Val, int Min)> of value tuples keeps each value and its
  minimum together. The two heights then cannot get out of step, and a caller
  cannot change one without the other.
COMPLEXITY
  Time  : O(1)
  Space : O(n)
================================================================================
*/
