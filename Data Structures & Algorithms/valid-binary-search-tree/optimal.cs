// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// #  Recursive post-order with min/max tuple   [tuple-minmax-sentinel]
// #  ties with optimal-variant.cs on O(n) time / O(n) space
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Each node visited once; call stack depth is tree height (O(n) worst
// #  case for skewed tree).
// ##########################################################################

// Sentinel values are safe only because Node.val is restricted to [-1000000000, 1000000000];
// if the value range can reach int.MinValue/MaxValue, use long or nullable bounds.
public class Solution
{
    //my solution
    public bool IsValidBST(TreeNode root)
    {
        var result = IsValidBSTWithMinMax(root);
        return result.found;
    }

    public (int min, int max, bool found) IsValidBSTWithMinMax(TreeNode root)
    {
        //Empty
        if (root == null)
            return (int.MaxValue, int.MinValue, true);

        //Leaf Node
        if (root.right == null && root.left == null)
            return (root.val, root.val, true);

        var left = IsValidBSTWithMinMax(root.left);
        var right = IsValidBSTWithMinMax(root.right);

        // left subtree must be valid and its max < root
        if (!left.found || left.max >= root.val)
            return (0, 0, false);

        if (!right.found || right.min <= root.val)
            return (0, 0, false);

        //we came to here means valid bst
        int min = Math.Min(left.min, root.val);
        int max = Math.Max(right.max, root.val);

        return (min, max, true);
    }
}

/*
================================================================================
 PATTERN : Post-order DFS - each subtree returns (min, max, valid)
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-2.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  result   the tuple from the root call; only .found is used
  left     (min, max, found) summary of the entire left subtree
  right    same summary for the right subtree
  min      smallest value in this subtree, reported to the parent
  max      largest value in this subtree, reported to the parent
  found    true if this subtree is itself a valid BST
WHY THIS PATTERN
  The BST rule is not about a node and its two children; it is about a node and
  every value in its two subtrees. So the check at root needs one number from
  each side: the biggest thing on the left and the smallest thing on the right.
  A post-order walk gives exactly that, because children finish before the
  parent runs, and left.max and right.min arrive already computed. The boolean
  found rides along in the same tuple so one traversal answers both questions.
BRUTE FORCE
  The first thing most people write is: at every node, walk the whole left
  subtree and confirm every value is smaller, then walk the whole right subtree
  and confirm every value is larger, then recurse. That is correct but costs O(n
  * h) - near O(n^2) on a skewed tree - because each node is visited once per
  ancestor. This file pays for each subtree scan only once by summarizing it
  into two integers.
INVARIANT
  Whenever IsValidBSTWithMinMax returns found = true, min and max are the true
  smallest and largest values in that whole subtree, and that subtree is already
  a valid BST. Given that, the parent's two tests left.max >= root.val and
  right.min <= root.val are equivalent to comparing root.val against every
  single descendant. By induction from the null leaves upward, a true at the
  root means the whole tree passed.
SENTINELS FOR THE NULL CHILD
  A null child returns (int.MaxValue, int.MinValue, true) - the min and max are
  deliberately swapped. This makes left.max = int.MinValue lose the >= test and
  right.min = int.MaxValue lose the <= test, so a missing child never blocks the
  parent. It also makes Math.Min(left.min, root.val) and Math.Max(right.max,
  root.val) collapse to root.val on that side. Because of this, the explicit
  leaf branch that returns (root.val, root.val, true) is redundant; deleting it
  gives the same answer through the null path.
WATCH OUT
  The failure return is (0, 0, false), and 0 is a lie about min and max. It is
  safe only because both checks are written as !left.found || ... - the short
  circuit means the garbage 0 is never compared. If someone reorders those
  conditions or hoists the min/max comparison first, a false subtree reporting 0
  can be accepted. Also note >= and <= reject equal values, so a duplicate key
  anywhere in the tree fails, which is what this problem wants but is easy to
  break by accident. Finally the recursion is as deep as the tree, so a long
  one-sided chain can overflow the stack.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it without returning tuples up the tree?
     Yes - pass bounds down instead: helper(node, low, high) checks low <
     node.val < high and recurses with tightened bounds. Same time, but you need
     long or nullable bounds at the root instead of the sentinel trick used
     here.
  2. Remove the recursion entirely.
     Do an in-order traversal with an explicit Stack<TreeNode> and keep a single
     prev variable; the tree is a BST exactly when the in-order sequence is
     strictly increasing. Extra space drops to the stack height and there is no
     call-stack overflow risk.
  3. Can space go below O(h)?
     Morris in-order threading gives O(1) extra space by temporarily rewiring
     right pointers to the in-order successor and restoring them. It mutates the
     tree during the walk, which is unacceptable if other threads read it.
  4. The tree is huge and lives on disk in chunks.
     Keep the same post-order summary, but return only (min, max, found) per
     chunk so a child chunk can be evicted from memory once its three values are
     known - the summary is what makes the algorithm streamable.
TRIGGER
  Reach for this when a node's validity depends on every value in its subtrees,
  and a small fixed summary from each child is enough to decide it.
C# NOTE
  (int min, int max, bool found) is a named ValueTuple, which is a struct, so
  each of the n recursive returns copies three fields on the stack with no heap
  object per node. The named fields also keep left.max and right.min readable,
  which Item1/Item2 would not.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
