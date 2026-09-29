// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// --------------------------------------------------------------------------

public class Solution
{
    public int DiameterOfBinaryTree(TreeNode root)
    {
        var (_, diameter) = DFS(root);
        return diameter;
    }

    private (int height, int diameter) DFS(TreeNode node)
    {
        if (node == null)
            return (0, 0);

        var left = DFS(node.left);
        var right = DFS(node.right);

        int height = 1 + Math.Max(left.height, right.height);
        int diameterThroughHere = left.height + right.height;
        int diameter = Math.Max(diameterThroughHere, Math.Max(left.diameter, right.diameter));

        return (height, diameter);
    }
}

/*
================================================================================
 PROBLEM : Given the root of a binary tree, return its diameter. The diameter
           is the number of EDGES on the longest path between any two nodes.
           The path does not have to pass through the root. Example:
           [1,2,3,4,5] -> 3 (path 4-2-1-3).
 PATTERN : DFS (post-order) returning (height, diameter)
================================================================================
IDEA
  DFS returns a pair for each subtree: its height (counted in nodes, with
  null = 0) and the best diameter found inside it. At each node,
  left.height + right.height is the longest path that bends at this node.
  diameter is the max of that path and both child diameters. It is correct
  because every path has exactly one highest node, where it bends, and
  every node is checked as that highest node. Unlike a shared max field,
  the answer travels up inside the tuple, so there is no mutable state.
EXAMPLE
  Tree: 1.left=2; 2.left=3, 2.right=4; 3.left=5; 4.right=6 (1 has no right)
  5,6 -> (1,0); 3 -> (2,1); 4 -> (2,1); 2 -> h=3, through=2+2=4, d=4
  1 -> h=4, through=3+0=3, d=max(3,4)=4
  Answer 4 (path 5-3-2-4-6). It does not pass through the root.
COMPLEXITY
  Time  O(n)  each node is visited once by DFS, with O(1) work per node
  Space O(n)  recursion stack depth equals tree height, up to n when skewed
WATCH OUT
  - Edges, not nodes: height counts nodes, so left.height + right.height is
    already the edge count. Adding +1 here makes the answer off by one.
  - Returning only diameterThroughHere at the root is wrong. You must carry
    Math.Max of left.diameter and right.diameter up, or the example gives 3.
  - A very deep skewed tree (a linked-list shape) can overflow the C# stack.
    An iterative post-order traversal avoids this.
  - The null base case must be (0, 0). A height of -1 would change the sum.
================================================================================
*/
