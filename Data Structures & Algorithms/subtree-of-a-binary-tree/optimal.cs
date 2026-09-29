// ##########################################################################
// #  optimal.cs            O(n * m) time / O(n + m) space
// ##########################################################################

public class Solution
{
    //My solution 
    public bool IsSubtree(TreeNode root, TreeNode subRoot)
    {
        if (root == null && subRoot != null)
            return false;

        if (IsSameTree(root, subRoot))
            return true;

        else
        {
            return IsSubtree(root.left, subRoot) || IsSubtree(root.right, subRoot);
        }
    }

    public bool IsSameTree(TreeNode first, TreeNode second)
    {
        if (first == null && second == null)
            return true;

        if (first != null && second != null && first.val == second.val)
            return IsSameTree(first.left, second.left) && IsSameTree(first.right, second.right);
        return false;
    }
}

/*
================================================================================
 PROBLEM : You get two binary trees, root and subRoot. Return true if some
           node in root has a subtree that is exactly subRoot: same values and
           same shape, all the way down to the leaves. root=[3,4,5,1,2],
           subRoot=[4,1,2] -> true.
 PATTERN : DFS + tree equality check at every node
================================================================================
IDEA
  IsSubtree walks every node of root with DFS. At each node it asks
  IsSameTree(root, subRoot). IsSameTree returns true only if both nodes are
  null, or if both have equal val and both child pairs also match.
  If the check fails, the search goes on in root.left and root.right.
  It is correct because any matching subtree must start at some node of
  root, and this code tries every node as that start.
EXAMPLE
  root=[3,4,5,1,2,null,null,null,null,0] (node 2 has left child 0),
  sub=[4,1,2]
  Node 3: 3!=4, fail. Node 4: 4=4, 1=1, 2=2, but 0 vs null -> fail.
  Nodes 1, 2, 0, 5: val != 4, fail. Null children -> false.
  Answer: false (4 matched at the top, but the leaf shape differed).
COMPLEXITY
  Time  O(n * m)  each of n nodes may start an IsSameTree that checks up to m
                  nodes
  Space O(n + m)  recursion depth is height of root plus height of subRoot
PATH TO OPTIMAL
  Compare at every node (this file) - O(n*m) - simple, the expected answer.
  Serialize both trees with null markers, then KMP - O(n+m) - linear.
KEYWORDS
  binary tree, subtree, same tree, DFS, recursion, serialization, KMP
WATCH OUT
  - Matching only values is wrong. Leaves must match too: the 0 under
    node 2 in the example makes the answer false.
  - Remove the root==null check and root.left throws a null reference.
  - Duplicate values: a node can match val but fail below. You must keep
    searching its children, as the || in IsSubtree does.
  - A very deep, skewed tree can overflow the call stack.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it in linear time?
     -> Serialize both trees in preorder with null markers, then use KMP to
        search. O(n+m) time and space, but more code to write.
  2. What can break the string approach?
     -> Values like 2 and 12 blur together. Put a separator before each value
        (",12") and write null as "#".
  3. Can hashing help?
     -> Give each subtree a hash built from val and child hashes (Merkle
        style). Compare hashes in O(1). O(n+m) time, with rare collisions.
  4. What if the tree is very deep?
     -> Use an iterative DFS with an explicit stack. The time stays the same,
        and memory moves to the heap, so no stack overflow.
TRIGGER
  When asked whether one tree appears inside another, run a same-tree check
  from every node, and use serialization plus string matching for linear time.
================================================================================
*/
