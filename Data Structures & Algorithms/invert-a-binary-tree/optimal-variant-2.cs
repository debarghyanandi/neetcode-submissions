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
 PATTERN : Iterative DFS - explicit stack, swap children in place
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-2.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
WHY THIS PATTERN
  The recursive version of this problem is three lines, so the only reason to
  write the loop is control over the stack. Recursion depth here equals tree
  height, and a degenerate tree (every node has one child) drives that to n and
  blows the CLR call stack. Stack<TreeNode> moves the same frames onto the heap,
  where the only limit is memory. That is the whole argument for this file - it
  buys robustness on skewed input, nothing else.
CORRECTNESS ARGUMENT
  Two claims. (1) Every node is pushed exactly once: root is pushed before the
  loop, and each node pushes its two children only at the moment it is popped,
  and each node has exactly one parent, so it is pushed by exactly one pop. (2)
  Every popped node has its two child pointers swapped. Together, every node in
  the tree gets its children swapped exactly once, which is the definition of
  the inverted tree. Nothing here depends on the order pops happen in.
INVARIANT
  The body of the loop touches only node.left and node.right - it never reads or
  writes any pointer inside the subtrees hanging off them. So the swap at one
  node commutes with the swap at any other node. That is why LIFO vs FIFO does
  not matter, and why swapping before pushing is safe: the pushes read node.left
  and node.right after the swap, so they push the two children in the other
  order, but it is still the same two children. Swap-then-push and
  push-then-swap enqueue identical sets.
WALK THE POINTERS
  leftChild exists because this is a plain three-step swap and C# has no
  tuple-free way around it here. Drop it and write node.left = node.right first,
  and both fields now point at the original right child - the original left
  subtree is unreachable and lost. If you want it terser, (node.left,
  node.right) = (node.right, node.left) is the same three steps with the temp
  hidden by the compiler.
WATCH OUT
  The null handling is split across two places and both are load-bearing. The
  guard at the top returns null for an empty tree, so root is never pushed as
  null. Inside the loop, children are filtered by the two if-checks before being
  pushed, so the stack only ever holds non-null references and node.Pop() needs
  no null test. If you move the null check to the pop site instead, you must
  remove it from the push site or you are paying for it twice - just do not do
  neither. Also note the stack peaks at roughly the widest level of the tree, so
  a bushy tree costs more here than a skewed one, which is the opposite of the
  recursive version's cost profile.
THE FOLLOW-UP
  Expect: change it to BFS. Swap Stack for Queue<TreeNode>, Push for Enqueue,
  Pop for Dequeue, Count stays - the loop body is otherwise unchanged, precisely
  because of the commuting-swaps argument above. Expect also: does the caller
  see the change? Yes - root is mutated in place and the returned reference is
  the same object that was passed in, so any other reference the caller holds to
  that tree now sees the inverted version. There is no copy.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
