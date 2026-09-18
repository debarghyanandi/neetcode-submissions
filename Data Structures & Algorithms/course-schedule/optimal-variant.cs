// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n + m) time / O(n + m) space
// -  DFS cycle detection with path tracking   [dfs-cycle-detect]
// -  ties with optimal.cs on O(n + m) time / O(n + m) space
// -
// -  Reference solution - not one you solved yourself (from submission-3)
// -
// -  Each node visited once and each edge traversed once; recursion depth
// -  is at most the number of courses; two boolean arrays track global
// -  completion and active path state.
// --------------------------------------------------------------------------

public class Solution
{
    public bool CanFinish(int numCourses, int[][] prerequisites)
    {
        //Dfs Directed cycle graph detection
        List<int>[] adj = new List<int>[numCourses];
        for (int i = 0; i < numCourses; i++)
            adj[i] = new List<int>();

        foreach (var pair in prerequisites)
        {
            int course = pair[0];
            int prereq = pair[1];
            adj[prereq].Add(course); // prereq must be done before course
        }

        bool[] visited = new bool[numCourses];     // fully explored, safe forever
        bool[] pathVisited = new bool[numCourses]; // currently on this DFS's active path

        for (int i = 0; i < numCourses; i++)
        {
            if (!visited[i])
            {
                if (HasCycle(i, adj, visited, pathVisited))
                    return false;
            }
        }
        return true;
    }

    private bool HasCycle(int node, List<int>[] adj, bool[] visited, bool[] pathVisited)
    {
        visited[node] = true;
        pathVisited[node] = true; // stepping onto this node's path

        foreach (int nei in adj[node])
        {
            if (pathVisited[nei])
                return true; // neighbor is still on OUR current path — that's a cycle

            if (!visited[nei])
            {
                if (HasCycle(nei, adj, visited, pathVisited))
                    return true;
            }
            // if visited[neighbor] is true and pathVisited[neighbor] is false,
            // it was fully explored elsewhere and proven cycle-free — safe to skip
        }

        pathVisited[node] = false; // stepping OFF this node's path before returning
        return false;
    }
}



/*
================================================================================
 PATTERN : Graph cycle detection - DFS with recursion-stack marking
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-3.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
WHY THIS PATTERN
  "Can you finish all courses" is exactly "is this directed graph free of
  cycles". Each pair becomes an edge adj[prereq] -> course, so a cycle means a
  set of courses that each wait on the next forever. DFS with a second flag
  array pathVisited finds a cycle the moment an edge points back to a node still
  open on the current recursion path, so one pass over all nodes and edges
  answers the question.
BRUTE FORCE
  The first thing most people write is: for every course i, run a fresh DFS or
  BFS from i and see if you can walk back to i. That is correct but costs O(n *
  (n + m)) because the whole graph is re-explored numCourses times, and it
  throws away the fact that a node proven cycle-free stays cycle-free. The
  visited array here keeps that fact, which is the whole saving.
INVARIANT
  At any moment inside HasCycle, pathVisited is true for exactly the nodes on
  the current recursion stack - the chain from the starting node down to
  current. visited is true for every node whose DFS has already begun; once
  HasCycle returns false for a node, that node and everything reachable from it
  is proven cycle-free. So seeing pathVisited[neighbor] true means the edge
  current -> neighbor closes a loop back onto the live chain, which is a real
  cycle; and if the outer loop finishes with no such edge, no cycle exists
  anywhere.
WHY SKIPPING A VISITED NODE IS SAFE
  The usual version of this uses three states (white/gray/black). Here two
  booleans do the same job: visited true with pathVisited false is the black
  state. Such a node was fully explored and returned false earlier, so it cannot
  reach a cycle; re-walking it would only repeat work. That is the comment at
  the bottom of the foreach, and it is what turns the repeated-search version
  into a single linear pass.
RESETTING PATHVISITED ON THE WAY OUT
  The line pathVisited[current] = false just before returning false is the easy
  line to forget. Without it, pathVisited would grow into a copy of visited, and
  a diamond shape such as 0->1, 0->2, 1->3, 2->3 would report a cycle that is
  not there. The reset is skipped on the early return true paths, but that is
  harmless since the answer is already decided and the outer loop returns false
  at once.
WATCH OUT
  This is recursive, so the depth equals the longest path in the graph. A single
  chain 0->1->2->...->numCourses-1 gives numCourses nested calls and can
  overflow the call stack on a deep input. Also note the order inside the
  foreach: the pathVisited[neighbor] test must come before the
  !visited[neighbor] test, because a back edge always points at a node that is
  already visited - swapping the two lines would silently skip every cycle. A
  self-loop like [1,1] is handled correctly only because pathVisited[current] is
  set before the neighbor scan starts.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Return an actual course order instead of true or false.
     Push current onto a list right after pathVisited[current] = false, then
     reverse the list at the end - that is a reverse post-order topological
     sort. There the direction adj[prereq] -> course matters, while for the
     yes/no answer it does not, since reversing every edge keeps the same
     cycles.
  2. Remove the recursion so a deep graph cannot blow the stack.
     Either use an explicit Stack<int> plus a per-node index into adj so you
     know when a node's children are done and pathVisited can be cleared, or
     switch to Kahn's algorithm: compute in-degrees, queue every node with
     in-degree 0, and check that the number of popped nodes equals numCourses.
     Kahn's is iterative by nature and the same cost, but needs an extra
     indegree array.
  3. What if the input has duplicate pairs, or numCourses is larger than any
  course number that appears?
     Duplicate edges are harmless; visited makes the second copy a cheap skip,
     so the answer does not change. Isolated courses are covered because the
     outer loop starts a DFS from every index 0..numCourses-1, not only from
     nodes named in prerequisites.
  4. The graph is far too big to keep as one list per node - what changes?
     Store edges in a compact layout instead: one int array of targets plus an
     offset array giving each node's slice, built with a counting pass. The
     algorithm is unchanged; only how adj is stored differs.
TRIGGER
  A dependency or ordering question whose answer is just "is it possible" -
  build the directed graph and look for an edge back into the live DFS path.
C# NOTE
  List<int>[] must be filled in by the first for loop; a fresh array of
  reference types holds nulls, so dropping that loop gives a
  NullReferenceException on the first Add. Using two bool[] instead of a
  HashSet<int> for visited and pathVisited keeps every check a direct index read
  with no hashing.
COMPLEXITY
  Time  : O(n + m)
  Space : O(n + m)
================================================================================
*/
