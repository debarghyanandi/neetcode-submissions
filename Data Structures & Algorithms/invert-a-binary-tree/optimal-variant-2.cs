// --------------------------------------------------------------------------
// -  optimal-variant-2.cs  O(n) time / O(n) space
// -  iterative DFS with explicit stack, swap children
// -  [iterative-dfs-stack-swap]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself (from submission-2)
// -
// -  explicit stack mimics recursion depth, worst-case O(n) for a skewed
// -  tree but only O(h) for balanced trees
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
 PATTERN : Iterative DFS - explicit stack, swap children at pop
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-2.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
WHY THIS PATTERN
  The mirror of a tree is defined purely locally: at every node, exchange the
  two child pointers. Nothing compares one node against another, and no node's
  result depends on a neighbour's, so any traversal that reaches every node
  exactly once is enough. That freedom is what lets the recursion be replaced by
  a hand-rolled Stack<TreeNode>: the depth of the walk now lives in a
  heap-allocated container instead of in call frames, so a long root-to-leaf
  chain costs stack objects rather than nested invocations.
ALGORITHM
  1. root == null returns null - an empty tree is its own mirror and nothing is
  ever pushed.
  2. Push root, then loop while stack.Count > 0.
  3. Pop into node. Save node.left into leftChild, set node.left = node.right,
  set node.right = leftChild.
  4. Push node.left if non-null, then node.right if non-null (these fields are
  read after the swap).
  5. Return root - the same object that came in, now mutated.
INVARIANT
  At the top of every iteration: each node still on the stack has not yet had
  its children exchanged, and each node already popped has had them exchanged
  exactly once.

  The counting argument for correctness: there are only two push sites, root
  before the loop and the two child pushes at the moment a parent is popped.
  Every non-root node has exactly one parent, so it is pushed exactly once,
  therefore popped exactly once, therefore swapped exactly once. n nodes, n
  swaps, one per node - which is the definition of the mirror. Termination
  follows from the same fact: pushes are bounded by n, so the loop runs exactly
  n times.
WHY SWAPPING BEFORE PUSHING IS SAFE
  After the three assignment lines, node.left holds the old right child. The
  pushes therefore enqueue the old right subtree first and the old left second -
  the reverse of what pushing before the swap would do. It does not matter: the
  set of children pushed is identical either way, only their visit order flips,
  and visit order is irrelevant because swapping at node A writes only A's two
  fields and never touches a pointer inside A's subtrees. Operations at distinct
  nodes commute, so preorder, postorder, or level order all produce the same
  final tree.
WATCH OUT
  - leftChild is not cosmetic. Writing node.left = node.right; node.right =
  node.left leaves both fields pointing at the old right child and silently
  drops the entire left subtree. The temp is the whole correctness of the swap.
  - The null tests guard the pushes, not the pop, so node is guaranteed non-null
  inside the loop body. If you push unconditionally instead, you must add a null
  check right after Pop, and every leaf then contributes two null pushes.
  - This mutates in place and returns the original reference. Any caller holding
  a pointer to an interior node sees that subtree mirrored underneath it too;
  nothing is copied.
  - Peak stack size is bounded by the depth of the current path plus one (each
  iteration removes one node and adds at most two). That is about log n for a
  balanced tree; it only reaches order n on a spine where nearly every node also
  carries a second child, such as a caterpillar shape.
INTERVIEW FOLLOW-UP
  - "Make it BFS." Swap Stack<TreeNode> for Queue<TreeNode> and change Push/Pop
  to Enqueue/Dequeue - nothing else moves, because of the commutativity above.
  The trade is which shape hurts you: the queue peaks at the widest level, this
  stack peaks at the deepest path.
  - "Why not just recurse?" The recursive form is three lines but spends one
  call frame per level, which is the exact cost this version moves off the call
  stack.
  - "Invert without mutating the input." Preorder still works here: allocate a
  copy of root, push pairs of (source, copy), and for each pair set copy.left
  from source.right and copy.right from source.left, pushing the matching pairs.
  No postorder or old-to-new map is needed as long as the parent copy exists
  before its children are processed.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
