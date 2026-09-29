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
 PROBLEM : You get the roots of two binary trees. Return true if they are the
           same tree. Same means the same shape and the same value at every
           matching node. Example: [1,2,3] and [1,2,3] -> true; [1,2] and
           [1,null,2] -> false.
 PATTERN : Iterative DFS with an explicit stack of node pairs
================================================================================
IDEA
  Walk both trees at the same time. Each stack entry is a pair (node1, node2)
  that sits at the same position in the two trees. Pop a pair. If both are
  null, skip it. If only one is null or the vals differ, return false.
  Otherwise push the right pair and then the left pair. Every position is
  checked once, so a mismatch in value or shape is always found. The explicit
  stack takes the place of the recursion call stack.
EXAMPLE
  root1 = [1,2,3], root2 = [1,2,4]
  pop (1,1) ok -> push (3,4), push (2,2); pop (2,2) ok -> push 2 null pairs
  pop (null,null) x2 -> continue; pop (3,4) -> 3 != 4 -> return false
COMPLEXITY
  Time  O(n)  each node pair is pushed and popped once, with O(1) work per
              pop.
  Space O(n)  the stack grows with the tree height, and a skewed tree has
              height n.
WATCH OUT
  - Keep the order of the checks. Test "both null" first, then "one null".
    If you read node1.val before the null checks, the code crashes.
  - Compare node1.val with node2.val, not node1 with node2. Comparing the
    nodes checks references, and nodes from two separate trees never match.
  - Always pair left with left and right with right. Pairing left with right
    checks for a mirror tree, which is a different problem.
  - Null children are pushed on purpose. If you skip them, [1,2] and
    [1,null,2] wrongly return true.
================================================================================
*/
