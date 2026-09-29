// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
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
 PATTERN : Tree DFS postorder - return subtree min/max upward
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-2.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  result   the (min, max, found) tuple for the whole tree
  found    true if the subtree is a valid BST (the name means "valid", not "found")
  left     the (min, max, found) tuple for root.left's subtree
  right    the (min, max, found) tuple for root.right's subtree
  min      the smallest value in the subtree rooted at root
  max      the largest value in the subtree rooted at root
WHY THIS PATTERN
  A valid BST needs every node in the left subtree to be smaller than the root,
  not only the direct child. The same holds for the right subtree with larger
  values. So each node needs a summary of its whole subtree, and postorder DFS
  (children first, then the node) gives exactly that. Each call returns (min,
  max, found). The parent then only compares left.max and right.min against
  root.val.
BRUTE FORCE
  For every node, walk its entire left subtree and check that all values are
  less than the node. Then walk its entire right subtree and check that all
  values are greater. This is correct, but a node's subtree is scanned again for
  each of its ancestors. On a skewed tree that costs O(n^2) time. This file
  visits each node once because the parent reuses the child's min and max.
INVARIANT
  When a call returns found = true, its subtree is a valid BST, and min and max
  are the true smallest and largest values in it. A node is accepted only if
  both children are valid and left.max < root.val < right.min. That makes every
  left value smaller than the root and every right value larger. The new bounds
  are Math.Min(left.min, root.val) and Math.Max(right.max, root.val), so the
  invariant carries up to the root.
INVERTED RANGE FOR EMPTY SUBTREE
  A null child returns (int.MaxValue, int.MinValue, true). This is an
  "impossible" range where min is larger than max. The checks left.max >=
  root.val and right.min <= root.val then pass on their own when a child is
  missing. Also, Math.Min(left.min, root.val) falls back to root.val. So no
  special case is needed for a node with one child.
WATCH OUT
  The sentinels (fixed stand-in values for "no child") are only safe because of
  the value range stated in the top comment. If root.val is int.MinValue and
  there is no left child, left.max >= root.val is true, and a valid tree is
  rejected. The same happens with int.MaxValue and no right child. The leaf
  shortcut hides this for leaves only, so the same value gives different results
  for a leaf and for a node with one child. Also, recursion depth equals tree
  height, so a very deep, skewed tree can overflow the call stack.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you pass bounds down instead of returning them up?
     Yes. Call Check(node, low, high) and require low < node.val < high. The
     left child gets (low, node.val) and the right child gets (node.val, high).
     Use long or int? bounds (int? can be null) so that no sentinel can clash
     with a real value. This is simpler, and it can stop at the first bad node.
  2. How would you do it without recursion?
     Do an iterative inorder traversal (left, node, right) with an explicit
     stack, and keep the previous value. A BST's inorder order must be strictly
     increasing, so return false when current <= prev. This removes the
     call-stack risk, but the explicit stack still uses O(h) memory (h = tree
     height).
  3. Can it use O(1) extra space?
     Use Morris inorder traversal. It temporarily links each node's inorder
     predecessor back to the node, so no stack is needed. The trade-off is that
     it changes the tree while it runs and must undo every link, even when it
     finds a violation early.
  4. This version checks the right subtree even after the left one fails. Can
  you fix that?
     Return early right after computing left when !left.found or left.max >=
     root.val. Compute right only after that. The worst case stays the same, but
     invalid trees finish sooner.
TRIGGER
  When a tree property depends on every node in a subtree (all smaller, all
  larger, subtree sum or size), return a summary from each child and combine the
  summaries in postorder.
C# NOTE
  The return type (int min, int max, bool found) is a ValueTuple, which is a
  struct, so each call returns its three values with no heap allocation. The
  names min, max and found exist only at compile time, so a clearer name like
  isValid would cost nothing.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
