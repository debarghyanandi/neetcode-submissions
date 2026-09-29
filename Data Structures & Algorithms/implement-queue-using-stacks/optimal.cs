// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// #  Two-stack FIFO queue   [two-stack-queue]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Pop and Peek require reversing stack to reverse when reverse is empty,
// #  transferring all n elements in worst case.
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

    public void Push(int x)
    {
        stack.Push(x);
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
        else
            return reverse.Pop();
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
        else
            return reverse.Peek();
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
 PATTERN : Two Stacks as a Queue - lazy transfer on empty outbox
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  stack     the inbox; every Push lands here, so the newest item is on top
  reverse   the outbox; it holds items in reversed order, so the oldest item is on top
WHY THIS PATTERN
  The problem asks for FIFO order (first in, first out) but only lets you use
  stacks, which are LIFO (last in, first out). If you pop every item from one
  stack and push it onto another, the order flips. So moving items from stack to
  reverse puts the oldest item on top of reverse, where Pop and Peek can reach
  it.
BRUTE FORCE
  Keep all items in one stack. For each Pop, move every item to a second stack,
  take the top one, then move everything back. This is correct, but every Pop
  and Peek costs O(n), so a run of n operations costs O(n^2). This file avoids
  the move back and only transfers when reverse is empty.
INVARIANT
  The items on reverse, read from top to bottom, are older than every item in
  stack. Each group of items inside each stack stays in arrival order. Pop and
  Peek refill reverse only when it is empty. So the refill never places newer
  items above older ones, and the top of reverse is always the oldest item in
  the whole queue. Empty() checks both stacks, because items can sit in either
  one.
EACH ITEM MOVES AT MOST ONCE
  One Pop can run the while loop over many items. But each item is pushed onto
  stack once, moved to reverse once, and popped from reverse once. Over a whole
  sequence of operations, the cost of each call is O(1) on average. This is
  called amortized cost: the total work split over all calls. Be ready to
  explain this, because the worst single call is still O(n).
WATCH OUT
  If Pop or Peek is called when both stacks are empty, the while loop does
  nothing and reverse.Pop() or reverse.Peek() throws InvalidOperationException.
  That is fine if the problem promises valid calls, but the code does not check
  for it. The transfer loop is copied in both Pop and Peek. If you change one
  copy and forget the other, the two methods will act differently. A private
  helper method would remove this risk. Never refill reverse while it still
  holds items. That would bury the oldest items under newer ones and break FIFO
  order.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can every single operation be O(1) in the worst case, not just on average?
     Not with this lazy design, because one Pop can move n items. You need a
     harder scheme that moves a few items on each call, or you use a real queue
     such as a linked list or a circular array. The trade-off is more complex
     code for a steady cost on every call.
  2. How do you build a stack using queues? (the reverse problem)
     On each Push, add the new item to the queue, then take out and add back the
     older items (count - 1 of them), so the new item is at the front. Push
     becomes O(n) and Pop becomes O(1). This does not work in an amortized way
     like the two-stack version.
  3. How would you add a GetMin() that runs in O(1)?
     Make each stack hold pairs (value, min so far in that stack). The queue
     minimum is the smaller of the two top-of-stack minimums. When items move to
     reverse, recompute the min values, because the order is now flipped.
TRIGGER
  When a problem says to build FIFO behavior using only LIFO structures, or
  needs a queue that also tracks a running stack-style value, think of an inbox
  stack and an outbox stack.
C# NOTE
  stack and reverse are public fields, so any caller can push to them directly
  and break the order. Declare them as private readonly Stack<int>: readonly
  still lets you add and remove items, but the fields can never be pointed at a
  different stack.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
