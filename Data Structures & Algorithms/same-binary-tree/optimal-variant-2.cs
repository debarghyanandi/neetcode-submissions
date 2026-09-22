// --------------------------------------------------------------------------
// -  optimal-variant-2.cs  O(n) time / O(n) space
// -  Iterative BFS level-order traversal   [iterative-bfs-level-order]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  Queue stores widest level of tree; worst case O(n) for complete binary
// -  tree.
// --------------------------------------------------------------------------

public class Solution
{
    public bool IsSameTree(TreeNode p, TreeNode q)
    {
        var queueP = new Queue<TreeNode>(new[] { p });
        var queueQ = new Queue<TreeNode>(new[] { q });

        while (queueP.Count > 0 && queueQ.Count > 0)
        {
            for (int i = queueP.Count; i > 0; i--)
            {
                var nodeP = queueP.Dequeue();
                var nodeQ = queueQ.Dequeue();

                if (nodeP == null && nodeQ == null)
                    continue;
                if (nodeP == null || nodeQ == null || nodeP.val != nodeQ.val)
                {
                    return false;
                }

                queueP.Enqueue(nodeP.left);
                queueP.Enqueue(nodeP.right);
                queueQ.Enqueue(nodeQ.left);
                queueQ.Enqueue(nodeQ.right);
            }
        }

        return true;
    }
}

/*
================================================================================
 PATTERN : BFS level order on two trees in lockstep
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-2.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  queueP   BFS frontier for tree p, with null children kept as placeholders
  queueQ   BFS frontier for tree q, same positions as queueP
  nodeP    the node pulled from queueP this step; may be null
  nodeQ    the node from queueQ that sits at the exact same position as nodeP
WHY THIS PATTERN
  The question asks whether two trees match in both value and shape, so every
  node in p needs a partner at the same position in q. Walking both trees in the
  same order and comparing the pair at each step answers this directly: queueP
  and queueQ are filled and drained in the same sequence, so nodeP and nodeQ
  always describe the same slot in the two trees. The first slot where the pair
  disagrees is proof the trees differ, and if no slot disagrees they are
  identical.
BRUTE FORCE
  The first thing many people write is to serialize each tree to a preorder
  string and compare the two strings. That is also O(n) time and space, but it
  is easy to get wrong: if you do not print explicit null markers, two different
  shapes such as a left chain and a right chain produce the same string and you
  return true for unequal trees. This file compares node pairs directly, so no
  encoding step can lose information.
INVARIANT
  At the top of every for-loop pass, queueP and queueQ hold the same number of
  entries, and entry k of one is the partner of entry k of the other; every pair
  already dequeued matched. The equal-length part holds because each pair either
  enqueues nothing (both null) or enqueues exactly two children into each queue.
  So when the loop ends with nothing left to compare, every position in both
  trees was checked and matched, and true is correct.
NULLS ARE PUSHED ON PURPOSE
  The code enqueues nodeP.left and nodeP.right even when they are null. That is
  what makes shape differences visible: a missing child in p meets a real child
  in q, and the nodeP == null || nodeQ == null test fires. If you filtered nulls
  out before enqueueing, the queues would drift out of alignment and two trees
  with the same values in different shapes could pass.
WATCH OUT
  The level size is snapshotted from queueP only (int i = queueP.Count), and the
  while test uses && on both counts; both are safe only because of the
  equal-length invariant above, so any future edit that enqueues a different
  number of items into the two queues silently breaks the pairing instead of
  failing loudly. Memory is not just the node count: a full bottom level of null
  placeholders sits in both queues at once, roughly doubling the peak. If
  nullable reference types are switched on in the project, new[] { p } and the
  enqueues of possibly-null children will produce compiler warnings, because
  Queue<TreeNode> is declared as non-null here.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do this without the queues?
     Recursive DFS: return p and q both null, or both non-null with equal val
     and matching left and right. Same O(n) time, and the stack is O(h) instead
     of a full level, but a long skewed tree can still overflow the stack, which
     this iterative version cannot.
  2. How would you cut the bookkeeping to one queue?
     Use a single Queue of value tuples, Queue<(TreeNode, TreeNode)>, and
     enqueue (nodeP.left, nodeQ.left) and (nodeP.right, nodeQ.right). The
     pairing becomes structural instead of an unwritten rule, and the
     level-by-level for loop is no longer needed since order alone is enough.
  3. Now check whether q is a subtree of p, not equal to it.
     Walk p and call this comparison at each node whose value equals q's root,
     giving O(n*m) worst case. To do better, serialize both trees with null
     markers and run a substring search such as KMP for O(n+m).
  4. What changes for "is this one tree symmetric"?
     Start both queues from root.left and root.right, and enqueue the children
     in mirrored order: left child of one against right child of the other.
TRIGGER
  Two trees (or two halves of one tree) must agree position by position,
  including where children are missing.
C# NOTE
  new Queue<TreeNode>(new[] { p }) goes through the IEnumerable constructor and
  allocates a one-element array just to seed the queue; var queueP = new
  Queue<TreeNode>(); queueP.Enqueue(p); says the same thing without the array.
  Queue<T> places no restriction on null elements, which is exactly why the
  placeholder trick compiles cleanly here.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
