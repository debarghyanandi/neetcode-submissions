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
    //Iterative DFS
    public TreeNode InvertTree(TreeNode root)
    {
        if (root == null) return null;
        Stack<TreeNode> stack = new Stack<TreeNode>();
        stack.Push(root);
        while (stack.Count > 0)
        {
            TreeNode node = stack.Pop();
            TreeNode leftChild = node.left;
            node.left = node.right;
            node.right = leftChild;
            if (node.left != null) stack.Push(node.left);
            if (node.right != null) stack.Push(node.right);
        }
        return root;
    }
}

/*
================================================================================
 PATTERN : Iterative DFS - explicit stack, swap children on pop
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-2.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
WHY THIS PATTERN
  Inverting a tree is a pure per-node action: at every node, left and right
  trade places, and nothing else about the tree changes. So this is a full
  traversal with one constant-time operation at each stop, and the only real
  design choice is how you walk the nodes. The Stack<TreeNode> here stands in
  for the call stack of the recursive version, which is the point of this
  variant: recursion depth on a skewed tree is bounded by the thread's stack,
  while an explicit stack is bounded only by heap memory.
CORRECTNESS ARGUMENT
  Two facts do all the work.

  1. Every node is pushed exactly once. In a tree each node is the child of
  exactly one parent, so it is reached by exactly one of the two guarded pushes;
  root is pushed once explicitly. Push-once plus pop-once means each node is
  swapped exactly once - never skipped, never double-swapped (a second swap
  would undo the first).

  2. The swap at a node writes only that node's left and right fields. Swaps at
  two different nodes touch disjoint memory, so they commute. Order of
  visitation is therefore irrelevant to the result - preorder, postorder, level
  order all produce the same tree.
THE SWAP
  leftChild exists because node.left = node.right destroys the old left pointer
  before you can save it. Without the temp, writing node.left = node.right then
  node.right = node.left leaves both fields pointing at the original right
  subtree and the entire left subtree is dropped from the tree - no crash, no
  infinite loop, just a silently wrong answer that a small symmetric test case
  can still pass. After the two assignments, node.left holds what used to be the
  right subtree.
PUSH ORDER IS A NON-ISSUE
  The two pushes read node.left and node.right AFTER the reassignment, so what
  actually goes on the stack is the original right child first, then the
  original left. That looks like a bug on a skim, but the swap only relabels
  which pointer names which subtree - the set of children is identical before
  and after. The same two subtrees get visited either way; moving the pushes
  above the swap would only flip the pop order, which point 2 of the correctness
  argument already showed does not matter.
WATCH OUT
  The if (root == null) return null; guard is load-bearing, not defensive
  decoration. Both pushes are null-checked, so no null ever enters the stack and
  node inside the loop is guaranteed non-null. Remove the top guard and a null
  root gets pushed, popped, and node.left throws a NullReferenceException on the
  first iteration.

  Second: this mutates in place and hands back the same object the caller passed
  in. root is never reassigned - only children's pointers are. Any reference the
  caller was holding to a subtree is now hanging off the opposite side of its
  parent.
FOLLOW-UP AN INTERVIEWER WILL ASK
  "Make it BFS." It is a one-line edit: change Stack<TreeNode> to
  Queue<TreeNode> and Push/Pop to Enqueue/Dequeue. Nothing else moves, and the
  correctness argument above carries over unchanged because it never depended on
  order.

  "What does the stack actually hold at peak?" Shape-dependent. On a left-skewed
  chain, each pop pushes exactly one node, so the stack never holds more than
  one node at a time - precisely the case where the recursive version would nest
  n frames. The worst case is the wide one: a perfect tree accumulates roughly
  the bottom level, about n/2 nodes.
TRIGGER
  Reach for this shape when the task is "mirror", "flip", or "invert" a tree, or
  any transform where each node's update depends only on that node's own fields.
  When per-node work is independent, skip the debate about traversal order and
  pick whichever container is easiest to reason about. Contrast with problems
  where the update depends on children's results (height, diameter, sum of
  subtree) - those need a genuine postorder and this pop-and-mutate loop will
  not do.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
