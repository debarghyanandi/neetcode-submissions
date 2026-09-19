// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// -  BFS with explicit queue level-order   [bfs-queue-levels]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  Queue stores at most the maximum level width; worst-case width is
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
 PATTERN : BFS level order - null-padded queue, one level per pass
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  result   result[d] = list of values at depth d, top to bottom
  queue    nodes of the current level, plus null slots pushed by leaves
  level    values collected for the depth being processed right now
  i        counts down from the frozen size of the current level
WHY THIS PATTERN
  The problem asks for the node values grouped by depth, left to right, which is
  exactly the order a breadth-first search visits them. A queue gives that order
  for free; the only extra work is knowing where one depth ends and the next
  begins. This code gets that boundary by reading queue.Count once per while
  pass, so every node taken out during the inner for loop belongs to the same
  depth and lands in the same level list.
BRUTE FORCE
  The naive version first measures the tree height, then for each depth d walks
  the whole tree from the root and collects only nodes at depth d. That is O(n)
  work per level, so O(n * h) total, degrading to O(n^2) on a skewed tree. BFS
  touches every node once instead, and the level boundary comes from a counter
  rather than from re-walking.
INVARIANT
  At the top of each while pass, the queue holds exactly the entries of one
  depth, in left-to-right order (real nodes plus null placeholders left by
  leaves at the previous depth). The inner loop removes exactly that many
  entries and appends the children of each real node, so when the loop ends the
  queue holds exactly the next depth, again in order. Since result gets one list
  appended per pass, result[d] is the values at depth d.
NULL SLOTS ARE ALLOWED IN THE QUEUE
  Unlike the usual version, children are enqueued without checking for null; the
  null test happens after Dequeue. This keeps the enqueue side branch-free at
  the cost of putting up to two nulls in the queue per leaf. The consequence is
  that the last pass dequeues a batch made only of nulls and builds an empty
  level, which is why the `if (level.Count > 0)` guard exists - without it the
  answer would end with a stray empty list.
THE COUNTDOWN FREEZES THE LEVEL WIDTH
  `for (int i = queue.Count; i > 0; i--)` reads queue.Count once, in the
  initializer, so the growth caused by Enqueue inside the body cannot extend the
  loop. That single read is what separates one depth from the next.
WATCH OUT
  Rewriting the inner loop as `for (int i = 0; i < queue.Count; i++)` re-reads
  Count every iteration and silently merges levels - this is the classic break
  of this code. The `if (root == null) return result;` guard is redundant:
  enqueueing a null root would dequeue one null, build an empty level, and fall
  through the Count > 0 guard to the same empty result; it is harmless, just not
  load-bearing. The null-padding also means the queue can hold up to twice the
  widest level, so peak memory is larger than a null-checking BFS. Finally, an
  inner level list is added whenever it is non-empty, so a depth with a mix of
  real nodes and null slots is fine - only the all-null tail batch is dropped.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How would you return the levels bottom-up?
     Build result the same way and call result.Reverse() at the end, or insert
     each level at index 0. Reverse is O(n) once; Insert(0, level) is O(levels)
     per level because List shifts elements.
  2. Zigzag order - left to right, then right to left?
     Keep the same loop and track a bool flipped, toggled once per while pass;
     when it is true, call level.Reverse() before adding, or fill level back to
     front. The queue logic never changes.
  3. Can you do this without a queue?
     Yes, recursive DFS passing a depth: if depth == result.Count add a new
     list, then result[depth].Add(node.val). Memory drops to O(h) call stack
     instead of O(width) queue, but on a skewed tree h is n and you risk stack
     overflow.
  4. You only need the last value of each level (right side view) - what
  changes?
     Keep the counting loop, but inside it only record the value when i == 1,
     that is, the last real entry of the batch. Output shrinks to O(levels)
     while the queue cost stays the same.
TRIGGER
  The problem asks for tree or graph nodes grouped by distance from the start,
  or says "level by level".
C# NOTE
  Queue<TreeNode> is the right structure here: Enqueue and Dequeue are amortized
  O(1) on its internal circular array, while a List with RemoveAt(0) would shift
  every element. Note that LeetCode's C# signature is usually IList<IList<int>>,
  and List<List<int>> does not convert to it - generics are not covariant that
  way - so if the judge rejects the return, declare result as IList<IList<int>>
  and add List<int> items.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
