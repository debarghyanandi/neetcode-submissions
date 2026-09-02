// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// #  two stacks, transfer on demand   [two-stack-queue]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF (from submission-0)
// #
// #  amortized O(1) per op, but a single Pop/Peek can transfer all n
// #  elements from the input stack to the output stack in the worst case;
// #  space is O(n) to hold all enqueued elements across the two stacks.
// ##########################################################################

public class MyQueue {
    public Stack<int> stack;
    public Stack<int> reverse;
    //my solution.
    public MyQueue() {
        stack = new Stack<int>();
        reverse = new Stack<int>();
    }
    
    public void Push(int x) {
        stack.Push(x);
    }
    
    public int Pop() {
        if (reverse.Count == 0){
            while(stack.Count != 0){
                reverse.Push(stack.Pop());
            }
            return reverse.Pop();
        }
        else return reverse.Pop(); 
    }
    
    public int Peek() {
        if (reverse.Count == 0){
            while(stack.Count != 0){
                reverse.Push(stack.Pop());
            }
            return reverse.Peek();
        }
        else return reverse.Peek(); 
    }
    
    public bool Empty() {
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
 PATTERN : Two stacks - inbox/outbox with lazy one-way transfer
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  A stack reverses whatever passes through it. One stack gives you LIFO; pushing
  every element through a second stack reverses it a second time, and two
  reversals restore arrival order. So stack holds new arrivals in LIFO order,
  reverse holds an already-flipped run whose top is the oldest surviving element
  - the queue front. No index shifting, no circular buffer, no linked list.
INVARIANT
  reverse, read top to bottom, is the oldest block of the queue in FIFO order.
  stack, read bottom to top, is the newest block in FIFO order. The logical
  queue is reverse (top-down) followed by stack (bottom-up). Push only ever
  touches stack; Pop and Peek only ever read reverse. Nothing is ever moved from
  reverse back into stack - the transfer is strictly one way.
WHY THE TRANSFER MUST BE LAZY
  The guard reverse.Count == 0 is the whole correctness argument, not an
  optimization. If you drained stack into reverse while reverse still held
  elements, the newer items would land on top of the older ones and come out
  first - the queue would emit them out of order. Draining only when reverse is
  empty means the two blocks in the invariant never interleave: the old block is
  fully consumed before a new one is built. Equivalently, once elements sit in
  reverse their relative order is frozen and correct, and anything in stack is
  strictly younger than all of them.
THE FOLLOW-UP: WHY THE SLOW POP IS RARE
  An interviewer will point at the while loop inside Pop and call it linear.
  Answer with the accounting: every value is pushed onto stack exactly once,
  popped off stack at most once, pushed onto reverse at most once, and popped
  off reverse at most once - four constant-cost touches over its entire
  lifetime, no matter how the calls interleave. Charge those four touches to the
  original Push and every operation is constant amortized. A single Pop can
  still cost as much as the number of buffered elements, so this is amortized,
  not worst-case per call; a real-time queue would need to move a fixed number
  of items per operation instead of draining all at once.
EMPTY IS THE ONE OPERATION THAT SEES BOTH
  Empty must test reverse.Count == 0 && stack.Count == 0. Checking only reverse
  would report empty right after a burst of Pushes with no intervening Pop,
  since nothing has been transferred yet; checking only stack would report empty
  while reverse still holds undelivered elements. This is the one place the
  split representation leaks, and it is the easy bug to reintroduce from memory.
WATCH OUT
  1. Pop and Peek differ in exactly one token: the final call is reverse.Pop()
  versus reverse.Peek(). Getting Peek to also pop is the classic slip - it
  silently drops an element and the failure shows up several operations later.
  2. The if/else in both methods is redundant. When the branch is taken the loop
  runs and then falls through to the same reverse.Pop() as the else; the loop is
  a no-op when reverse is non-empty. The body collapses to: run the while, then
  return. Same behavior, half the code.
  3. Neither Pop nor Peek guards against an empty queue. If both stacks are
  empty the while loop does nothing and Stack<int>.Pop throws
  InvalidOperationException. That is fine under the problem's guarantee that
  calls are valid, but it is what you say out loud rather than let the
  interviewer find.
TRIGGER
  Reach for this whenever you need FIFO but the only primitive you are handed is
  LIFO (or the mirror: a stack from two queues). The generalization is the
  amortized rebuild - a cheap append side plus a frozen consume side, rebuilt
  only when the consume side runs dry. The same shape shows up in a dynamic
  array's doubling and in the two-list functional queue.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
