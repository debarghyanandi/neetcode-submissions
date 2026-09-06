// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// #  two stacks, lazy transfer for amortized FIFO   [two-stack-queue]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  push stack accumulates arrivals; pop/peek drain it into a reverse
// #  stack only when empty, giving amortized O(1) per op but O(n) worst
// #  case for a single Pop/Peek, with O(n) space to hold all buffered
// #  elements
// ##########################################################################

public class MyQueue
{
    public Stack<int> stack;
    public Stack<int> reverse;
    //my solution.
    public MyQueue()
    {
        stack = new Stack<int>();
        reverse = new Stack<int>();
    }

    public void Push(int item)
    {
        stack.Push(item);
    }

    public int Pop()
    {
        if (reverse.Count == 0)
        {
            while (stack.Count != 0)
            {
                reverse.Push(stack.Pop());
            }
            return reverse.Pop();
        }
        else return reverse.Pop();
    }

    public int Peek()
    {
        if (reverse.Count == 0)
        {
            while (stack.Count != 0)
            {
                reverse.Push(stack.Pop());
            }
            return reverse.Peek();
        }
        else return reverse.Peek();
    }

    public bool Empty()
    {
        return (reverse.Count == 0 && stack.Count == 0);
    }
}

/**
 * Your MyQueue object will be instantiated and called as such:
 * MyQueue obj = new MyQueue();
 * obj.Push(x);
 * int param_2 = obj.Pop();
 * int param_3 = obj.Peek();
 * bool param_4 = obj.Empty();
 */

/*
================================================================================
 PATTERN : Two Stacks - lazy drain, refill only when output empty
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  A stack only exposes the newest element; a queue needs the oldest. One
  reversal fixes that permanently: pouring stack into reverse flips arrival
  order, so the element that entered first ends up on top of reverse. The whole
  design is "reverse once, lazily, and only when you have to". stack is the
  intake buffer (top = back of the queue), reverse is the output buffer (top =
  front of the queue).
INVARIANT
  At all times, reading reverse from top to bottom and then stack from bottom to
  top yields the queue in exact front-to-back order. Every operation preserves
  it:
  1. Push(item) appends to the tail of the stack segment, which is the tail of
  the queue.
  2. Pop/Peek read reverse's top, which is the head of the queue.
  3. The drain loop moves the entire stack segment into reverse, reversing it -
  which is precisely what turns "bottom-to-top of stack" into "top-to-bottom of
  reverse". Order is unchanged; only the representation moves.
THE GUARD IS THE WHOLE ALGORITHM
  reverse.Count == 0 is not an optimization, it is a correctness condition.
  Draining while reverse still holds items would bury older elements under newer
  ones.
  Concrete break: Push 1, Push 2, Pop -> drain makes reverse = [1 on top, 2
  below], returns 1. Now Push 3, then Pop. With the guard, reverse is non-empty
  so 2 is returned - correct. Without the guard, 3 would be drained on top of 2
  and Pop would return 3, letting the newest element cut in front of the oldest.
  Refill only when the output buffer is exhausted.
AMORTIZED ACCOUNTING
  A single Pop can touch every buffered element, but each element is moved a
  fixed number of times over its lifetime: pushed to stack, popped from stack,
  pushed to reverse, popped from reverse - four operations, never more. No
  element is ever drained twice, because the guard means an element enters
  reverse only once and leaves the structure from there. So the cost of an
  expensive drain is prepaid by the Pushes that filled stack, giving O(1)
  amortized per operation across any sequence.
WATCH OUT
  1. Empty() must test both stacks. Testing only reverse reports empty while
  unread items still sit in stack; testing only stack reports empty while
  drained items wait in reverse.
  2. Pop and Peek on a fully empty queue: the while loop does nothing and
  reverse.Pop()/reverse.Peek() throws InvalidOperationException. That is
  acceptable only because the problem guarantees calls are valid - if an
  interviewer asks for a defensive version, guard with Empty() and return a
  sentinel or throw deliberately.
  3. Peek must not consume. It uses reverse.Peek(), but it still performs the
  drain - peeking is a read that can mutate internal representation while
  leaving the logical queue unchanged.
SHAPE OF THE CODE
  Pop and Peek both spell out an if/else where both branches end in the same
  reverse operation; the else is redundant since the drain leaves reverse
  non-empty. Both collapse to: run the drain when reverse.Count == 0, then
  return reverse.Pop() or reverse.Peek() once. Pulling the drain into one
  private Transfer() method removes the duplicated loop between the two methods
  and makes the guard read as a single precondition. The fields are also public,
  which exposes both buffers to callers who could violate the invariant from
  outside.
ALTERNATIVE DESIGN
  The mirror image is the costly-push variant: on every Push, drain reverse back
  into stack, push the new item, drain back. That makes Pop and Peek a single
  stack operation with no branch, but Push becomes linear every single time, so
  a push-heavy workload has no amortization to fall back on. This version is the
  better default because it charges the transfer only when a reader actually
  needs the old elements, and interleaved pushes never re-pay for elements
  already sitting in reverse.
TRIGGER
  Reach for two stacks whenever you must emulate FIFO semantics on top of
  LIFO-only primitives, or more generally whenever a structure needs its
  elements in the opposite order from how they arrive and the reversal can be
  batched. The tell is that a single reversal serves many subsequent reads -
  which is what makes the lazy drain pay off. The same intake/output-buffer
  split shows up in queue-from-stacks, stack-from-queues, and iterator designs
  that flush a pending batch only on demand.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
