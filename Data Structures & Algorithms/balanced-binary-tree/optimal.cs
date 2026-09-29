// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  Post-order DFS with single-pass height computation
// -  [recursive-balance-single-pass]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  Each node visited exactly once, height computed and returned alongside
// -  balance status; worst-case call-stack depth is n for skewed trees.
// --------------------------------------------------------------------------

public class Solution
{
    public bool IsBalanced(TreeNode root)
    {
        return CheckBalance(root).balanced;
    }

    // returns balanced (1 or 0) and height as 2 element int array
    private (bool balanced, int height) CheckBalance(TreeNode node)
    {
        if (node == null)
            return (true, 0);
        var left = CheckBalance(node.left);
        var right = CheckBalance(node.right);
        bool balanced = left.balanced && right.balanced && Math.Abs(left.height - right.height) <= 1;
        int height = 1 + Math.Max(left.height, right.height);
        return (balanced, height);
    }
}

/*
================================================================================
 PATTERN : Tree DFS, Post-order - return (balanced, height) up
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  left      (balanced, height) result for the node.left subtree
  right     (balanced, height) result for the node.right subtree
  balanced  true if this whole subtree is balanced
  height    number of nodes on the longest path from node down to a leaf
WHY THIS PATTERN
  A tree is balanced only if every node's two subtrees differ in height by at
  most 1. So each node needs the heights of both children before it can decide
  anything. That points to post-order DFS: go down into both children first,
  then decide at the parent. CheckBalance returns balanced and height together,
  so each node is visited only once.
BRUTE FORCE
  The first idea most people write works top-down. At each node, call a separate
  Height(node.left) and Height(node.right), compare them, then call IsBalanced
  on both children. This is correct, but it measures the same subtree height
  again at every ancestor. On a skewed tree that costs O(n^2) time, and about
  O(n log n) on a balanced tree. It is slow because height is measured in one
  pass and balance is checked in another.
INVARIANT
  When CheckBalance(node) returns, height is the true height of the subtree
  under node. balanced is true exactly when every node in that subtree meets the
  difference-of-at-most-1 rule. The null case (true, 0) makes this true for
  empty trees. If it is true for both children, then the AND of left.balanced,
  right.balanced and the Math.Abs check makes it true for node, and so does 1 +
  Math.Max for height. By induction, the call on root gives the correct answer.
WATCH OUT
  The comment says the method returns "balanced (1 or 0) and height as 2 element
  int array". The code actually returns a named tuple (bool balanced, int
  height), so the comment is out of date and misleading. The recursion depth
  equals the tree height. A very deep, skewed tree (like a linked list) can
  cause a StackOverflowException, and C# cannot catch that exception. The code
  also does not stop early: after it finds an unbalanced subtree, it still
  visits every other node.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you stop as soon as you find an imbalance?
     Return only an int and use -1 as a sentinel (a special value that means
     "unbalanced"). If either child returns -1, or the heights differ by more
     than 1, return -1 at once. You get the same result with less work on
     unbalanced trees, but "height" and "not balanced" now share one value,
     which is harder to read.
  2. How would you do this without recursion?
     Do an iterative post-order traversal with an explicit Stack<TreeNode> and a
     Dictionary<TreeNode, int> that stores the height of each finished node.
     This removes the call-stack depth limit. The cost is more code and heap
     memory for the dictionary.
  3. What if "balanced" means the heights may differ by at most k?
     Change only the check to Math.Abs(left.height - right.height) <= k. The
     traversal and the complexity stay the same.
TRIGGER
  The answer at a node depends on a value computed from both of its subtrees
  (height, sum, depth), so compute it bottom-up in one post-order pass and
  return several values together.
C# NOTE
  The named tuple (bool balanced, int height) is a ValueTuple, which is a
  struct. So returning two values needs no out parameters and no helper class,
  and callers can read left.height by name.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
