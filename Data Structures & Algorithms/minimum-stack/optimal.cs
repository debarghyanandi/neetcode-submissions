// ##########################################################################
// #  YOU SOLVED THIS YOURSELF  (submission-0, marked '//My Solution')
// #  the standard answer, reached first try - nothing to trade up to
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
 PATTERN : Auxiliary Stack - carry a PRECOMPUTED ANSWER alongside the data
 SOURCE  : YOUR OWN SOLUTION (submission-0, marked '//My Solution')
 STATUS  : Optimal - O(1) for every operation
================================================================================

WHY THIS PATTERN
  GetMin() is asked for in O(1), but a stack only exposes its top. The trick
  is to notice that "the minimum of the current stack" is not one value - it
  is a HISTORY of values, one per stack depth, and it changes in lockstep
  with the stack itself. A stack is therefore the correct container for it.

  Generalise the idea and it stops being a puzzle: any query answerable in
  O(1) from (new element, answer for the previous state) can be carried in a
  parallel stack. Min, max, running sum, running GCD - same skeleton.

BRUTE FORCE (and why it fails)
  Keep only `stack` and scan it on every GetMin(): O(n) per call, O(1) space.
  Fine if GetMin is rare; a problem if it is in a loop. Alternatively keep a
  single `min` int - which works until that minimum is popped, and then there
  is no way to recover the previous one. That failure is the whole point of
  the problem: the minimum has to be RESTORABLE, and a scalar cannot restore.

INVARIANT
  stack.Count == minStack.Count at all times, and minStack's k-th entry from
  the bottom equals min(stack[0..k]). Because both stacks are pushed and
  popped together, the invariant cannot drift.

ALGORITHM (NeetCode: "Two Stacks")
  1. Push(val)  : stack.Push(val); minStack.Push(min(val, minStack.Peek()))
                  - or val itself when minStack is empty.
  2. Pop()      : pop both.
  3. Top()      : stack.Peek().
  4. GetMin()   : minStack.Peek().

COMPLEXITY
  Time  : O(1) for all four operations, worst case (no amortisation needed).
  Space : O(n) - one extra int per element.

TRIGGER
  "Support <some aggregate> in O(1) alongside normal stack operations."
  If the aggregate is computable from (new value, aggregate of the rest) it
  goes in a parallel stack. If it needs the whole multiset - a median, say -
  it does not, and the answer is two heaps instead.

C# NOTES
  - System.Collections.Generic.Stack<T> is an array-backed T[] with a size
    counter; Push amortises to O(1) and doubles the array when full. Prefer
    it to Stack (non-generic) and to List<T> used as a stack - Peek/Pop read
    better and bounds-check once.
  - Peek() on an empty stack throws InvalidOperationException; the
    `minStack.Count == 0` guard in Push is what keeps the first push legal.
  - The fields are public here, matching the submission. Private is the
    correct call - the class's contract is its five members, and exposing
    the stacks lets a caller break the height invariant from outside.
  - Constructing with `new Stack<int>(capacity)` skips the early doublings
    when an upper bound is known.

WATCH OUT
  - The two stacks must be pushed and popped in lockstep. The "optimisation"
    of only pushing to minStack when val <= current min is valid, but then
    Pop MUST compare before popping minStack - forgetting that is the bug
    this version cannot have.
  - Use <= not < in that variant, or duplicate minima get lost. This version
    sidesteps the issue entirely by always pushing, at the cost of n ints.
  - Reassigning the `val` parameter before pushing to minStack works, but
    reads as if the wrong value lands in `stack`. It does not - stack.Push
    already ran. A separate local would be clearer.
================================================================================
*/
