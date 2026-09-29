// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// #  Recursive depth-first search   [recursive-dfs]
// #  ties with optimal-variant.cs on O(n) time / O(n) space
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Recursively visits each of n nodes; call stack depth equals tree
// #  height, worst case O(n) in a skewed tree.
// ##########################################################################

public class Solution
{
    // my solution
    public int MaxDepth(TreeNode root)
    {
        if (root == null)
            return 0;
        return Math.Max(MaxDepth(root.left), MaxDepth(root.right)) + 1;
    }
}

/*
================================================================================
 PATTERN : Tree DFS (post-order) - depth = 1 + max of child depths
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-2.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  root  the node of the current subtree; null means an empty subtree with depth 0
WHY THIS PATTERN
  The depth of a tree is defined through its subtrees: it is one more than the
  deeper of the left and right subtrees. A definition like this, built from
  itself, fits recursion directly. MaxDepth(root.left) and MaxDepth(root.right)
  solve the two smaller problems. The current call adds 1 for root itself.
BRUTE FORCE
  A first try might list every root-to-leaf path as its own list of nodes and
  then take the longest list. The answer is correct, but copying each path costs
  up to h nodes, where h is the tree height. That is O(n*h) time and memory, and
  O(n^2) on a tree shaped like a chain. It loses because you only need the
  length of each path, not the nodes on it.
INVARIANT
  Each call to MaxDepth(root) returns the exact number of nodes on the longest
  downward path that starts at root. For null this is 0, which is correct. For a
  real node, if both child calls are correct, then the longest path goes through
  the deeper child, so Math.Max(...) + 1 is correct too. By induction on subtree
  size, the top call returns the depth of the whole tree.
WATCH OUT
  The code counts nodes, not edges. So a single node returns 1. If the problem
  defines depth in edges, the answer is off by one. The recursion goes as deep
  as the tree is tall. On a very skewed tree, for example a long chain of only
  left children, it can throw a StackOverflowException, and a try/catch cannot
  recover from that. The comment "my solution" says nothing about the method, so
  it is not wrong, but it adds nothing.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it without recursion?
     Yes. Use a BFS (level-order walk) with a Queue<TreeNode>. Take one full
     level from the queue at a time and add 1 to a counter for each level.
     Memory is then the widest level, not the tallest path. Or use an explicit
     stack of (node, depth) pairs and keep the largest depth seen.
  2. How would you find the minimum depth instead?
     You cannot just change Max to Min. A node with only one child would then
     count the null side as depth 0. Only recurse into the child that is not
     null, or use BFS and stop at the first leaf. BFS can finish early on wide
     trees.
  3. How do you find the diameter (the longest path between any two nodes)?
     Keep this same recursion, which returns height. At each node, also update a
     shared best value with leftHeight + rightHeight. It is still one pass.
  4. What if each node can have many children (an N-ary tree)?
     Take the max over all children in a loop, then add 1. The structure stays
     the same.
TRIGGER
  When a tree answer for a node is built only from the answers of its children,
  reach for post-order recursive DFS.
C# NOTE
  The body can be one expression-bodied member: public int MaxDepth(TreeNode
  root) => root == null ? 0 : 1 + Math.Max(MaxDepth(root.left),
  MaxDepth(root.right)); The logic is the same, just shorter.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
