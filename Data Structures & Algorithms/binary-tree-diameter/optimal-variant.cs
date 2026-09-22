// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// -  Post-order DFS height calculation   [dfs-postorder-height]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  Each node visited once; returns tuple instead of global variable; call
// -  stack depth equals tree height (worst case n for skewed tree).
// --------------------------------------------------------------------------

public class Solution
{
    public int DiameterOfBinaryTree(TreeNode root)
    {
        var (_, diameter) = DFS(root);
        return diameter;
    }

    private (int height, int diameter) DFS(TreeNode node)
    {
        if (node == null)
            return (0, 0);

        var left = DFS(node.left);
        var right = DFS(node.right);

        int height = 1 + Math.Max(left.height, right.height);
        int diameterThroughHere = left.height + right.height;
        int diameter = Math.Max(diameterThroughHere, Math.Max(left.diameter, right.diameter));

        return (height, diameter);
    }
}

/*
================================================================================
 PATTERN : Post-order DFS returning (height, best) pair up the tree
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  left                 the (height, diameter) pair from the left subtree
  right                the (height, diameter) pair from the right subtree
  height               1 + the taller child's height, for this node
  diameterThroughHere  edge count of the longest path whose top point is this node
  diameter             best of the three candidates seen in this subtree
WHY THIS PATTERN
  The longest path in a tree has one highest node, and at that node the path
  goes down into the left subtree and down into the right subtree. So for each
  node you only need two numbers from each child: how deep it goes (height) and
  the best answer already found inside it (diameter). A single post-order walk -
  children first, then the parent - computes both, which is why DFS returns a
  tuple instead of one value. The final answer is the diameter field of the
  root's pair.
BRUTE FORCE
  The first thing most people write is: for every node compute
  diameterThroughHere by calling a separate Height(node) helper on each child,
  and take the max over all nodes. That re-walks the same subtrees once per
  ancestor, so it costs O(n^2) time on a skewed tree. This file kills the
  repeated work by having the same recursion hand the height back up alongside
  the running best.
INVARIANT
  When DFS(node) returns, height is the number of nodes on the longest downward
  path starting at node, and diameter is the edge count of the longest path that
  lies entirely inside node's subtree. The three-way Math.Max is exhaustive: any
  path in this subtree either passes through node (diameterThroughHere) or is
  fully inside one child (left.diameter or right.diameter). Since both facts
  hold for the children before the parent uses them, induction gives the correct
  answer at the root.
NODES VERSUS EDGES
  The two numbers use different units on purpose. height counts nodes (a leaf
  returns 1 because null returns 0), while the problem's diameter counts edges.
  left.height + right.height happens to be exactly the edge count of the path
  through this node, because the two +1s for the node's own two edges are
  already baked into the children's node counts. A leaf gives 0 + 0 = 0, which
  is right.
WATCH OUT
  The null case returns the tuple (0, 0), so an empty tree gives diameter 0 -
  fine, but only because 0 is also a valid diameter, not because of any guard.
  The recursion has no depth limit: a long chain of nodes makes the call stack
  as deep as the tree, and a very skewed tree can throw StackOverflowException,
  which you cannot catch in .NET. Also note the local int height shadows nothing
  but is easy to confuse with left.height; if you ever mix them up and write 1 +
  left.height + right.height you silently start counting nodes instead of edges.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you avoid returning a tuple?
     Yes - keep a field or a ref int best on the Solution class, have DFS return
     only the int height, and update best inside. Same work, one less allocation
     per call, but the method is no longer pure and is not safe to call from two
     threads on one instance.
  2. How would you remove the recursion for a very deep tree?
     Do an explicit post-order traversal with a Stack<TreeNode> plus a
     Dictionary or a second stack holding each node's computed height, so the
     heights of both children are ready before you pop the parent. It costs more
     code and an explicit heap-allocated stack, but the depth limit becomes
     memory rather than the thread stack.
  3. What if each edge has a weight and you want the heaviest path?
     Replace 1 + Math.Max(...) with weight + Math.Max(...) using the edge weight
     into each child, and diameterThroughHere becomes leftWeighted +
     rightWeighted. With negative weights you must also allow a child's
     contribution to be clamped at 0, since skipping a branch can beat taking
     it.
  4. You also need the actual path, not just its length.
     Store the node where the best diameter was achieved while taking the
     three-way max, then from that node walk down the deepest child on each
     side, which needs the heights kept per node or recomputed along those two
     chains only.
TRIGGER
  A tree question that asks for the longest or best path that may bend at some
  node instead of starting at the root - return a per-node "reaching down" value
  and a separate running best.
C# NOTE
  The value tuple (int height, int diameter) is a struct, so each DFS return is
  stack-copied, not heap-allocated, and the named fields let you write
  left.height instead of left.Item1. The deconstruction var (_, diameter) =
  DFS(root) with the discard for height is the idiomatic way to drop the part
  you do not need.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
