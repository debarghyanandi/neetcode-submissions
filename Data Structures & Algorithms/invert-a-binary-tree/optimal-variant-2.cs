// --------------------------------------------------------------------------
// -  optimal-variant-2.cs  O(n) time / O(n) space
// --------------------------------------------------------------------------

public class Solution
{
    // Iterative DFS
    public TreeNode InvertTree(TreeNode root)
    {
        if (root == null)
            return null;

        Stack<TreeNode> stack = new Stack<TreeNode>();
        stack.Push(root);
        while (stack.Count > 0)
        {
            TreeNode node = stack.Pop();
            TreeNode leftChild = node.left;
            node.left = node.right;
            node.right = leftChild;
            if (node.left != null)
                stack.Push(node.left);
            if (node.right != null)
                stack.Push(node.right);
        }

        return root;
    }
}

/*
================================================================================
 PATTERN : Tree Traversal (Iterative DFS) - swap children at each node
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-2.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  stack      nodes found but not yet swapped
  leftChild  the node's original left child, saved before node.left is overwritten
WHY THIS PATTERN
  To invert a tree, every node's left and right children must trade places. That
  is one small local change applied to every node, so any full traversal will
  do. This file uses a Stack<TreeNode> to walk the tree depth-first without
  recursion. Each popped node gets its swap, and then its non-null children go
  onto stack.
BRUTE FORCE
  Most people first write the recursive version: swap root.left and root.right,
  then call InvertTree on both children. Its cost is the same as this file. Its
  extra memory is the call stack, which grows with the tree height. It only
  loses on a very deep, skewed tree, where the call stack can overflow. This
  file keeps that depth in a heap-allocated stack instead.
INVARIANT
  Every node popped from stack has its two children swapped exactly once. Every
  node that is still waiting sits in stack with its own children not yet
  touched. Each non-null child is pushed exactly once, by its parent. So when
  stack is empty, every node has been visited and swapped once, and the whole
  tree is mirrored.
WATCH OUT
  The tree is changed in place, and the returned root is the same object that
  was passed in. Any caller that still needs the original tree has lost it. The
  swap depends on saving leftChild first. If you write node.left = node.right
  before saving it, the original left subtree is lost. The early return for root
  == null is required, because otherwise stack.Push(null) would lead to a
  NullReferenceException on node.left.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it level by level instead?
     Yes. Replace Stack<TreeNode> with Queue<TreeNode> and use Enqueue/Dequeue.
     The swap stays the same. The peak memory becomes the widest level instead
     of about the tree height, so a bushy tree costs more and a skewed tree
     costs less.
  2. How would you check that a tree is symmetric (a mirror of itself)?
     Do not invert it. Walk two pointers at the same time: compare a.left with
     b.right and a.right with b.left, using a stack of pairs. This only reads
     the tree, and it can stop at the first mismatch.
  3. Does the order in which nodes are visited matter here?
     No. Each swap only touches one node's own two pointers, so preorder,
     postorder, or BFS all give the same result. Only the peak size of the
     container changes.
TRIGGER
  When the same local change must be done at every node of a tree, and no node
  needs results from its subtrees, pick any traversal and apply the change on
  visit.
C# NOTE
  The leftChild temp can be replaced by a tuple swap: (node.left, node.right) =
  (node.right, node.left). This does the same exchange in one line, and you
  cannot get the order wrong.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
