// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
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
 PATTERN : Iterative DFS - compare two trees in lockstep with a stack
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  stack    pairs (node1, node2) still waiting to be compared, one from each tree at the same position
  node1    the current node taken from the first tree (root1)
  node2    the node at the same position in the second tree (root2)
WHY THIS PATTERN
  Two trees are the same only if every position holds the same value and has the
  same shape. So you must visit the matching positions of both trees together.
  The stack holds pairs (node1, node2), so the two trees are walked in exactly
  the same order. The walk stops at the first pair that is different.
BRUTE FORCE
  A simple first idea: serialize each tree to a string or list, writing a marker
  like "#" for null children, then compare the two results. This is O(n) time
  and O(n) space, and it is correct only if you include the null markers. It
  loses because it always walks both whole trees and builds extra output, even
  when the roots already differ. The paired walk can stop at the first
  difference.
INVARIANT
  Every pair on the stack sits at the same path from the roots in both trees.
  Every pair already popped matched: both were null, or both had equal val. If
  any pair fails, the trees differ at that position, so returning false is
  correct. If the stack empties, every position has been checked in both trees
  with no difference, so returning true is correct.
WATCH OUT
  The order of the checks matters. The both-null check must come first. After
  it, "node1 == null || node2 == null" catches the case where only one is null,
  before node1.val is ever read. If you swap the checks or merge them
  carelessly, you get a NullReferenceException. Also, null pairs are pushed onto
  the stack and only thrown away when popped, so every leaf adds two extra
  push/pop steps.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you write it recursively?
     Yes: return true if both are null, false if only one is null, else compare
     val and recurse on left and on right. The code is shorter, but the call
     stack can grow to the tree height, and a very deep, skewed tree can
     overflow it. The explicit Stack here lives on the heap, so it avoids that.
  2. How would you check if one tree is a subtree of another?
     Run this same-tree check from every node of the big tree. That is O(m*n) in
     the worst case. For O(m+n), serialize both trees with null markers and use
     string matching such as KMP, or compare tree hashes.
  3. What if the trees are mirror images (Symmetric Tree)?
     Push crossed pairs instead: (node1.left, node2.right) and (node1.right,
     node2.left). Start from (root.left, root.right).
  4. Would BFS work too?
     Yes. Use a Queue of pairs instead of the Stack. Correctness and cost are
     the same. Only the visit order changes, which affects how early a
     difference is found.
TRIGGER
  When you must check that two tree structures match position by position, walk
  them together with one stack or queue of node pairs.
C# NOTE
  Stack<(TreeNode, TreeNode)> uses a value tuple, so each pair is stored inside
  the stack's array with no extra object created per push. "var (node1, node2) =
  stack.Pop();" unpacks the pair with deconstruction, which saves you from a
  small helper class.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
