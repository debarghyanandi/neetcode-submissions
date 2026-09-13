// --------------------------------------------------------------------------
// -  optimal.cs            O(n) time / O(n) space
// -  in-order DFS, early-stop via sentinel propagation
// -  [inorder-traversal-early-stop]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  recurses left-root-right counting visited nodes, returning early once
// -  the k-th is found, but worst case (skewed tree or large k) still
// -  visits O(n) nodes and recurses O(n) deep
// --------------------------------------------------------------------------

public class Solution
{
    private int visitedCount = 0;

    public int KthSmallest(TreeNode root, int k)
    {
        if (root == null)
            return -1;

        int left = KthSmallest(root.left, k);
        if (left != -1)
            return left;

        visitedCount++;

        if (visitedCount == k)
        {
            return root.val;
        }

        return KthSmallest(root.right, k);
    }
}

/*
================================================================================
 PATTERN : In-order DFS, visit counter plus -1 sentinel return
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The BST property says every value in root.left is smaller than root.val and
  every value in root.right is larger. Nothing local to a node tells you its
  rank, so you cannot navigate straight to the answer without extra bookkeeping.
  What you can do is drain the left subtree completely before touching root,
  which emits values in ascending order; the k-th value emitted is the answer.
  That is why the recursion order here is left, then self, then right, and never
  anything else.
ALGORITHM
  1. Empty node: return -1, meaning "answer not in here".
  2. Recurse left first and capture the result in left.
  3. If left != -1 the answer was already found below; return it immediately and
  touch nothing else.
  4. Otherwise the entire left subtree has been counted, so increment
  visitedCount for root itself.
  5. If visitedCount == k, root is the answer; return root.val.
  6. Otherwise recurse right and return whatever it gives back (possibly -1).
INVARIANT
  At the instant visitedCount++ executes at some node, every node in the whole
  tree holding a smaller value has already been incremented exactly once, and no
  node holding a larger value has been touched. So immediately after that
  increment, visitedCount equals the 1-based rank of root.val among all values
  in the tree. The test visitedCount == k is therefore an exact rank test, not
  an approximation, and it fires at exactly one node. This is the sentence to
  say out loud when asked to prove the code correct.
WHY THE EARLY RETURN IS LOAD-BEARING
  The check if (left != -1) return left is not an optimization, it is required
  for correctness. Delete it and the recursion keeps unwinding upward, keeps
  running visitedCount++ at every ancestor, and visitedCount climbs past k. The
  equality test never fires again, the already-found value is discarded, and the
  call returns -1. Because the increment sits after this check, no node ordered
  after the answer is ever counted, and the right subtree of every ancestor on
  the path back up is skipped.
THE -1 SENTINEL
  -1 carries two meanings at once: "this subtree is empty" and "the answer is
  not in this subtree". Unifying them is what keeps the code this short. It is
  only safe because the problem constrains node values to 0 <= val <= 10^4.
  Loosen that and it breaks concretely: if the answer node holds -1, its parent
  reads left == -1, treats the found answer as a miss, increments visitedCount
  past k, and walks right, returning a wrong value or -1. The fixes are to
  return int? and test HasValue, or to switch to a bool TryKthSmallest with an
  out int result. Expect this exact question from an interviewer.
SHARED MUTABLE COUNTER
  visitedCount is an instance field rather than a parameter because a plain int
  parameter is passed by value, so sibling recursive calls would each get their
  own copy and the count would not accumulate across the traversal. The cost is
  that it is never reset: calling KthSmallest twice on the same Solution object
  makes the second call start from the first call's leftover count, so it
  overshoots k and returns -1. The method is also not safe to call concurrently
  on one instance. Cleaner alternatives that keep the sharing but drop the
  hidden state: pass ref int visited to a private helper, or capture a local in
  a closure.
FOLLOW-UP
  If the tree is queried many times and mutated between queries, this approach
  is the wrong shape because every query re-walks from the root. Store a subtree
  node count in each node, then at each step compare k against size(root.left) +
  1 to decide left, self, or right, and repair the counts along the insert or
  delete path. That answers a query in O(h).

  The other follow-up is to rewrite this iteratively with an explicit
  Stack<TreeNode>, pushing left spines and popping k times. That version needs
  no sentinel value and no field, so both traps above vanish. Also note the
  failure mode of this code when k exceeds the number of nodes: it returns -1
  silently rather than signaling an error.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
