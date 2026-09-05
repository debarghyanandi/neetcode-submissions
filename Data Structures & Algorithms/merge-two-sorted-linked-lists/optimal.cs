// --------------------------------------------------------------------------
// -  optimal.cs            O(n + m) time / O(1) space
// -  iterative merge with dummy head   [iterative-merge-dummy-head]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself (from submission-0)
// -
// -  single pass through both lists comparing heads and relinking nodes, no
// -  extra allocation beyond dummy node
// --------------------------------------------------------------------------

public class Solution {
    public ListNode MergeTwoLists(ListNode list1, ListNode list2)
    {
        ListNode dummy = new ListNode(0);
        ListNode tail = dummy;

        while (list1 != null && list2 != null)
        {
            if (list1.val < list2.val)
            {
                tail.next = list1;
                list1 = list1.next;
            }
            else
            {
                tail.next = list2;
                list2 = list2.next;
            }

            tail = tail.next;
        }

        if (list1 != null)
        {
            tail.next = list1;
        }
        else
        {
            tail.next = list2;
        }

        return dummy.next;
    }
}

/*
================================================================================
 PATTERN : Dummy head + two pointers - splice, never allocate
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  Both inputs are already sorted, so the smallest unused node in the whole
  problem is always sitting at the head of list1 or at the head of list2 -
  nowhere else. That single observation removes any need to search, buffer, or
  re-sort: you compare two heads, take the winner, and repeat. The dummy node
  exists only so that the first append is written the same way as every other
  append. Without it you would need a special case to decide which node becomes
  the head before the loop can start.
INVARIANT
  At the top of every iteration:
  (1) dummy.next ... tail is a sorted chain of exactly the nodes already
  consumed, and tail is its last node;
  (2) list1 and list2 point at the first unconsumed node of their own list, and
  every remaining node in each is >= tail.val;
  (3) no node has been copied - the output is built out of the original nodes.
  The body preserves this: it appends min(list1.val, list2.val), which is <=
  every remaining value on both sides and >= tail.val by (2), then advances tail
  so (1) holds again. Each iteration advances list1 or list2 by exactly one
  node, so the loop runs at most n + m times and cannot spin.
ALGORITHM
  1. dummy = new ListNode(0); tail = dummy. dummy.val is never read - 0 is
  arbitrary filler, not a sentinel value that must beat the data.
  2. While both list1 and list2 are non-null: if list1.val < list2.val, splice
  list1 onto tail.next and advance list1; else splice list2 and advance list2.
  3. tail = tail.next after either branch - this is the one line whose omission
  silently drops nodes, since the next iteration would overwrite the same
  tail.next.
  4. After the loop, attach the leftover: tail.next = list1 if list1 is
  non-null, else tail.next = list2.
  5. Return dummy.next, not dummy.
WHY THE LEFTOVER SPLICE IS ONE POINTER WRITE
  The remaining nodes are already a sorted, null-terminated chain, and by
  invariant (2) all of them are >= tail.val. So one assignment finishes the
  merge - there is no need to walk the tail, and no need to write tail.next =
  null anywhere, because the suffix carries its own terminator.
  Edge case the if/else quietly covers: the loop is only entered when both are
  non-null, so on exit at least one is null and the other holds the suffix. The
  exception is when the loop never ran at all - if both inputs were null on
  entry, the else branch sets tail.next = list2 = null, dummy.next is null, and
  the function correctly returns null.
TIE HANDLING
  The comparison is strict: list1.val < list2.val. On equal values control falls
  to the else branch, so ties are taken from list2 first. For this problem that
  is unobservable - the nodes carry only val, so two equal nodes are
  interchangeable. If an interviewer adds a payload and asks for a stable merge
  that keeps list1's element first among equals, the entire fix is changing < to
  <=. Know that this line is the stability knob; do not claim the current code
  is stable in list1's favor.
WATCH OUT
  - This is destructive. Every taken node's next is rewritten, so after the call
  list1 and list2 as the caller held them are no longer valid lists - they alias
  into the merged result. If the original lists must survive, you must allocate
  new nodes and give up the O(1) space.
  - Returning dummy instead of dummy.next prepends a phantom 0 to the answer,
  which passes trivial eyeballing and fails the tests.
  - Do not restructure to while (list1 != null || list2 != null); that forces a
  null check inside both branches for no gain, since the leftover suffix needs
  no per-node work.
  - The recursive formulation (return the smaller head with .next = merge(rest,
  other)) is shorter but adds O(n + m) call frames, which is a real difference
  from this version, not a stylistic one.
TRIGGER
  Reach for a dummy head whenever the head of the result is not known until the
  first comparison, or whenever the head itself might be removed or replaced -
  merge, partition list, remove nth from end, remove duplicates. Reach for the
  two-head comparison whenever the inputs are independently sorted and the
  output must be sorted; this is the merge half of merge sort, and it is also
  the routine that k-way list merging calls, with a heap over the k heads in
  place of the single if.
COMPLEXITY
  Time  : O(n + m)
  Space : O(1)
================================================================================
*/
