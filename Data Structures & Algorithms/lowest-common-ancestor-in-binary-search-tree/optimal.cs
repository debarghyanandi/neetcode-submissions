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
 PATTERN : BST Descent - walk down until p and q split
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  node     the current node on the single path down from root; the answer once p and q split here
WHY THIS PATTERN
  The tree is a binary search tree (BST): every value in the left subtree is
  smaller than the node, and every value in the right subtree is larger. So
  p.val and q.val alone tell you which side each target is on, and you never
  need to search. While both are on the same side, the lowest common ancestor
  (LCA) is also on that side, so node moves there. The first node where they do
  not go the same way is the LCA.
BRUTE FORCE
  Ignore the BST order and use the general binary-tree LCA. Either recurse
  through the whole tree and return the node where p and q are found in
  different subtrees, or record the root-to-p path and the root-to-q path and
  take the last node the two paths share. Both take O(n) time. They also need
  O(h) recursion stack or O(n) path storage, where h is the tree height. They
  lose because they explore many nodes or keep memory, while this file follows
  one path with a single pointer.
INVARIANT
  At the top of every loop pass, node is an ancestor of both p and q, or is one
  of them. This is true at root. It stays true when we move right because both
  values are larger than node.val, so both targets are in the right subtree, and
  the same holds for the left. When the else branch runs, p and q are on
  different sides of node, or one of them is node. No lower node can have both
  below it, so node is the lowest common ancestor.
EQUALITY FALLS INTO ELSE
  Both comparisons are strict (> and <). So when node.val equals p.val or q.val,
  neither move condition is true and the code returns node. This is correct,
  because the problem counts a node as an ancestor of itself. If you change
  either test to >= or <=, the walk moves past the target and gives a wrong
  answer.
WATCH OUT
  The code assumes p and q are both in the tree. If one is missing, it still
  returns the split node, not null. The final return null only happens when root
  is null or when the walk runs off the tree. The code also assumes the values
  are unique. If there are duplicates, a value equal to node.val can be in
  either subtree, and the split logic stops being correct.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you write it recursively?
     Yes. If both values are larger, recurse right. If both are smaller, recurse
     left. Otherwise return root. The code is shorter, but it uses O(h) call
     stack where the loop uses none.
  2. What if the tree is a plain binary tree, not a BST?
     You can no longer pick a direction from the values. Use a post-order
     recursion: return the node where the left and right calls both return
     non-null, otherwise pass up whichever side is non-null. It visits every
     node and uses O(h) stack.
  3. What if p or q might be missing from the tree?
     First search the BST for each of them, then run the split walk only if both
     are found. The cost is still one path per search, so the extra time is
     small.
  4. What if there are many LCA queries on the same fixed tree?
     Preprocess once with binary lifting (a table that stores each node's 2^k-th
     ancestor) or with an Euler tour plus range-minimum queries. Each query then
     costs O(log n) or O(1), in exchange for O(n log n) or O(n) extra memory.
TRIGGER
  The problem asks for an ancestor or a meeting point of two nodes in a BST:
  compare both values to the current node and follow the side they share.
C# NOTE
  The method can return null, so with nullable reference types turned on the
  honest return type is TreeNode?. That makes callers handle the missing-node
  case the compiler would otherwise hide.
COMPLEXITY
  Time  : O(n)
  Space : O(1)
================================================================================
*/
