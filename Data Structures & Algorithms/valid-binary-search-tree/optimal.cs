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
 PROBLEM : Given the root of a binary tree, return true if it is a valid BST.
           Every node in the left subtree must be strictly smaller than the
           node. Every node in the right subtree must be strictly larger.
           Duplicates fail. [5,4,6,null,null,3,7] -> false, because 3 sits in
           the right subtree of 5.
 PATTERN : DFS post-order (return subtree min/max upward)
================================================================================
IDEA
  IsValidBSTWithMinMax returns (min, max, found) for each subtree.
  A node is valid if both children are valid, left.max < root.val, and
  right.min > root.val. It then returns min(left.min, root.val) and
  max(right.max, root.val). A null child returns (MaxValue, MinValue, true),
  so every check passes. This compares the node with the whole subtree,
  not only with its direct child, so a deep bad node is caught.
EXAMPLE
  [5,4,6,null,null,3,7]: leaf 4 -> (4,4,T); leaves 3,7 -> (3,3,T),(7,7,T)
  node 6: 3<6, 7>6 -> (3,7,T)
  root 5: left.max 4<5 ok; right.min 3<=5 -> (0,0,false) -> answer false
COMPLEXITY
  Time  O(n)  each node is visited once, with O(1) work
  Space O(n)  recursion stack, depth up to n on a skewed tree
PATH TO OPTIMAL
  Scan each whole subtree for every node - O(n^2) - simple, but repeats work.
  Post-order min/max (this file) - O(n) - each subtree summarised once.
  optimal-variant.cs - O(n) - another linear way to do the same check.
KEYWORDS
  binary search tree, validate BST, DFS, post-order, min/max bounds, recursion
WATCH OUT
  - Checking only root.left.val < root.val misses the deep case above.
  - Sentinels: a node with val int.MinValue and no left child fails,
    because left.max (int.MinValue) >= root.val. Use long or nullable.
  - Use >= and <=, not > and <, or duplicates are wrongly accepted.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it top-down instead?
     -> Pass (low, high) bounds down. The left child gets high = val, the
        right child gets low = val. Still O(n) time, O(h) space.
  2. Another approach without bounds?
     -> An inorder traversal of a BST is strictly increasing. Keep prev and
        fail if val <= prev. O(n) time; stops early when it fails.
  3. Can you use O(1) extra space?
     -> Morris inorder traversal uses temporary thread links, not a stack.
        O(n) time, O(1) space, but it changes the tree for a short time.
TRIGGER
  A tree rule that must hold for whole subtrees, not just for parent-child
  pairs.
================================================================================
*/
