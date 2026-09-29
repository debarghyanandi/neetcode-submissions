// ##########################################################################
// #  suboptimal.cs         O(n) time / O(n) space
// #  BST traversal, recursive descent   [bst-recursive]
// #  ranks below optimal.cs (O(n) time / O(1) space)
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Recursively traverses BST using the same property; call stack depth
// #  equals tree height, O(n) worst case for skewed tree.
// ##########################################################################

public class Solution
{
    public TreeNode LowestCommonAncestor(TreeNode root, TreeNode p, TreeNode q)
    {
        //My solution
        int min = Math.Min(p.val, q.val);
        int max = Math.Max(p.val, q.val);

        if (max >= root.val && min <= root.val)
            return root;

        if (min > root.val)
        {
            return LowestCommonAncestor(root.right, p, q);
        }
        else
            return LowestCommonAncestor(root.left, p, q);
    }
}

/*
================================================================================
 PATTERN : BST Search - walk down to the split point
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Suboptimal
================================================================================
VARIABLES
  min  the smaller of p.val and q.val
  max  the larger of p.val and q.val
WHY THIS PATTERN
  The tree is a BST (binary search tree): every value on the left is smaller
  than the node, and every value on the right is larger. So one comparison with
  root.val tells you which side p and q are on. If both are larger, the answer
  is on the right. If both are smaller, it is on the left. The first node where
  they split is the lowest common ancestor.
BETTER APPROACH
  The better approach is the same logic in a while loop: move root to root.left
  or root.right until the values split, then return it. That needs O(1) extra
  space. This file recurses once per level, so the call stack grows with the
  height of the tree. On a tree shaped like a chain, that height is n. The loop
  needs no stack, and the code is just as short.
INVARIANT
  At every call, p and q are both inside the subtree of the current root. When
  min > root.val, both nodes are in the right subtree, so every ancestor they
  share is in there too. The same holds on the left when max < root.val. The
  first root with min <= root.val <= max is the deepest node that still holds
  both p and q in its subtree, so it is the answer.
EQUALITY MEANS A NODE CAN BE ITS OWN ANCESTOR
  The split test uses >= and <=, not > and <. If root.val equals p.val, then
  root is p, and p is an ancestor of q. So root must be returned right there.
  With strict < and > the code would step past p and search a subtree that does
  not contain it.
WATCH OUT
  There is no null check. If p or q is not in the tree, the search walks off a
  leaf and root.left or root.right is null. The next call then reads root.val
  and throws a NullReferenceException. The code also compares values, not node
  references, so it assumes all values in the BST are unique. The final else
  branch really means "max < root.val". That is only true because the two
  earlier checks have already failed, so be careful if you reorder them.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if the tree is a plain binary tree, not a BST?
     Values no longer tell you which side to go. Search both subtrees. If each
     side finds one of p or q, the current node is the answer. Otherwise, return
     whichever side found something. This takes O(n) time because you may visit
     every node.
  2. What if p or q might not be in the tree?
     First confirm that both exist, using two BST searches of O(h) time each,
     and return null if either is missing. Or track in the walk whether each
     node was actually matched. It costs an extra pass but avoids a wrong
     answer.
  3. What if you must answer many LCA queries on the same tree?
     Preprocess once with binary lifting (store the 2^k-th ancestor of each
     node) or with an Euler tour plus a range-minimum structure. Each query then
     takes O(log n) or O(1). The trade-off is O(n log n) or O(n) extra memory
     and more setup work.
TRIGGER
  The tree is a BST and you need the node where two values stop sharing a path
  from the root.
C# NOTE
  Math.Min and Math.Max are called again in every recursive call, but p and q
  never change. In the iterative version, you compute min and max once before
  the loop.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
