// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
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
 PATTERN : BFS level order - snapshot the queue size per level
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  queue    nodes waiting to be visited, including null children
  i        counts down from the queue size taken when the level starts
  level    values of the non-null nodes at the current depth
WHY THIS PATTERN
  The problem asks for values grouped by depth, from top to bottom and left to
  right. A FIFO queue (first in, first out) visits nodes in exactly that order.
  When a new level starts, queue holds that whole level and nothing else. The
  children added while that level is processed go behind it, so they become the
  next level.
BRUTE FORCE
  First find the tree height. Then, for each depth d, walk down from root and
  collect the nodes at depth d into their own list. This is correct, but every
  pass starts again at the root. That costs O(n*h) time, where h is the height,
  so O(n^2) on a tree that is one long chain. The queue visits each node only
  once.
INVARIANT
  At the top of each while pass, queue holds exactly the entries of one depth,
  in left-to-right order. Some of those entries may be null. The for loop
  removes exactly those entries and adds their children in the same order. So
  when the pass ends, queue again holds exactly one full depth, in order. This
  means each level list gets the right values in the right order, and result
  gets the levels top to bottom.
THE LOOP BOUND IS READ ONCE
  In "for (int i = queue.Count; i > 0; i--)", queue.Count is read only once,
  when the loop starts. The children added inside the loop do not change i. That
  is why the loop stops at the edge of the level. If you wrote "i < queue.Count"
  as the loop condition, it would read the growing count on every step, and
  levels would mix together.
WATCH OUT
  The code adds node.left and node.right even when they are null. Every real
  node adds two entries, so the queue also carries about n+1 nulls. The last
  while pass removes only nulls and builds an empty level. The "level.Count > 0"
  check is the only thing that keeps an empty list off the end of result, so if
  you remove that check the output is wrong. A cleaner fix is to add a child
  only when it is not null. Then the "node != null" check and the empty-level
  check are both no longer needed.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you return the levels bottom-up?
     Build result the same way and reverse it at the end, or insert each level
     at index 0. Reversing once costs O(n) in total. Inserting at the front
     shifts the whole list every time, which costs O(levels^2).
  2. How do you do zigzag order (left to right, then right to left)?
     Keep a bool that flips after each level. Reverse level before adding it, or
     fill it from the back. The queue order stays the same.
  3. Can you do this with DFS instead?
     Yes. Recurse with a depth parameter. If depth == result.Count, add a new
     list, then add the value to result[depth]. Visit left before right. The
     call stack uses O(h) space instead of the queue's width. On a very deep
     chain, the recursion can cause a stack overflow.
  4. How do you get the right side view?
     Use the same level loop and keep only the last non-null node you remove in
     each level (the one removed when i == 1).
TRIGGER
  The problem asks for tree nodes grouped by depth, or asks for something per
  level (average, max, rightmost), so reach for BFS with a snapshot of the level
  size.
C# NOTE
  This method returns List<List<int>>. LeetCode's usual signature is
  IList<IList<int>>, and C# will not convert List<List<int>> to
  IList<IList<int>> because generic types in C# are invariant (a list of a
  subtype is not a list of the base type). If you must match that signature,
  declare result as new List<IList<int>>().
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
