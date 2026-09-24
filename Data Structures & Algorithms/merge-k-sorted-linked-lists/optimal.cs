// --------------------------------------------------------------------------
// -  optimal.cs            O(n log k) time / O(log k) space
// -  Divide and conquer merge   [divide-conquer-merge]
// -  ranks above suboptimal.cs (O(n log k) time / O(k) space)
// -
// -  Reference solution - not one you solved yourself
// -
// -  Recursively divides k lists in half, merging pairs at each of log(k)
// -  recursion levels.
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
 PATTERN : Divide and Conquer - pairwise merge of k sorted lists
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-2.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  lists    the k list heads; nodes get rewired in place
  mid      split point of the index range [l, r], written as l + (r - l) / 2
  dummy    throwaway head node so the merge loop never special-cases the first link
  curr     tail of the merged list being built; always the last node attached
WHY THIS PATTERN
  The input is k lists that are each already sorted, so the only work left is
  interleaving them. Merging two sorted lists is a clean linear pass (Conquer),
  and merging is associative, so Divide can split the index range [l, r] in
  half, solve each half, and merge the two results. Halving means each node is
  copied forward only log k times instead of k times.
BRUTE FORCE
  The first thing most people write is a running accumulator: start with
  lists[0] and fold in lists[1], then lists[2], and so on with the same two-list
  merge. That is correct but the growing accumulator is re-walked on every
  merge, giving O(n k) time. Another honest first try is to collect every value
  into a List<int>, sort it, and rebuild the chain - O(n log n) time but O(n)
  extra memory and it throws away the fact that each input is already sorted.
INVARIANT
  In Conquer, every node from dummy up to curr is already in sorted order, and
  curr.next is the only loose end; l1 and l2 always point at the smallest
  not-yet-taken node of their own list. Because each step attaches the smaller
  of the two heads, the sorted prefix can never be violated. In Divide, the
  returned node is the head of a fully sorted list containing exactly the nodes
  of lists[l..r], which is what the level above needs to keep its own merge
  correct.
THE TAIL SHORTCUT
  When the loop ends, at most one list still has nodes, and those nodes are
  already sorted and all larger than everything attached so far. So the code
  links the whole remainder with a single assignment instead of looping. If both
  are exhausted, curr.next = l2 stores null, which correctly terminates the
  chain.
WATCH OUT
  This is destructive: the next pointers of the input nodes are rewritten, so
  lists no longer describes the original k lists after the call, and calling
  MergeKLists twice on the same array gives wrong results. A null entry inside
  lists is handled, because Divide returns lists[l] as-is and Conquer treats a
  null head as an empty list - but only the whole-array null and empty cases are
  checked up front. The l > r branch in Divide is dead code given the l == r
  base case and a non-empty array; harmless, but do not rely on it as the real
  guard. Recursion depth is log k, so stack space is fine, yet the helper
  methods are instance methods and cannot be called without a Solution object.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do this without recursion?
     Yes - loop over the array merging lists[i] with lists[i + interval],
     doubling interval from 1 each round until interval >= lists.Length; same
     time, and it removes the call stack at the cost of overwriting slots in
     lists.
  2. What changes if you use a min-heap instead?
     Push all k heads into a PriorityQueue keyed by val, pop the smallest,
     append it, push its next. Same O(n log k) time, but O(k) extra space
     instead of O(log k), and it is the better fit when the lists arrive as a
     stream and you cannot index them.
  3. What if k is huge but each list is very short?
     Divide and conquer still holds; the cost is dominated by the k - 1 merge
     calls and the log k levels, so avoid the heap version, which would hold all
     k heads in memory at once.
  4. What if the lists are sorted descending?
     Flip the comparison in Conquer to l1.val >= l2.val; nothing else changes,
     since Divide never looks at values.
TRIGGER
  Several inputs that are each already sorted and must become one sorted output
  - merge them pairwise in a tree, not one after another.
C# NOTE
  Each Conquer call allocates one dummy ListNode, so k - 1 throwaway nodes are
  created overall; a version that picks the smaller head first and then loops
  avoids them, at the cost of an extra branch before the loop.
COMPLEXITY
  Time  : O(n log k)
  Space : O(log k)
================================================================================
*/
