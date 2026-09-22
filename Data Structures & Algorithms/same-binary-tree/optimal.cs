// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// #  Recursive DFS traversal   [recursive-dfs]
// #  ties with optimal-variant-2.cs on O(n) time / O(n) space
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Recursion depth equals tree height; worst case O(n) for completely
// #  skewed tree.
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
 PATTERN : Tree Recursion - parallel DFS on two trees
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-1.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  first    root of the first tree in the current pair
  second   root of the second tree in the current pair
WHY THIS PATTERN
  The question asks whether two trees are identical in both shape and values.
  Identity is defined recursively: two nodes match if their values match and
  their left subtrees match and their right subtrees match. So the code walks
  both trees in lockstep, passing first.left with second.left and first.right
  with second.right, and the recursion mirrors the definition exactly.
BRUTE FORCE
  A common first attempt is to serialize each tree into a string or list with
  null markers, for example a preorder walk, and then compare the two results.
  That is still O(n) time but it builds two full sequences, so it uses extra
  memory and does not stop early at the first mismatch. This version returns
  false the moment first.val != second.val, without allocating anything.
INVARIANT
  Every call receives two nodes that occupy the same position in their trees,
  reached by the same sequence of left/right moves from each root. The function
  returns true only if the subtrees rooted at those two positions are identical.
  Because every position is checked, and null-versus-node is caught by the final
  return false, a true result at the root means every position in both trees
  agrees.
WATCH OUT
  The three cases must stay in this order. If first is null and second is not,
  the first if fails, the second if fails on first != null, and control falls to
  return false - correct, but only because the null check comes first. A
  refactor that touches first.val before confirming both are non-null throws a
  NullReferenceException. Also note the recursion depth equals the tree height,
  so a long skewed chain can overflow the stack; the code has no depth guard.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How would you do this without recursion?
     Push pairs onto an explicit stack or queue, for example a Stack of
     (TreeNode, TreeNode) tuples, popping a pair and comparing it, then pushing
     the two child pairs. Same time, and the stack size replaces the call stack,
     so you control the memory instead of the runtime.
  2. What if the question changed to "is second a subtree of first"?
     Keep this method as a helper, then walk every node of first and call
     IsSameTree(node, second) at each one. That costs O(n*m) in the worst case;
     hashing each subtree or string matching on serialized trees brings it down.
  3. What if the trees are the same shape but children may be swapped - a mirror
  check?
     Compare first.left with second.right and first.right with second.left
     instead. The value check and the null base cases stay identical.
  4. What if nodes held a value type that is not comparable with ==, such as a
  string or a custom class?
     Replace first.val == second.val with an equality comparer call so reference
     comparison is not used by accident. Here val is an int, so == is a plain
     value comparison and is safe.
TRIGGER
  Two trees (or two positions in one tree) must be compared in lockstep -
  recurse on both at once and let the null cases be the base.
C# NOTE
  The && operator short-circuits, so IsSameTree(first.right, second.right) is
  never called once the left subtrees disagree - the early exit costs nothing
  extra to write.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
