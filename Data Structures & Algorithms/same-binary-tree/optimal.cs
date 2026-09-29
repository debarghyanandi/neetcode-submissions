// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
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
 PATTERN : Tree DFS - recursive structural comparison of two trees
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-1.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  first    the current node in tree one (can be null)
  second   the node at the same position in tree two (can be null)
WHY THIS PATTERN
  The problem asks whether two trees have the same shape and the same values.
  That is true only if the roots match and the left subtrees match and the right
  subtrees match. Each of those checks is the same question on a smaller pair of
  trees, so recursion on (first.left, second.left) and (first.right,
  second.right) fits the problem directly. The recursion follows both trees
  together, so each call always compares nodes at the same position.
BRUTE FORCE
  The simplest correct approach is to serialize both trees, for example in
  preorder with a marker such as "#" for every null child, and then compare the
  two strings. This takes O(n) time and space, but it builds two full strings
  and always walks every node. The recursive compare can stop at the first
  mismatch.
INVARIANT
  Each call returns true only when the subtree under first and the subtree under
  second are identical. There are three cases. If both are null, the subtrees
  are equal (two empty trees). If both are non-null with equal val, the answer
  depends only on the two child pairs. Every other case is a mismatch: one side
  is null, or the values differ. These cases cover all inputs, so by induction
  on subtree height the answer at the root is correct.
WATCH OUT
  The recursion goes as deep as the tree is tall. A very skewed tree, like a
  linked list, makes the call stack as deep as the node count and can cause a
  StackOverflowException. There is no try/catch that can recover from that.
  Also, the final "return false" handles two different cases at once: exactly
  one node is null, or the values differ. If you later split the second if
  statement, make sure the one-null case still returns false and does not fall
  through into reading .val on a null node.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you remove the recursion?
     Use a Stack or Queue of (TreeNode, TreeNode) pairs. Push (first, second).
     Pop a pair and apply the same three checks, then push the two child pairs.
     The call stack is gone, but you manage the pairs yourself, and the memory
     still grows with tree height or width.
  2. How do you check whether a tree is symmetric (a mirror of itself)?
     Use the same function on two inputs, left and right, but compare them
     crossed: a.left with b.right, and a.right with b.left. The code only
     changes in which children you pair up.
  3. How do you check whether tree t is a subtree of tree s?
     Call IsSameTree(node, t) at every node of s. That is O(m*n) in the worst
     case. A faster way is to serialize both trees with null markers and search
     for t's string inside s's string with KMP, which is O(m+n). The trade-off
     is extra memory for the strings.
TRIGGER
  Reach for this when a problem asks whether two trees (or two halves of one
  tree) match node by node, so you can walk both trees in lockstep with one
  recursive call per child pair.
C# NOTE
  You could write "first is null && second is null" in place of "first == null
  && second == null". The "is null" check can never be redirected by a custom ==
  operator on TreeNode, and it reads as a pure reference check.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
