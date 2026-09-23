// --------------------------------------------------------------------------
// -  optimal.cs            O(n + m) time / O(1) space
// -  Two-pointer merge   [two-pointer-merge]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  Each node from both lists is visited exactly once; comparison at each
// -  step determines which node to append.
// --------------------------------------------------------------------------

public class Solution
{
    public ListNode MergeTwoLists(ListNode list1, ListNode list2)
    {
        ListNode dummy = new ListNode(0);
        ListNode node = dummy;

        while (list1 != null && list2 != null)
        {
            if (list1.val < list2.val)
            {
                node.next = list1;
                list1 = list1.next;
            }
            else
            {
                node.next = list2;
                list2 = list2.next;
            }

            node = node.next;
        }

        if (list1 != null)
        {
            node.next = list1;
        }
        else
        {
            node.next = list2;
        }

        return dummy.next;
    }
}

/*
================================================================================
 PATTERN : Two-pointer merge on sorted lists with a dummy head
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  dummy    a throwaway node whose next ends up being the merged head
  node     the tail of the merged list built so far
  list1    the still-unmerged part of the first input list
  list2    the still-unmerged part of the second input list
WHY THIS PATTERN
  Both inputs are already sorted, so the smallest remaining value is always at
  the front of list1 or list2. That means one comparison per step picks the next
  output node, and no sorting or extra storage is needed. The dummy node exists
  so the first append is written the same way as every later one: node.next =
  ..., no special case for an empty result. Because nodes are relinked instead
  of copied, the merge runs without allocating per element.
BRUTE FORCE
  The first thing most people write is: walk both lists, copy every val into a
  List<int>, call Sort(), then build a fresh linked list. That is O((n+m)
  log(n+m)) time and O(n+m) extra space, and it throws away the fact that the
  inputs are already ordered. It also allocates a whole new set of nodes instead
  of reusing the ones handed in.
INVARIANT
  At the top of every loop pass, the chain from dummy.next to node holds all
  values already taken, in sorted order, and every value still in list1 or list2
  is greater than or equal to node.val. So appending the smaller of list1.val
  and list2.val keeps the output sorted. The loop ends when one list is empty;
  the remaining list is sorted and all of it is at least node.val, so a single
  link to it finishes the job.
THE TAIL SPLICE IS ONE POINTER WRITE
  After the loop, at most one list is non-empty, and it is already a correctly
  ordered chain. The code links to it whole instead of walking it node by node.
  The if/else is not really two cases: if list1 is null the else branch assigns
  list2, which may itself be null, and that is the correct terminator anyway.
WATCH OUT
  If both inputs are null the loop never runs, node.next is set to null, and
  dummy.next returns null - correct, but only because the else branch tolerates
  a null list2. This merge is destructive: the next pointers of the input nodes
  are rewritten, so the caller can no longer use list1 or list2 as they were.
  Using < rather than <= means that on a tie the node from list2 is taken first;
  the result is still sorted, but the merge is not stable with respect to list1,
  which matters if nodes carry extra data beyond val. The dummy node is
  allocated on every call even when one list is empty.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Merge k sorted lists instead of two.
     Either fold this method over the k lists pairwise in a tournament, O(N log
     k) total, or push the k heads into a min-heap and pop-and-advance. The
     pairwise fold reuses this exact code and needs no new data structure.
  2. Write it recursively.
     Compare the two heads, set the smaller node's next to the recursive merge
     of the rest, and return that node. It is shorter, but it uses stack depth
     proportional to n+m and can overflow on long lists, which the loop here
     cannot.
  3. The caller must keep the original lists intact.
     Allocate a new ListNode per output element instead of relinking, copying
     val across. Same time, but space grows to O(n+m).
  4. The inputs are sorted descending.
     Flip the comparison to list1.val > list2.val. The invariant argument is
     unchanged, only the ordering relation is reversed.
TRIGGER
  Two or more already-sorted sequences that must become one sorted sequence,
  especially when the container is a linked list and you can move links instead
  of copying values.
C# NOTE
  ListNode is a class, so list1 and node are references; node = node.next just
  moves the reference and no data is copied. Returning dummy.next rather than
  dummy is the whole point of the pattern - and since dummy is a local with no
  other reference to it, it becomes garbage as soon as the method returns.
COMPLEXITY
  Time  : O(n + m)
  Space : O(1)
================================================================================
*/
