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
 PATTERN : Two parallel stacks - store the min alongside each value
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Optimal
================================================================================
VARIABLES
  stack      every pushed value, in push order
  minStack   minStack top = minimum of all values now in stack; same height as stack
WHY THIS PATTERN
  The problem asks for push, pop, top and GetMin all in constant time. A single
  stack cannot answer GetMin without scanning, and a heap cannot be popped in
  stack order. Because the minimum only depends on the values currently present,
  we can freeze the answer at push time: minStack records, for each depth, the
  min of everything at or below that depth. Popping a value throws away exactly
  the one minStack entry that was created with it, so the old answer is restored
  for free.
BRUTE FORCE
  Keep only stack and make GetMin loop over its items with foreach, returning
  the smallest. Push, Pop and Top stay O(1) but GetMin becomes O(n) per call, so
  a run of n GetMin calls costs O(n^2). This file trades one extra int per
  element for a constant-time GetMin.
INVARIANT
  After every operation, stack.Count == minStack.Count, and minStack.Peek()
  equals the minimum of all values in stack. Push keeps it by pushing
  Math.Min(val, old top), which is by definition the min of the new contents.
  Pop keeps it by removing one entry from each, which returns both stacks to the
  exact state they had one push earlier. So GetMin can just read the top.
DUPLICATE MINIMA ARE SAFE
  Repeated values are handled without any special case because minStack stores a
  value per element, not per distinct minimum. Pushing 5, 5 stores 5 twice, so
  popping one 5 still leaves 5 as the reported minimum, which is correct. A
  space-saving variant that only pushes when val <= current min would need that
  <= exactly, not <, or the second 5 would be lost.
WATCH OUT
  Pop, Top and GetMin all call Peek or Pop with no emptiness check, so calling
  any of them on an empty MinStack throws InvalidOperationException. Push reuses
  the parameter val as scratch for the computed minimum; after that line the
  original argument is gone, which is fine here since stack.Push(val) already
  happened, but any later edit that needs the raw value will silently read the
  min instead. Both fields are public, so outside code could push to stack alone
  and break the equal-height invariant that Pop relies on.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you cut the memory used by minStack?
     Push onto minStack only when val <= minStack.Peek(), and in Pop only pop
     minStack when stack.Pop() returned a value equal to minStack.Peek(). Best
     case it stores one entry; worst case (strictly decreasing input) it is the
     same size, and the code gets more branches.
  2. Can you do it with one stack and no extra stack at all?
     Keep a single min field and push the encoded value 2*val - min when a new
     minimum arrives, restoring the old min on pop. It works but needs a long to
     avoid overflow, and it is much harder to read than this version.
  3. What if you also need GetMax?
     Add a third parallel stack built the same way with Math.Max. The invariant
     and the equal-height argument carry over unchanged.
  4. How does this change if it must be a queue instead of a stack (min at
  front, pop from front)?
     The freeze-at-push trick fails because the oldest element leaves first. Use
     two stacks to simulate the queue, each with its own min stack, and take the
     min of the two tops, giving amortized O(1).
TRIGGER
  A data structure must report an aggregate (min, max, count) in O(1) while
  elements leave in the exact reverse order they arrived.
C# NOTE
  System.Collections.Generic.Stack<int> is the right choice here: it is an
  array-backed stack of value types, so no per-node allocation and no boxing.
  Making stack and minStack private readonly (or exposing only the five methods)
  would stop callers from desynchronising the two heights.
COMPLEXITY
  Time  : O(1)
  Space : O(n)
================================================================================
*/
