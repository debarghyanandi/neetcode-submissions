// --------------------------------------------------------------------------
// -  optimal.cs            O(n log k) time / O(log m) space
// -  Divide and conquer pairwise merge   [divide-conquer-merge]
// -  ranks above suboptimal.cs (O(n log k) time / O(k) space)
// -
// -  Reference solution - not one you solved yourself (from submission-2)
// -
// -  Recursively divides k lists in half and merges pairs bottom-up; log k
// -  recursion depth processes all n nodes
// --------------------------------------------------------------------------

public class Solution
{
    public ListNode MergeKLists(ListNode[] lists)
    {
        //Not solved..
        if (lists == null || lists.Length == 0)
        {
            return null;
        }
        return Divide(lists, 0, lists.Length - 1);
    }

    private ListNode Divide(ListNode[] lists, int l, int r)
    {
        if (l > r)
        {
            return null;
        }
        if (l == r)
        {
            return lists[l];
        }

        int mid = l + (r - l) / 2;
        ListNode left = Divide(lists, l, mid);
        ListNode right = Divide(lists, mid + 1, r);

        return Conquer(left, right);
    }

    private ListNode Conquer(ListNode l1, ListNode l2)
    {
        ListNode dummy = new ListNode(0);
        ListNode curr = dummy;

        while (l1 != null && l2 != null)
        {
            if (l1.val <= l2.val)
            {
                curr.next = l1;
                l1 = l1.next;
            }
            else
            {
                curr.next = l2;
                l2 = l2.next;
            }
            curr = curr.next;
        }

        if (l1 != null)
        {
            curr.next = l1;
        }
        else
        {
            curr.next = l2;
        }

        return dummy.next;
    }
}

/*
================================================================================
 PATTERN : Divide and Conquer - pairwise merge of sorted lists
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-2.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  lists    array of k sorted list heads; lists[i] is the head of one list
  l, r     inclusive range of list indices this Divide call owns
  mid      split point, computed as l + (r - l) / 2
  dummy    throwaway head node so the merge loop never special-cases the first link
  curr     tail of the merged list being built
WHY THIS PATTERN
  The input is k lists that are each already sorted, so the only work left is
  combining them. Merging two sorted lists is a clean linear pass (Conquer), and
  merging is associative, so you can pair them up instead of folding them one by
  one. Divide splits the index range [l, r] in half until a single list remains,
  then merges on the way back up, so every node is touched once per level and
  there are log k levels.
BRUTE FORCE
  The first thing most people write is a running merge: start with lists[0],
  then merge lists[1] into it, then lists[2], and so on. That is correct but the
  accumulated list grows each time, so the first list's nodes get walked k times
  - O(n*k). The other common first attempt is to dump every val into a
  List<int>, sort it, and rebuild the chain: O(n log n) time but O(n) extra
  memory and it throws away the fact that each input list is already sorted.
INVARIANT
  Divide(lists, l, r) returns the head of a single sorted list containing
  exactly the nodes of lists[l..r]. That holds at the base cases (null for an
  empty range, lists[l] untouched for one list, both already sorted), and
  Conquer preserves it: at every loop step, dummy.next..curr is sorted and every
  value in it is less than or equal to both l1.val and l2.val, because it always
  attaches the smaller head. So the top-level call returns all nodes of the
  whole array in sorted order.
NODES ARE RELINKED, NOT COPIED
  Conquer never allocates a node for the data - it only rewires next pointers on
  the existing nodes, plus one dummy per merge. That is why the extra space is
  just the recursion stack and not proportional to the node count. The caller's
  lists array is left pointing at nodes whose next chains have been rewritten,
  so the original lists are destroyed.
WATCH OUT
  The comment "//Not solved.." sits above working code - the method is complete
  and correct, so the comment is stale and misleading. The l > r base case can
  only fire from the top-level call when lists.Length is 0, but that is already
  caught by the guard in MergeKLists; inside Divide, mid is always >= l and mid
  + 1 is always <= r, so neither half is ever empty. An element of lists may
  itself be null (an empty list), and that is fine: Conquer handles a null l1 or
  l2 through the tail copy, and Divide can return it as-is. Recursion depth is
  log k, which is small, but this is still real stack use if k is huge.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Remove the recursion - how?
     Loop bottom-up: while lists.Length > 1, walk i from 0 to length step 2 and
     write Conquer(lists[i], lists[i+1]) back into lists[i/2], halving the live
     count each pass. Same time, O(1) extra space, but it mutates the caller's
     array.
  2. Why not a min-heap of the k heads instead?
     Pop the smallest head, append it, push its next - same O(n log k) time, but
     O(k) space for the heap instead of O(log k) stack. The heap wins when lists
     arrive as a stream and you cannot index them; this file wins when the array
     is fully known up front.
  3. What if the lists were not individually sorted?
     Divide and conquer buys you nothing, since Conquer's correctness depends on
     each input being sorted. Collect all values, sort, and rebuild - O(n log
     n).
  4. What if each list is huge but k is 2?
     This degenerates to a single Conquer call, O(n) time with no recursion
     beyond one level - already optimal; there is nothing to improve.
TRIGGER
  Several already-sorted sequences that must become one sorted sequence - pair
  them up instead of folding them one at a time.
C# NOTE
  ListNode dummy = new ListNode(0) relies on the ListNode(int) constructor that
  LeetCode's C# stub provides; if your local definition only has a parameterless
  constructor this will not compile, and the 0 value is never read since the
  method returns dummy.next.
COMPLEXITY
  Time  : O(n log k)
  Space : O(log m)
================================================================================
*/
