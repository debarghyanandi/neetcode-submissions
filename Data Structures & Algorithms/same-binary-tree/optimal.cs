// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// #  recursive DFS, pairwise compare   [dfs-compare]
// #  ties with optimal-variant.cs on O(n) time / O(n) space
// #
// #  YOU SOLVED THIS YOURSELF (from submission-1)
// #
// #  Visits each node once; recursion depth (and thus call stack space) is
// #  O(n) for a skewed tree.
// ##########################################################################

public class Solution
{
    public bool IsSameTree(TreeNode first, TreeNode second)
    {
        //my solution.
        if (first == null && second == null)
        {
            return true;
        }
        if (first != null && second != null && first.val == second.val)
        {
            return (IsSameTree(first.left, second.left) && IsSameTree(first.right, second.right));
        }
        return false;
    }
}

/*
================================================================================
 PATTERN : Lockstep DFS on two trees - structural recursion
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-1.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  Tree equality is itself a recursive definition: two trees are the same iff
  both roots are empty, or both exist with equal val and their left subtrees are
  the same and their right subtrees are the same. The method body is that
  sentence transcribed. The key move is advancing first and second together -
  one call frame holds one position in each tree - because shape and value have
  to be checked at the same coordinate. Any approach that flattens one tree
  before looking at the other throws away the alignment you need.
THE THREE EXITS
  Every call leaves through exactly one of three doors.
  1. first == null && second == null -> true. Two empty trees agree. This is the
  terminating base case; every root-to-leaf path in the recursion ends here.
  2. both non-null and first.val == second.val -> recurse on (first.left,
  second.left) and (first.right, second.right).
  3. the final return false absorbs two distinct failures: exactly one of the
  pair is null (shape mismatch) and both non-null with different val (value
  mismatch). Merging them is sound because neither can be repaired by anything
  deeper in the tree - one mismatch anywhere makes the answer false.
  Delete the first if and the code still compiles, but every leaf's null
  children now fall to case 3, so any two non-empty trees compare unequal. That
  base case is load-bearing, not a convenience.
BOTH SHORT-CIRCUITS MATTER
  There are two && chains and they do different jobs.
  In the guard, first != null && second != null && first.val == second.val
  relies on left-to-right short-circuit for null safety: when either side is
  null, evaluation stops before .val is read. Reorder the conjuncts so the value
  comparison comes first and you get a NullReferenceException at the first leaf.
  In the return, IsSameTree(first.left, second.left) && IsSameTree(first.right,
  second.right) short-circuits for work avoidance: a false anywhere in the left
  subtree means the right subtree call is never made. A mismatch near the root
  cuts the traversal off immediately - the full walk only happens when the trees
  actually match.
CORRECTNESS ARGUMENT
  Induct on the height of the shallower argument. Base: height 0 means at least
  one of first/second is null, and cases 1 and 3 decide it with no recursion -
  correctly, since an empty tree equals only an empty tree. Step: assume the two
  child calls return the right answer. The node returns true only when both
  roots exist, their vals agree, and both child calls returned true, which is
  precisely the definition; and it returns false in every case where one of
  those three conditions fails. Termination is free: each recursive call
  descends one level, so the argument pair strictly shrinks.
RECURSION DEPTH AND THE ITERATIVE REWRITE
  The only storage this uses is call frames, and the frame count at any moment
  is the depth of the position being compared. A balanced pair of trees stays
  around log n deep; a degenerate left-chain of 10^5 nodes is where the stack
  actually becomes a risk, and that is the case worth naming if asked. The
  iterative version keeps the same three-case test verbatim but holds the
  frontier explicitly: push the root pair onto a Stack<(TreeNode, TreeNode)>,
  pop a pair, apply case 1 (continue), case 3 (return false), case 2 (push the
  two child pairs). Note the iterative form loses the left-before-right
  short-circuit ordering, but not correctness - it still bails on the first
  mismatch it pops.
WATCH OUT
  The tempting alternative - serialize both trees preorder and compare the
  strings - is wrong unless null children are emitted as explicit markers.
  Without them, a root with child 2 on the left and the same root with child 2
  on the right produce identical sequences. This method sidesteps the issue
  entirely by comparing positions rather than sequences.
  Also note the comparison is first.val == second.val, an int comparison. If
  TreeNode ever carried a reference payload, == would compare identity and you
  would need Equals instead.
  Finally, do not confuse this with subtree-of-another-tree: that problem calls
  a method like this one as a helper at every candidate node, it is not solved
  by this method alone.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
