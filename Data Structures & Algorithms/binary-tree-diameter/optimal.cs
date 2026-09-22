// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  Post-order DFS height calculation   [dfs-postorder-height]
// -  ties with optimal-variant.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  Each node visited once during recursive descent; call stack depth
// -  equals tree height (worst case n for skewed tree).
// --------------------------------------------------------------------------

public class Solution
{
    public int res = 0;

    public int DiameterOfBinaryTree(TreeNode root)
    {
        Height(root);
        return res;
    }

    private int Height(TreeNode root)
    {
        if (root == null)
            return 0;

        int left = Height(root.left);
        int right = Height(root.right);

        res = Math.Max(res, left + right);

        return 1 + Math.Max(left, right);
    }
}

/*
================================================================================
 PATTERN : Post-order DFS - height return, diameter tracked in a field
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  res      best diameter seen so far, in edges
  left     height of root.left subtree
  right    height of root.right subtree
WHY THIS PATTERN
  The longest path between any two nodes must bend at exactly one node - its
  highest point. So the problem becomes: for every node, what is the longest
  path that passes through it? That value is left + right, the two deepest
  downward paths. One post-order walk computes the height of each node and, at
  the same visit, offers left + right as a candidate for res.
BRUTE FORCE
  The first version most people write calls a separate Height function from
  inside a recursion over every node: for each node compute height(left) +
  height(right) and take the max. That re-walks every subtree once per ancestor,
  so it costs O(n^2) on a skewed tree. This file fixes it by returning the
  height upward and updating res on the way, so each node is visited once.
INVARIANT
  When Height(node) returns, two things hold: the return value is the number of
  nodes on the longest downward path from node, and res is the maximum of left +
  right over every node already visited, including node itself. Since the true
  diameter bends at some node and that node is visited exactly once, res ends up
  holding it. The order matters - res is updated after both child calls return,
  so left and right are final values.
EDGES VERSUS NODES
  Height returns a node count (a leaf returns 1), but res stores left + right,
  which is an edge count. For a leaf, left and right are both 0, so res stays 0
  - correct, since a single node has diameter 0. The mix works because adding
  the two child heights counts exactly the edges on both sides of the bend node.
WATCH OUT
  res is a public instance field initialized to 0, not a local. If the same
  Solution object is reused for a second tree, res keeps the old value and the
  answer can only grow - the judge builds a new object per test, but that is
  luck, not design. Reset res at the top of DiameterOfBinaryTree or pass it by
  ref. Also, the recursion depth equals the tree height, so a long chain of
  nodes can throw StackOverflowException, which .NET cannot catch.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Remove the mutable field - how?
     Have Height return a pair, for example a (int height, int diameter) tuple,
     and combine children in the caller. Pure function, no shared state, but
     more allocation-free struct plumbing and slightly noisier code.
  2. Return the actual path, not just its length.
     Store the bend node when res improves, then walk down from it on each side
     always choosing the deeper child. Costs one extra downward walk, still
     linear.
  3. The tree is too deep for recursion - rewrite it iteratively.
     Do an explicit-stack post-order traversal, or reverse-topological order,
     keeping a dictionary or field of computed heights per node. Same O(n) time,
     but you now pay for the stack and the height map explicitly instead of
     using the call stack.
  4. Each edge has a weight and you want the heaviest path.
     Height returns the max weighted downward distance, and the candidate
     becomes left + right where each side already includes its edge weight. Same
     shape; only negative weights would force extra care.
TRIGGER
  A tree question asking for a best value over all nodes where each node's
  answer needs facts from both subtrees - return one thing upward, record
  another in a field.
C# NOTE
  Math.Max is used twice per node on ints and reads clearly; the alternative res
  = left + right > res ? left + right : res buys nothing here. The public int
  res field is the only piece of state - making it private and resetting it in
  DiameterOfBinaryTree would match normal C# style without changing the
  algorithm.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
