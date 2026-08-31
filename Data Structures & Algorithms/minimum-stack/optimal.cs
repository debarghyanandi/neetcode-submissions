// ##########################################################################
// #  optimal.cs            O(1) time / O(n) space
// #  auxiliary stack tracking running minimum   [min-stack-parallel-stack]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  a parallel stack pushes/pops in lockstep with the main stack, each
// #  slot holding the min seen up to that depth, giving O(1) GetMin at the
// #  cost of one extra int per element
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
 PATTERN : Auxiliary stack - push the running minimum in parallel
 SOURCE  : YOUR OWN SOLUTION - your own annotation at c76939d
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  GetMin has to answer in constant time, but the set of live values changes on
  every Push and Pop, so scanning is out. The saving observation is that a stack
  only ever mutates at the top: the contents after any sequence of operations
  are always some prefix of the push history. So the minimum for a given state
  can be computed once, at the moment that state is created, and stored with the
  element that created it. minStack is exactly that - one precomputed answer per
  element.
INVARIANT
  Two things hold before and after every public operation:
  1. stack.Count == minStack.Count. Push does exactly one Push on each, Pop does
  exactly one Pop on each; nothing else touches them.
  2. minStack.Peek() == the minimum of everything currently in stack.
  Invariant 1 is what makes the bare minStack.Pop() in Pop legal with no
  emptiness check - if stack.Pop() did not throw, minStack is non-empty too.
WHY POP NEEDS NO RECOMPUTATION
  The correctness argument an interviewer wants. If the pushes so far are
  v1..vk, then the i-th entry of minStack is min(v1..vi). Popping stack returns
  the structure to the state it held after v1..v(k-1) - a state that literally
  existed earlier - and the minStack entry now exposed, min(v1..v(k-1)), is the
  answer computed for precisely that state. Contrast the tempting single-int
  approach: one `min` field answers GetMin fine, but once you pop the element
  that was the minimum you have no way to recover the previous one without an
  O(n) rescan.
WATCH OUT
  Push reassigns the parameter val before pushing it onto minStack, so the order
  of the two lines is load-bearing: stack.Push(val) must run first, while val
  still holds the caller's value. Move it below the Math.Min line and you would
  silently store running minima into the real stack, and Top() would start
  lying.

  The ternary minStack.Count == 0 ? val : minStack.Peek() guards Peek on an
  empty stack. The common alternative is seeding minStack with int.MaxValue,
  which removes the branch but leaves a sentinel that GetMin would return on an
  empty structure. The check here has no such hole.

  Top, Pop and GetMin all throw InvalidOperationException on an empty stack.
  That is the problem's guarantee (calls are valid), not a property of this
  code.
THE FOLLOW-UP: SHRINKING MINSTACK
  Expect "can you use less than one extra entry per element?" Yes: push onto
  minStack only when val <= minStack.Peek(), and on Pop, pop minStack only if
  the value popped from stack equals minStack.Peek().

  The trap is <= versus <. With pushes 2 then 2, strict < stores the 2 only
  once; the first Pop sees a popped value equal to minStack.Peek() and removes
  it, leaving the surviving 2 with no min entry - GetMin then reports a stale or
  wrong answer. Using <= stores both duplicates and keeps the counts aligned.
  The variant that avoids duplicate storage entirely is a stack of (value,
  count) pairs, incrementing the count on a repeat of the current minimum.

  Neither variant improves the worst case: a strictly decreasing input pushes an
  entry every time. This file trades that constant factor for code with no
  conditional in Pop at all.
TRIGGER
  Reach for a parallel stack whenever a LIFO structure must also report an
  aggregate in O(1). The aggregate has to be foldable as a prefix - computable
  from (previous aggregate, new value) alone, which is why min here is just
  Math.Min(val, previous). Max is the same code with Math.Max. Median or "second
  smallest" do not qualify: they cannot be reconstructed from the prior answer
  plus the new element, and need a heap or ordered structure instead.
COMPLEXITY
  Time  : O(1)
  Space : O(n)
================================================================================
*/
