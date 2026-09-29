// ##########################################################################
// #  optimal.cs            O(1) time / O(n) space
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
 PROBLEM : Design a stack class with Push(val), Pop(), Top() and GetMin().
           Every operation must run in O(1) time. GetMin returns the smallest
           value now in the stack. Pop, Top and GetMin are only called on a
           non-empty stack. Example: Push(-2), Push(0), Push(-3), GetMin ->
           -3, Pop, Top -> 0, GetMin -> -2.
 PATTERN : Stack + parallel min stack (running minimum per level)
================================================================================
IDEA
  Keep two stacks of equal height: stack holds the real values, minStack
  holds the minimum of everything at that level and below. Push adds
  Math.Min(val, minStack.Peek()) to minStack. Pop removes one entry from each.
  This is correct because the minimum below a level never changes while that
  level exists, so the top of minStack is always the current answer.
EXAMPLE
  Push 5,3,7,3 -> stack [5,3,7,3], minStack [5,3,3,3]; GetMin -> 3
  Pop -> minStack [5,3,3]; GetMin -> 3 (the other 3 is still there)
  Pop, Pop -> stack [5], minStack [5]; GetMin -> 5
COMPLEXITY
  Time  O(1)  each call does a fixed number of Push/Pop/Peek on two stacks
  Space O(n)  minStack stores one extra int for every value in stack
PATH TO OPTIMAL
  Plain stack, GetMin scans all values - O(n) GetMin - simplest version.
  Stack + sorted multiset of values - O(log n) per op - no full scan.
  Parallel minStack (this file, optimal.cs) - O(1) per op - saves each min.
KEYWORDS
  min stack, design, auxiliary stack, running minimum, O(1) getMin, stack
WATCH OUT
  - Space-saving variant (push to minStack only when val <= min): with < the
    duplicate case breaks. Push 3,3, Pop, and GetMin fails on an empty stack.
  - Push and Pop must always touch both stacks. If they skip one, the heights
    drift and GetMin returns a stale value.
  - Pop/Top/GetMin on an empty stack throw InvalidOperationException here.
    There is no guard, so this code relies on the problem's guarantee.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you get O(1) extra space instead of a second stack?
     -> Keep one long min and push val - min. If the diff is negative, val is
        the new min; on pop, rebuild the old min as min - diff. Use long for
        overflow.
  2. Design a Max Stack that also supports PopMax.
     -> Use a doubly linked list plus a sorted map from value to nodes. PopMax
        finds the max and unlinks it: O(log n) per op, more code.
  3. Can you get the min of a queue in O(1)?
     -> Build the queue from two min stacks (in and out). Each value moves
        between them only once, so every operation is amortized O(1).
TRIGGER
  A design problem that asks for an aggregate (min/max) of a LIFO structure in
  O(1) time means you store that aggregate next to each pushed element.
================================================================================
*/
