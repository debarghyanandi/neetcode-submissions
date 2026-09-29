// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
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
 PROBLEM : Build a FIFO queue using only two stacks (push, pop, peek, count
           only). Support Push(x), Pop() (remove and return the front), Peek()
           (return the front) and Empty(). Pop and Peek are only called on a
           non-empty queue. Example: Push 1, Push 2, Peek, Pop, Empty -> 1, 1,
           false.
 PATTERN : Two Stacks (lazy transfer, amortized O(1))
================================================================================
IDEA
  Push always goes onto stack, the "in" stack. Pop and Peek read from
  reverse, the "out" stack. Only when reverse is empty do we pour all of
  stack into reverse. Pouring flips the order, so the oldest item ends up
  on top of reverse. This is correct because reverse always holds items
  older than anything in stack. So we must never pour while reverse still
  has items.
EXAMPLE
  Push 1, Push 2 -> stack=[1,2] (top 2), reverse=[]
  Pop: reverse empty, pour -> reverse=[2,1] (top 1), return 1
  Push 3 -> stack=[3]; Peek -> 2 (no pour); Pop -> 2
  Pop: reverse empty, pour 3 -> return 3; Empty -> true
COMPLEXITY
  Time  O(n)  each item moves stack->reverse at most once; amortized O(1) per
              op
  Space O(n)  the two stacks together hold each item exactly once
PATH TO OPTIMAL
  Pour everything into the other stack and back on every Pop/Peek - O(n)
    per Pop, because each call moves every item twice.
  Costly Push (keep one stack in queue order on each Push) - O(n) per Push.
  Lazy two stacks (this file) - amortized O(1), because each item moves once.
KEYWORDS
  queue using stacks, two stacks, amortized O(1), FIFO, LIFO, lazy transfer
WATCH OUT
  - Pour only when reverse is empty. If you pour while it has items, newer
    items land on top of older ones and the FIFO order breaks.
  - On an empty queue, Pop/Peek call reverse.Pop() on an empty Stack, which
    throws InvalidOperationException. Check Empty() first if calls can be bad.
  - Empty() must check both stacks. Checking only reverse returns true
    while items still wait in stack.
  - The pour loop is copied into Pop and Peek. Move it into one helper, so
    a fix in one place is not missed in the other.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Why is it amortized O(1) if one Pop can cost O(n)?
     -> Each item is pushed to stack once, moved to reverse once and popped
        once. That is 3 moves per item, so n ops cost O(n) in total.
  2. Now implement a stack using queues.
     -> On Push, add x to the queue, then rotate the older size-1 items to the
        back. Push becomes O(n), Pop/Top O(1), space O(n). It is not amortized.
  3. Can you make every operation worst-case O(1)?
     -> Yes, but it is complex. Spread the transfer over the next few ops
        (Hood-Melville style, or a real-time queue). It uses more bookkeeping.
  4. Add GetMin() to the queue in O(1).
     -> Use two min-stacks. Each entry also stores the min so far. GetMin is
        the smaller of the two tops, still amortized O(1).
TRIGGER
  You must get one order (FIFO) from structures that give the opposite order
  (LIFO), and cheap average cost per op is enough: use two stacks with a
  lazy transfer.
================================================================================
*/
