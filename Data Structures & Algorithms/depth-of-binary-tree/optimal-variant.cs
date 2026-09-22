// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n) time / O(n) space
// -  Iterative breadth-first search   [iterative-bfs]
// -  ties with optimal.cs on O(n) time / O(n) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  Iteratively processes each of n nodes level by level; queue size
// -  reaches maximum width of tree, worst case O(n).
// --------------------------------------------------------------------------

//Breadth-First Search (BFS) find level.
public class Solution
{
    public int MaxDepth(TreeNode root)
    {
        Queue<TreeNode> queue = new Queue<TreeNode>();
        if (root != null)
        {
            queue.Enqueue(root);
        }

        int level = 0;
        while (queue.Count > 0)
        {
            int levelSize = queue.Count;
            for (int i = 0; i < levelSize; i++)
            {
                TreeNode node = queue.Dequeue();
                if (node.left != null)
                {
                    queue.Enqueue(node.left);
                }
                if (node.right != null)
                {
                    queue.Enqueue(node.right);
                }
            }
            level++;
        }
        return level;
    }
}

/*
================================================================================
 PATTERN : BFS level-order traversal - count levels
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  queue        nodes of the current frontier waiting to be expanded
  levelSize    how many nodes belong to the level being processed right now
  level        number of levels fully drained so far; the answer at the end
WHY THIS PATTERN
  Maximum depth is the number of levels from root to the deepest leaf, and BFS
  visits a tree exactly one level at a time. By reading queue.Count into
  levelSize before the inner loop, the code freezes the size of the current
  level, so the inner for loop pops exactly that level and the children it
  enqueues form the next one. Every completed inner loop means one more full
  level existed, so level is incremented once per level and ends equal to the
  depth.
BRUTE FORCE
  The first thing most people write is recursion: return 0 for null, otherwise 1
  + max(MaxDepth(root.left), MaxDepth(root.right)). That is also linear time,
  and its space is the recursion stack, which is the tree height. It does not
  lose on complexity - it loses only when the tree is a long chain and the call
  stack overflows, which is the reason to prefer this explicit queue.
INVARIANT
  At the top of each while iteration, the queue holds exactly the nodes at depth
  level (0-indexed), and nothing else. The inner loop removes all levelSize of
  them and enqueues all their non-null children, so the queue then holds exactly
  depth level+1 before level++ restores the invariant. The loop stops when a
  level has no children, meaning the deepest level was just counted, so level is
  the true depth.
WATCH OUT
  The null check happens twice in different places: root is guarded before the
  first Enqueue, and children are guarded before theirs, so a null never enters
  the queue. If the early guard on root were dropped, a null root would be
  enqueued and node.left would throw NullReferenceException; the empty tree
  returns 0 only because of that one if. Reading queue.Count fresh inside the
  for condition instead of caching it in levelSize would be a real bug - the
  count grows as children are added, and the loop would swallow several levels
  as one. Nothing here caps the queue, so a very wide level holds many nodes at
  once.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How would you find the minimum depth instead?
     Same loop, but return level+1 the moment you dequeue a node with both
     children null. BFS reaches the shallowest leaf first, so it stops early
     instead of scanning the whole tree.
  2. Can you drop the queue and use less memory?
     Use the recursive 1 + max(left, right) form; space becomes the height
     instead of the widest level. Better for wide bushy trees, worse for a
     skewed chain because of stack depth.
  3. The tree has n children per node instead of two - what changes?
     Replace the two if blocks with a foreach over node.children that enqueues
     each non-null child. The level counting logic is untouched.
  4. Return the node values grouped by level, not just the count?
     Build a List<int> inside the while, add node.val in the inner loop, and add
     that list to a result list where level++ is now. levelSize is what makes
     the grouping correct.
TRIGGER
  Reach for this when the question asks about distance, depth, or anything "per
  level" in a tree or graph and you want to avoid deep recursion.
C# NOTE
  Queue<T> gives O(1) Enqueue and Dequeue, which is what the per-level loop
  needs; using a List<TreeNode> with RemoveAt(0) instead would shift elements on
  every pop and make the traversal quadratic.
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
