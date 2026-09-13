// ##########################################################################
// #  optimal.cs            O(n) time / O(n) space
// #  DFS recursion tracking depth as level index   [dfs-track-level]
// #  ties with optimal-variant.cs on O(n) time / O(n) space
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Visits each node once via preorder DFS, appending to the list for its
// #  depth; recursion stack depth is O(h) which is O(n) worst case for a
// #  skewed tree.
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
 PATTERN : DFS preorder carrying a depth index into res[level]
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  Level order is usually reached for with a queue, but the queue only exists to
  discover which level a node belongs to. Here the level is already known: it is
  handed down the recursion as the second parameter of Traverse, incremented
  once per edge. Once you carry the depth explicitly, you no longer need to
  visit nodes in level order at all - you only need each node to land in the
  right bucket. That frees the traversal to be a plain preorder recursion, which
  is shorter than the BFS version and has no queue bookkeeping.
INVARIANT
  At every call to Traverse(root, level) with root != null: res.Count >= level,
  and res.Count == level exactly when this is the first node ever visited at
  that depth.

  The lower bound holds by induction. The root is called with level 0 and
  res.Count >= 0 trivially. Any child is called with level + 1, and the parent
  has just executed the res.Add / res[level].Add pair, so res.Count >= level + 1
  when the child runs. res.Count is therefore never smaller than level, which is
  what makes res[level].Add safe - the index is always in range after the guard.

  The equality half is the trick: lists are only ever appended to the end of
  res, so res.Count is the number of levels touched so far, i.e. 1 + the deepest
  level seen. res.Count == level means this level has not been reached yet, so
  create its list. If res.Count > level, the list already exists and we just
  append.
ALGORITHM
  1. If root is null, return res as-is (empty on a fresh Solution).
  2. Call Traverse(root, 0).
  3. In Traverse: null check, return.
  4. If res.Count == level, push a new empty List<int> - this is the first node
  at this depth.
  5. Append root.val to res[level].
  6. Recurse into root.left with level + 1, then root.right with level + 1.
  7. Return res.
WHY THE ORDER WITHIN A LEVEL IS CORRECT
  This is the follow-up an interviewer will push on, because left-to-right
  output is not obviously preserved once you abandon BFS.

  Take two nodes u and v at the same depth with u to the left of v. Walk up from
  both until their paths meet at their lowest common ancestor a. u descends into
  a.left and v into a.right - if they descended the same way, a would not be the
  meeting point. Preorder visits the entire a.left subtree before touching
  a.right, so u is visited before v. Appends to res[level] happen in visit
  order, so u.val sits before v.val in that list.

  The argument rests entirely on the line order in step 6: left recursed before
  right. Swap those two lines and every level comes out mirrored. It is the only
  place the output order is enforced.
WATCH OUT
  res is an instance field initialized at construction, not a local. Traverse
  mutates it as a side effect and LevelOrder never clears it. Call LevelOrder
  twice on the same Solution object and the second call appends into the lists
  left behind by the first, producing garbage. The judge constructs a fresh
  Solution per test case so this passes, but it is the first thing a reviewer
  flags. Fix is one line: res = new List<List<int>>() at the top of LevelOrder,
  or make res a local and pass it into Traverse.

  The null-root early return has the same smell - it hands back whatever res
  currently holds rather than a guaranteed empty list.

  Separately, recursion depth tracks the height of the tree, so a degenerate
  chain of left children can exhaust the call stack. The queue-based BFS is
  iterative and has no such ceiling; that is the real argument for preferring it
  outside an interview.
TRIGGER
  Reach for depth-carrying DFS whenever the answer is indexed by level but does
  not require processing levels in order: bottom-up level order (build res
  identically, then reverse it), right side view (keep the last value written
  into each res[level], or recurse right before left and take the first), max
  level sum, level averages. Reach for a real BFS queue instead when you must
  stop early at some level, need the node count of the current level while you
  are processing it, or cannot risk the stack depth.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
