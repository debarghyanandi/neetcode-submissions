// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// -  Iterative DFS with explicit stack   [iterative-dfs-stack]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  Stack stores pairs of nodes at current depth; worst case O(n) for
// -  skewed tree.
// --------------------------------------------------------------------------

public class Solution
{
    public bool IsSameTree(TreeNode root1, TreeNode root2)
    {
        var stack = new Stack<(TreeNode, TreeNode)>();
        stack.Push((root1, root2));

        while (stack.Count > 0)
        {
            var (node1, node2) = stack.Pop();

            if (node1 == null && node2 == null)
                continue;
            if (node1 == null || node2 == null || node1.val != node2.val)
            {
                return false;
            }
            stack.Push((node1.right, node2.right));
            stack.Push((node1.left, node2.left));
        }

        return true;
    }
}

/*
================================================================================
 PATTERN : Iterative DFS with an explicit stack of node pairs
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  stack    pairs still to compare; each entry is (node from tree 1, node at the SAME position in tree 2)
  node1    current node from the first tree
  node2    the node at the matching position in the second tree
WHY THIS PATTERN
  Two trees are the same only if every position holds the same value and the
  same shape of children. That is a positional comparison, so the unit of work
  is a pair, not a single node - hence a stack of tuples instead of a stack of
  nodes. Popping a pair and pushing (left, left) and (right, right) keeps the
  two walks locked in step, so any mismatch in value or in null-ness is caught
  at the exact position where it happens.
BRUTE FORCE
  The first thing most people write is the recursive form: return true if both
  are null, false if exactly one is null or values differ, otherwise recurse on
  left and right. It does the same amount of work, but the pair stack lives in
  the call stack, so a long skewed tree can throw StackOverflowException, which
  you cannot catch and recover from. Another first attempt is to serialize both
  trees to strings with null markers and compare the strings; that also touches
  every node but builds throwaway text and is easy to get wrong if the null
  marker is omitted.
INVARIANT
  At the top of every loop pass, every pair still on the stack sits at the same
  position in both trees, and every pair already popped has been proven equal in
  value. So if the loop drains the stack without returning false, every
  reachable position matched, and true is correct. The continue on the both-null
  case is what closes a branch: it says this position ends in both trees, so
  nothing below it needs checking.
TRAVERSAL ORDER DOES NOT MATTER
  Pushing right before left makes the pops come out in pre-order, left subtree
  first. Nothing in the algorithm depends on that. Because each stack entry
  carries its own pair and the final answer is an AND over all positions, you
  could swap the two pushes, or use a Queue for level order, and still get the
  same result - only the position of the first mismatch found would change.
WATCH OUT
  The both-null case is handled AFTER the pair is pushed and popped, so leaf
  children fill the stack with (null, null) entries that do nothing but get
  popped and skipped. For a tree with L leaves that is 2L wasted push/pop pairs;
  guarding the pushes with a null test would remove them, at the cost of more
  code. Also note the early-return on mismatch leaves entries on the stack -
  that is fine here because stack is a local, but do not reuse the same stack
  object across calls. Finally, node1.val != node2.val is a plain value compare;
  if val were a reference type this would compare references, not contents.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How would you cut the memory this uses?
     Morris traversal on both trees at once removes the stack by rewiring right
     pointers of predecessors, giving O(1) extra space, but it mutates the trees
     during the walk and the double bookkeeping is easy to get wrong. A simpler
     partial win is to not push (null, null) pairs at all.
  2. Change it to "is tree 2 a subtree of tree 1".
     Keep this method as the equality check, then walk tree 1 and call
     IsSameTree at every node whose val equals root2.val. That is O(n*m) in the
     worst case; a linear answer serializes both trees with null markers and
     runs a substring search such as KMP.
  3. Change it to "is one tree the mirror of the other".
     Same loop, one line different: push (node1.left, node2.right) and
     (node1.right, node2.left). The invariant becomes "paired positions are
     mirror images", and the both-null and value checks stay untouched.
  4. The trees are huge and stored on disk, one page at a time.
     Switch the Stack to a Queue for level-order pairing so nodes are read in
     depth order and whole levels can be streamed, and stop at the first
     mismatch. Time stays linear; peak memory becomes the widest level rather
     than the deepest path.
TRIGGER
  Two structures must be walked in lockstep and compared position by position -
  pair them up on one stack instead of running two separate traversals.
C# NOTE
  (TreeNode, TreeNode) is a ValueTuple, a struct, so the pairs live inside the
  Stack's internal array with no extra object per pair; using a class or a
  custom Pair type would allocate one object per push. The var (node1, node2) =
  stack.Pop() deconstruction needs C# 7 or later.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
