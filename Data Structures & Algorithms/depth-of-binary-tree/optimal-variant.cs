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
 PATTERN : BFS Level Order - count the levels of the tree
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-1.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  queue      nodes still waiting to be visited, in level order
  level      number of levels fully processed so far
  levelSize  number of nodes in the current level, read before the for loop starts
WHY THIS PATTERN
  The problem asks for the number of nodes on the longest path from the root
  down to a leaf. That is the same as the number of levels in the tree. BFS
  (breadth-first search: visit all nodes at one distance before going deeper)
  handles the tree one level at a time. So every pass of the while loop is one
  level, and level++ counts it. When the queue is empty, no deeper level exists,
  and level is the depth.
BRUTE FORCE
  The first version most people write is recursive DFS (depth-first search:
  follow one branch to the bottom first): return 0 for null, otherwise 1 +
  max(MaxDepth(left), MaxDepth(right)). It is also O(n) time. Its space is the
  call-stack height, which is O(n) on a tree that is one long chain. It is short
  and correct. The weak point is that deep recursion can overflow the stack on a
  very skewed tree. BFS uses no recursion at all.
INVARIANT
  At the top of each while pass, queue holds exactly the nodes of one level and
  nothing else, and level equals the number of levels above it. The for loop
  removes exactly levelSize nodes, so it takes out only that level and adds only
  their children, which are the next level. By induction, when the queue becomes
  empty, level equals the number of levels that had at least one node, and that
  is the max depth.
WATCH OUT
  levelSize must be saved before the for loop starts. If you write i <
  queue.Count, the loop condition sees the children added during the loop, so
  one pass mixes levels and the count is too small. The null check on root
  matters: without it, an empty tree would enqueue null and then crash on
  node.left. With the check, an empty tree returns 0 as it should.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How would you find the minimum depth instead?
     Use the same BFS, but return level + 1 as soon as you dequeue a leaf,
     meaning a node with no left and no right child. BFS is better than DFS here
     because it stops at the shallowest leaf and does not look at deeper parts
     of the tree.
  2. Can you do it with DFS but no recursion?
     Use an explicit Stack of (node, depth) pairs and keep a running max of
     depth. Memory becomes O(h), where h is the tree height, instead of the
     width of the widest level. The cost is extra code to carry the depth with
     each node.
  3. What if you must return the nodes of each level, not just the count?
     Inside the for loop, add each node.val to a list for that level, and add
     the list to the result after the loop. The levelSize loop structure stays
     the same.
TRIGGER
  If the problem talks about levels, depth or the shortest number of steps in a
  tree or graph, reach for BFS with a levelSize loop.
C# NOTE
  Queue<TreeNode> is a circular buffer, so Dequeue costs O(1). Using a
  List<TreeNode> with RemoveAt(0) instead would shift the whole list on every
  removal and make the method O(n^2).
COMPLEXITY
  Time  : O(n)
  Space : O(n)
================================================================================
*/
