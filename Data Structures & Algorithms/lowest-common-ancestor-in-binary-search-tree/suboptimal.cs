// ##########################################################################
// #  suboptimal.cs         O(n) time / O(n) space
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
 PROBLEM : You get the root of a binary search tree (BST) and two nodes p and
           q in it. Return their lowest common ancestor: the deepest node that
           has both p and q in its subtree. A node counts as its own ancestor.
           Example: tree [6,2,8,0,4,7,9,null,null,3,5], p=2, q=4 -> 2.
 PATTERN : BST property + recursive DFS down one path
================================================================================
IDEA
  Put the two values in order as min and max. If root.val lies between them
  (inclusive), p and q split here or one of them is root, so return root.
  If min > root.val, both are bigger, so recurse into root.right. Otherwise
  both are smaller, so recurse into root.left. The code differs from
  optimal.cs only in using recursion instead of a loop.
EXAMPLE
  Tree [6,2,8,0,4,7,9,null,null,3,5]. p=3, q=5: min=3, max=5.
  6: 5>=6 fails, 3>6 fails -> left. 2: 5>=2, 3<=2 fails; 3>2 -> right.
  4: 5>=4 and 3<=4 -> return 4. With p=2, q=4, node 2 returns at once.
COMPLEXITY
  Time  O(n)  one node per level on a single root-to-node path, up to n levels
  Space O(n)  recursion stack as deep as that path, n calls in a skewed tree
WATCH OUT
  - Keep >= and <=. With strict < and >, p=2, q=4 walks past node 2, where
    p itself is the answer, and it returns the wrong node.
  - No null check: if p or q is not in the tree, root becomes null and
    root.val throws NullReferenceException.
  - A skewed tree (sorted inserts) gives very deep recursion and can cause a
    stack overflow. The loop in optimal.cs avoids this.
================================================================================
*/
