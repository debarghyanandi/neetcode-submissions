// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// #  DFS recursion, pre-order traversal   [dfs-recursion]
// #  ties with optimal-variant.cs on O(n) time / O(n) space
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Each node visited once; space bounded by call stack depth, worst case
// #  O(n) in skewed tree.
// ##########################################################################

public class Solution
{
    private List<List<int>> res = new List<List<int>>();

    public List<List<int>> LevelOrder(TreeNode root)
    {
        //My Solution
        if (root == null)
            return res;

        Traverse(root, 0);
        return res;
    }

    private void Traverse(TreeNode root, int level)
    {
        if (root == null)
            return;
        // When we first reach a new level, level == res.Count.
        // For example, at the first node of level 1, both are 1,
        // so we create a new list for that level.

        // When we reach the next node at the same level,
        // level is still 1, but res.Count is now 2 because
        // the list for level 1 was already created.
        if (res.Count == level)
            res.Add(new List<int>());

        res[level].Add(root.val);

        Traverse(root.left, level + 1);
        Traverse(root.right, level + 1);
    }
}

/*
================================================================================
 PATTERN : DFS preorder with depth index - one list per level
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  res      res[d] = values of all nodes at depth d, from left to right
  level    depth of the current node (the root is 0)
WHY THIS PATTERN
  The problem asks for the node values grouped by depth, with each group in
  left-to-right order. You do not need a queue for this. You only need to know
  the depth of each node when you visit it. Traverse passes level + 1 to each
  child, so every node knows its depth. It then adds its value to res[level].
BRUTE FORCE
  A simple correct first idea has two passes. First, find the height h of the
  tree. Then, for each depth d from 0 to h-1, walk the tree from the root and
  collect only the nodes at depth d. This costs O(n*h) time, and O(n^2) on a
  skewed tree (a tree that is one long chain). It loses because it walks the
  upper levels again for every depth. This file visits each node only once.
INVARIANT
  When Traverse reaches a node at depth level, its parent at level - 1 was
  visited earlier. So res already holds lists 0..level-1, and res.Count is at
  least level. Because of this, the check res.Count == level is true exactly
  once per depth: at the first node we reach at that depth. That is the moment
  the new list is created. The code visits the node first, then the left child,
  then the right child (preorder). So nodes at the same depth are added from
  left to right, and every res[level] ends up in the correct order.
WATCH OUT
  res is a field of the class, not a local variable. If LevelOrder is called
  twice on the same Solution object, the second result also contains the first
  tree's levels. The comment says "res.Count is now 2" at the next node of level
  1. That is not always true: the left subtree may already have created lists
  for deeper levels, so res.Count can be 3 or more. The code is still correct,
  because it only checks for equality with level. The root == null check in
  LevelOrder does nothing extra, because Traverse already returns early on null.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you write it without recursion?
     Use BFS (breadth-first search) with a Queue<TreeNode>. At the start of each
     level, save count = queue.Count, then dequeue exactly count nodes into a
     new list. This removes the call stack, which matters on a very deep tree.
     The cost is that the queue can hold a whole level at once, up to about n/2
     nodes.
  2. Zigzag order, where every other level goes right to left?
     Keep this same DFS. When level is odd, insert at the front of res[level]
     instead of the end. On a List<int>, inserting at the front costs O(k) per
     insert, so either use a LinkedList or reverse the odd lists once at the
     end.
  3. Right side view, meaning the last node of each level?
     Keep the same res.Count == level check, but visit the right child before
     the left child and store only one value per level. The first node you reach
     at each depth is then the rightmost one.
  4. Bottom-up level order?
     Build res exactly as now and reverse it once at the end. Or use BFS and
     insert each finished level at the front.
TRIGGER
  The output groups nodes by their depth in a tree, so passing a depth number
  down the recursion can replace a BFS queue.
C# NOTE
  res[level] on a List<T> is an O(1) indexed read (it reads straight from an
  internal array), so adding to any earlier level while the DFS is deeper in the
  tree is cheap. This code returns List<List<int>>. If the judge's signature is
  IList<IList<int>>, you must declare the outer list as List<IList<int>>,
  because C# does not convert List<List<int>> to IList<IList<int>>.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
