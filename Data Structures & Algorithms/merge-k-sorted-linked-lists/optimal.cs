// --------------------------------------------------------------------------
// -  optimal.cs            O(n log k) time / O(log k) space
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
  l, r     the range lists[l..r] that this Divide call must merge into one list
  mid      the split point; the left half is lists[l..mid] and the right half is lists[mid+1..r]
  left     the merged result of lists[l..mid]
  right    the merged result of lists[mid+1..r]
  dummy    a fake head node, so the first real node needs no special case
  curr     the tail of the merged list that Conquer is building
WHY THIS PATTERN
  The problem gives k lists that are each already sorted and asks for one sorted
  list. Merging two sorted lists is easy and linear, so the task becomes: how do
  we pair up the merges? Divide splits the range at mid, merges each half, then
  calls Conquer(left, right). Each node takes part in only about log k merges.
BRUTE FORCE
  The simple approach merges the lists one by one into a growing result: merge
  lists[0] with lists[1], then merge that result with lists[2], and so on. It is
  correct, but the result gets longer every time, so early nodes are walked
  again in every later merge. That costs O(n * k) in total. Pairwise merging
  keeps the sizes balanced, and that is the whole gain.
INVARIANT
  Divide(lists, l, r) always returns one sorted list that holds every node from
  lists[l..r]. The base cases are true: an empty range gives null, and one list
  is already sorted. Inside Conquer, the list from dummy.next to curr is always
  sorted, and it holds the smallest nodes taken so far from l1 and l2. This is
  because we always take the smaller of the two heads. When one list runs out,
  the rest of the other list is sorted and not smaller than curr, so we attach
  it whole.
WATCH OUT
  The code reuses the input nodes by changing their next pointers. After the
  call, the original lists are destroyed, so a caller who still needs them will
  get wrong data. The "l > r" guard in Divide never runs from MergeKLists,
  because the Length == 0 case returns early. Keep it anyway, because it
  protects the method if another caller uses it. Null entries inside lists are
  safe, because Conquer treats a null list as empty. The comment "//Not
  solved.." is only a personal note, not a claim about the code.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you remove the recursion and get O(1) extra space?
     Yes. Merge bottom-up: first merge pairs at distance 1, then 2, then 4, and
     store each result back into lists. The time is the same, there is no call
     stack, and the code is a little harder to read.
  2. How would you solve it with a heap (a structure that always gives you the
  smallest item fast)?
     Put the head of each list into a min-heap with k slots. Pop the smallest
     node, append it, and push its next node. The time is still O(n log k) and
     the space is O(k). The heap version also works when the lists arrive as
     streams.
  3. What if duplicates must be removed from the output?
     In Conquer, skip a node when its val equals curr.val before you attach it
     (and do not compare against the dummy). Merging still works the same way.
TRIGGER
  When you see "k sorted inputs to combine into one sorted output", think of
  balanced pairwise merging or a k-sized min-heap.
C# NOTE
  ListNode is a reference type, so "curr.next = l1" only moves a pointer and
  copies nothing. The only new object is the one dummy node made in each Conquer
  call.
COMPLEXITY
  Time  : O(n log k)
  Space : O(log k)
================================================================================
*/
