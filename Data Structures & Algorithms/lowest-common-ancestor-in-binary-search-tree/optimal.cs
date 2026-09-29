// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public TreeNode LowestCommonAncestor(TreeNode root, TreeNode p, TreeNode q)
    {
        TreeNode node = root;

        while (node != null)
        {
            if (p.val > node.val && q.val > node.val)
            {
                node = node.right;
            }
            else if (p.val < node.val && q.val < node.val)
            {
                node = node.left;
            }
            else
            {
                return node;
            }
        }
        return null;
    }
}

/*
================================================================================
 PROBLEM : You get the root of a binary search tree (BST) and two nodes p and
           q that are in the tree. Return their lowest common ancestor (LCA):
           the deepest node that has both p and q in its subtree. A node
           counts as its own ancestor. Example: BST
           [6,2,8,0,4,7,9,null,null,3,5], p=2, q=8 -> 6.
 PATTERN : BST property descent (iterative, one path down)
================================================================================
IDEA
  Start at root and walk down with one pointer, node. If p.val and q.val
  are both bigger than node.val, the LCA is in the right subtree, so move
  right. If both are smaller, move left. Otherwise p and q split here, or
  one of them is node itself, so node is the answer. This is correct
  because below the split point, no single subtree holds both p and q.
EXAMPLE
  Same tree, p=3, q=5: node=6, both < 6 -> left; node=2, both > 2 -> right;
  node=4, 3 < 4 < 5, split -> return 4.
  Tricky case, p=2, q=4: node=6 -> left; node=2, p.val == 2 so neither
  branch fires -> return 2 (a node is its own ancestor).
COMPLEXITY
  Time  O(n)  one step per level, height h, which is n on a skewed tree
  Space O(1)  only the node pointer, no recursion stack
PATH TO OPTIMAL
  Root-to-p and root-to-q paths, then compare them - O(n) time and space.
  Generic tree LCA by DFS - O(n) time, O(h) stack - ignores BST order.
  BST descent with recursion - O(h) time, O(h) stack - see suboptimal.cs.
  Same descent as a while loop - O(h) time, O(1) space - this file.
KEYWORDS
  lowest common ancestor, BST, binary search tree, split point, iterative
WATCH OUT
  - Use strict > and <. With >= the code steps past node when node == p,
    and then it misses the answer.
  - It assumes p and q exist. If q is missing, it still returns a node,
    not null. The final return null only happens when root is null.
  - Duplicate values break the BST rule, and the descent can go wrong.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. What if the tree is a plain binary tree, not a BST?
     -> Use postorder DFS. Return the node if it is p or q, and return node
        when both sides are non-null. O(n) time, O(h) stack.
  2. What if p or q might not be in the tree?
     -> Find the split node as here, then search down from it for both p and
        q. Return null if one is missing. Still O(h) time, O(1) space.
  3. Nodes have parent pointers but you do not get the root?
     -> Walk up from p and q like two pointers on linked lists that meet at an
        intersection. O(h) time, O(1) space.
  4. Many LCA queries on the same big tree?
     -> Precompute binary lifting (jump tables of 2^k ancestors). Each query
        is then O(log n), after O(n log n) time and space of setup.
TRIGGER
  A BST question about two nodes: walk down until they go different ways.
================================================================================
*/
