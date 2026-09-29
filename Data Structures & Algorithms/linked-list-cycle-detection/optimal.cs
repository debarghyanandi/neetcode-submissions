// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public bool HasCycle(ListNode head)
    {
        ListNode slow = head;
        ListNode fast = head;
        while (fast != null && fast.next != null)
        {
            slow = slow.next;
            fast = fast.next.next;
            if (fast == slow)
                return true;
        }
        return false;
    }
}

/*
================================================================================
 PROBLEM : You get the head of a singly linked list. Return true if some
           node's next pointer leads back to an earlier node (a cycle), else
           false. Nodes are compared by identity, not by value. Example: 1 ->
           2 -> 3 -> 4 -> back to 2 -> true; 1 -> 2 -> null -> false.
 PATTERN : Fast and Slow Pointers (Floyd's cycle detection)
================================================================================
IDEA
  slow moves one node per step and fast moves two nodes per step.
  With no cycle, fast reaches null first and the loop exits with false.
  With a cycle, both pointers end up inside the loop. There the gap between
  them shrinks by exactly 1 each step, so fast can never jump over slow.
  They must meet (fast == slow), and the code returns true.
EXAMPLE
  List 1 -> 2 -> 3 -> 4, and 4.next = 2. Start: slow=1, fast=1.
  Step 1: slow=2, fast=3. Step 2: slow=3, fast=2 (3 -> 4 -> 2).
  Step 3: slow=4, fast=4 (2 -> 3 -> 4). They meet, so the answer is true.
COMPLEXITY
  Time  O(n)  fast gains 1 node on slow per step, so they meet within ~n steps
  Space O(1)  only two pointer variables, slow and fast
PATH TO OPTIMAL
  HashSet of visited nodes - O(n) time / O(n) space - simple, one pass.
  Floyd fast/slow (optimal.cs) - O(n) / O(1) - no extra memory needed.
  (The HashSet step has no sibling file in this folder.)
KEYWORDS
  linked list, cycle detection, Floyd, tortoise and hare, fast slow pointers
WATCH OUT
  - Compare fast == slow only AFTER moving. Both start at head, so a check
    before the first move returns true on every list.
  - Keep "fast != null" before "fast.next != null". Swapping them throws
    a NullReferenceException on even-length lists.
  - Compare node references, not .val. Duplicate values do not mean a cycle.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Where does the cycle start? (LeetCode 142)
     -> After they meet, reset one pointer to head. Move both one step at a
        time. They meet again at the cycle entry. Still O(n) time / O(1) space.
  2. How long is the cycle?
     -> From the meeting point, keep slow still and step once around the loop,
        counting until you are back at slow. This adds O(cycle length) time.
  3. Why can't fast skip over slow?
     -> Inside the loop, each step closes the gap by exactly 1. It goes k,
        k-1, ..., 0, so the gap must hit 0. It never goes negative.
  4. Can you do it without Floyd?
     -> Put each node in a HashSet and return true on a repeat. It is O(n)
        space and easier to explain, but it uses more memory.
TRIGGER
  When you follow "next" links (a list, or x -> f(x)) and must detect a loop
  using O(1) memory, use fast and slow pointers.
================================================================================
*/
