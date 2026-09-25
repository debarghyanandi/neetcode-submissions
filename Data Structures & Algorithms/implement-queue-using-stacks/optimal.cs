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
 PATTERN : Two Stacks - amortized queue with lazy transfer
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  stack    input stack; Push always lands here, newest on top
  reverse  output stack; holds the front of the queue on top, in reversed order
WHY THIS PATTERN
  The problem asks for FIFO order (first in, first out) using only stack
  operations, and a stack gives LIFO order. Pouring one stack into another
  reverses the order, so the oldest element ends up on top. That is why Pop and
  Peek read from reverse, and the refill from stack only happens when reverse is
  empty.
BRUTE FORCE
  The simple first attempt is to move every element into reverse on each Push,
  take the element, then move them all back, so the input stack always stays in
  queue order. That costs O(n) per operation instead of O(1) amortized. It loses
  because the same elements get shuffled back and forth over and over, even when
  nobody asked for the front.
INVARIANT
  At every point, the queue from front to back is reverse read top-to-bottom,
  then stack read bottom-to-top. Push preserves this because it appends to the
  back of stack, which is the back of the queue. The transfer loop preserves it
  because moving all of stack onto reverse when reverse is empty flips the order
  exactly once, keeping the relative sequence intact.
WHY THE GUARD MUST BE "EMPTY"
  The transfer is allowed only when reverse.Count == 0. If you poured stack into
  a non-empty reverse, the newer elements would sit on top of older ones and
  come out first, breaking FIFO. That single condition is the whole correctness
  argument for the lazy transfer.
AMORTIZED COST
  Any single Pop can cost O(n) when it triggers the drain loop, but each element
  is pushed to stack once, popped from stack once, pushed to reverse once,
  popped from reverse once. That is four stack operations per element over its
  whole life, so a sequence of m operations costs O(m) total.
WATCH OUT
  Pop and Peek do not check whether the queue is empty; on an empty object the
  drain loop does nothing and reverse.Pop() throws InvalidOperationException.
  Both fields are public, so outside code can push into reverse directly and
  destroy the ordering invariant - they should be private readonly. The if/else
  in both methods duplicates the return line; if a future edit changes only one
  branch the two paths will drift apart.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you write Pop without duplicating the return statement?
     Do the transfer inside a plain if with no else, then fall through to a
     single return reverse.Pop(). Same behaviour, one exit point, less chance of
     the two branches drifting.
  2. How would you implement a stack using two queues instead?
     Mirror the idea: on Push, enqueue into an empty queue then move all of the
     other queue behind it, so the newest element is always at the front. That
     makes Push O(n) and Pop O(1), the opposite trade-off from this file.
  3. What if the queue must be safe for several threads?
     Two plain Stack<int> objects are not thread safe, and the drain loop is a
     multi-step operation that must not be interleaved. Wrap each public method
     in a lock on a private object, or switch to ConcurrentQueue if the
     stack-only restriction is lifted.
  4. How would you add a Size or Count operation?
     Return stack.Count + reverse.Count, which is O(1) since both stacks track
     their own count. No change to the transfer logic is needed.
TRIGGER
  Reach for this when a problem forces you to build one ordering (FIFO) out of a
  container that only gives the opposite ordering (LIFO), and per-operation cost
  may be amortized.
C# NOTE
  Stack<int> here stores value types directly in its internal array, so no
  boxing happens; the old non-generic System.Collections.Stack would box every
  int. Making both fields private readonly would also let the constructor stay
  as is while blocking outside mutation.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
