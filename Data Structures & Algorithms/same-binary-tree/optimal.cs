// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// #  recursive DFS, pairwise compare   [dfs-pairwise-compare]
// #  ties with optimal-variant-2.cs on O(n) time / O(n) space
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  visits each node once via recursion, comparing positions in lockstep;
// #  call stack depth is O(n) for a skewed tree
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
 PATTERN : Lockstep DFS - walk both trees on the same path
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-1.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  Two trees are identical exactly when their roots match and their corresponding
  subtrees match. That definition is already recursive, so the code is a direct
  transcription of it: pair first with second, pair first.left with second.left,
  pair first.right with second.right. Nothing is serialized, flattened, or
  hashed - you never build an intermediate representation of either tree, which
  is why a single traversal of the shorter structure decides it. Note that one
  traversal order would NOT be enough on its own: two different trees can share
  a preorder or an inorder sequence, so the lockstep pairing, not the visit
  order, is what carries the proof.
THE THREE EXITS
  Every call leaves through exactly one of three points, and it is worth naming
  what reaches each.

  1. return true - both first and second are null. Two empty subtrees.
  2. the recursive return - both non-null AND values equal. Defer the answer to
  the children.
  3. the final return false - everything else.

  That last line silently folds two distinct failure modes: exactly one of
  first/second is null (the shapes diverge here), or both are non-null but
  first.val != second.val (the shapes agree here, the payloads do not). It is
  safe as a fall-through because the both-null case was already consumed by exit
  1, so "not both null and not (both non-null with equal values)" really is the
  complete failure set.
INVARIANT
  Every invocation of IsSameTree receives a pair of nodes reached by the
  identical sequence of left/right moves from their respective roots. True at
  the top call by definition; preserved by the recursive step because left is
  only ever paired with left and right only ever with right. This is the whole
  reason a positional comparison is legitimate - you are never comparing
  first.left against second.right.
CORRECTNESS
  Induction on the height of the first tree. Base: height -1 (null). The call
  returns true iff second is also null, which is correct for empty trees. Step:
  assume the calls on both child pairs are correct. If the trees are identical,
  then first.val == second.val and both child pairs are identical, so exit 2 is
  taken and both recursive calls return true. If they are not identical, the
  difference is either at this node (caught by exit 1 or 3) or inside one of the
  subtrees (caught by the inductive hypothesis, and propagated because &&
  returns false the moment either side does). The recursion terminates because
  each call descends one level and the null base case is reachable from every
  path.
SHORT-CIRCUIT ORDER
  Both && chains in this file depend on evaluation order, for different reasons.

  In the guard, first != null && second != null && first.val == second.val - the
  null tests must precede the dereference. Reorder the conjuncts so that
  first.val is read first and you get a NullReferenceException on any tree pair
  where one side ends before the other, which is the very case the function
  exists to detect.

  In the returned expression, IsSameTree(first.left, ...) &&
  IsSameTree(first.right, ...) - a false from the left subtree skips the right
  subtree recursion entirely. That is pure pruning, not correctness: swapping to
  a non-short-circuiting & would still return the right answer, just after
  exploring a subtree whose result can no longer matter.
WATCH OUT
  The classic wrong refactor is hoisting the null handling into a single early
  guard: if (first == null || second == null) return false; That is wrong for
  the both-null pair, and every leaf in a matching tree hits exactly that pair,
  so it returns false on identical trees. If you want the flat form, it has to
  be first == null || second == null then return first == second, which happens
  to work only because both being null makes the reference comparison true.

  Separately, first.val == second.val is a value comparison and the whole point;
  reference-comparing the nodes themselves (first == second) answers a different
  question - whether the two trees literally share nodes in memory - and would
  return false for structurally identical but separately allocated trees.
TRIGGER
  Reach for lockstep recursion whenever a problem hands you two positions that
  must advance together. The tells are "same", "mirror", "merge", or "contains"
  over two trees.

  Symmetric tree: same body, but the root call pairs root.left with root.right
  and the recursive step crosses over (a.left with b.right). Subtree of another
  tree: call this exact function from every node of the larger tree. Merge two
  binary trees: same three-case split, except the one-null branch returns the
  surviving node instead of false.
FOLLOW-UP
  "Make it iterative" is the expected push. Push the pair (first, second) onto a
  Stack of tuples, and on each pop apply the identical three-case test, pushing
  the two child pairs when the node test passes. The answer is true if the stack
  drains without a failure.

  Be ready to say why that does not actually buy you anything asymptotically:
  the explicit stack holds at most what the call stack held, and for a
  degenerate skewed tree that is one frame per node either way. The real
  argument for the iterative form is avoiding a stack overflow on a deep tree in
  an environment with a fixed stack, not a better bound. There is no tail call
  to exploit here either - the recursive return combines two results with &&, so
  neither call sits in tail position.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
