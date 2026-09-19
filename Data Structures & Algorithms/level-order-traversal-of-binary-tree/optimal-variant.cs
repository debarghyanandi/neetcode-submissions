// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// -  BFS with queue, level-order traversal   [bfs-queue]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  Each node visited once; queue holds maximum tree width, worst case
// -  O(n).
// --------------------------------------------------------------------------

public class Solution
{
    public List<List<int>> LevelOrder(TreeNode root)
    {
        List<List<int>> result = new List<List<int>>();

        if (root == null)
            return result;

        Queue<TreeNode> queue = new Queue<TreeNode>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            List<int> level = new List<int>();

            for (int i = queue.Count; i > 0; i--)
            {
                TreeNode node = queue.Dequeue();
                if (node != null)
                {
                    level.Add(node.val);
                    queue.Enqueue(node.left);
                    queue.Enqueue(node.right);
                }
            }
            if (level.Count > 0)
            {
                result.Add(level);
            }
        }
        return result;
    }
}

/*
================================================================================
 PATTERN : BFS Level Order Traversal - queue with level-size snapshot
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  queue    BFS frontier; holds real nodes AND null child placeholders
  level    values of the nodes popped in the current round
  i        counts down from the queue size captured at the start of this round
WHY THIS PATTERN
  The problem asks for the values grouped by depth, one inner list per level.
  Breadth-first search visits nodes in exactly depth order, so the only extra
  work is knowing where one level ends. Capturing queue.Count into i before the
  inner loop freezes the size of the current level, so everything added during
  that loop belongs to the next level and is not consumed early. Each round
  fills one level list and appends it to result.
BRUTE FORCE
  The simple first attempt is a depth-first recursion that carries a depth
  argument and does result[depth].Add(node.val), creating a new inner list the
  first time a depth is seen. That is also linear in time and space, so it does
  not lose on complexity; what it loses is control - it needs the explicit depth
  parameter and the recursion stack goes as deep as the tree, which can be the
  node count on a skewed tree. An actually worse attempt is computing the height
  first, then walking the tree once per level to collect that level, which is
  O(n * height).
INVARIANT
  At the top of each while iteration, the queue holds exactly the entries
  produced by the previous level - real nodes at the current depth, plus null
  slots where a child was missing. The inner loop pops exactly i of them, so it
  never touches the children it just pushed. Therefore every value added to
  level comes from one depth, and levels are appended to result in increasing
  depth order.
NULLS ARE ENQUEUED, NOT FILTERED
  Unlike the common version, left and right are pushed without checking for
  null; the null check happens after the Dequeue. This is why the guard if
  (level.Count > 0) exists: the round after the last real level pops only nulls
  and would otherwise append an empty list to result. The guard is load-bearing,
  not defensive polish.
WATCH OUT
  Because nulls go into the queue, the queue can hold up to about twice the real
  level width, and the algorithm always runs one extra full round that pops
  nulls and produces nothing. If someone "cleans up" the code by moving the null
  test to the Enqueue side but leaves the level.Count > 0 guard, it still works;
  if someone removes the guard while keeping null enqueues, every answer gains a
  trailing empty list. Also note the loop bound is read once at initialization -
  rewriting it as for (int i = 0; i < queue.Count; i++) is wrong, because
  queue.Count grows inside the loop.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How would you return the levels bottom-up (deepest level first)?
     Build result exactly as here and then call result.Reverse(), or insert each
     level at index 0. Reverse is O(n) once; Insert(0, level) is O(levels)
     shifting per level, so reverse at the end is cheaper.
  2. Zigzag order - left to right, then right to left, alternating?
     Keep the same loop and flip a bool each round; when the flag is set,
     reverse level before adding it, or fill it back-to-front into a pre-sized
     list. Traversal logic is untouched.
  3. Can you cut the memory the queue uses?
     Replace the queue with two lists, current and next: iterate current, push
     children into next, then swap. This holds at most two levels instead of a
     queue that mixes levels, and it also drops the need for the size snapshot.
  4. The tree is huge and you only need the last level, or the right side view?
     Keep the same BFS but do not store every level - for the last level
     overwrite a single list each round; for the right side view take only the
     final non-null node of each round. Space drops to the width of one level.
TRIGGER
  The answer must be grouped by distance from a start point, or asks about
  depth, width or "first time reached" - reach for BFS with a level-size
  snapshot.
C# NOTE
  Queue<TreeNode> accepts null happily because TreeNode is a reference type,
  which is what makes this enqueue-then-check style legal; under nullable
  reference types you would need Queue<TreeNode?> to avoid warnings. Minor win
  available: new List<int>(i) to pre-size level, since the snapshot already
  tells you the upper bound on this level's width.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
