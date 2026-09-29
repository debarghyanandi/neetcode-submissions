// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n + m) time / O(n + m) space
// -  DFS cycle detection, path-visit tracking   [dfs-cycle-detection]
// -  ties with optimal.cs on O(n + m) time / O(n + m) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  DFS visits each node and edge once; call stack depth bounded by
// -  longest dependency chain.
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
 PATTERN : DFS Cycle Detection - on-path marker in a directed graph
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-3.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  adj          adj[p] = list of courses that need course p first (edge p -> course)
  visited      visited[v] = true once DFS has entered v at any time
  pathVisited  pathVisited[v] = true while v is on the current DFS path (the recursion stack)
  nei          a course that depends on the current node
WHY THIS PATTERN
  Each prerequisite pair says "take one course before another", so the input is
  a directed graph. You can finish all courses only if no chain of prerequisites
  loops back to itself. So the question is really "does this directed graph have
  a cycle?" DFS finds a cycle when it meets a node that is still in pathVisited,
  which means the node is still waiting for its own DFS call to finish.
BRUTE FORCE
  From every course, run a separate DFS or BFS and check whether you can get
  back to the start course. This is correct, but it costs O(n * (n + m)),
  because the same parts of the graph are explored again for each start course.
  This file checks each node only once: the shared visited array means a node
  that has been fully explored is never explored again.
INVARIANT
  At every moment, pathVisited is true for exactly the nodes on the current
  recursion stack. The code sets it on entry and clears it just before HasCycle
  returns false. Because of this, an edge to a node with pathVisited true is a
  back edge (an edge to a node that is still being explored). A back edge closes
  a loop, so returning true is correct. If a node was entered before and is no
  longer on the path, its whole subtree already finished with no cycle, so
  skipping it cannot miss a cycle.
A DIAMOND IS NOT A CYCLE
  Say 0 -> 1, 0 -> 2, 1 -> 3 and 2 -> 3. Node 3 is reached twice, but there is
  no loop. If you checked only visited, the second visit to 3 would be wrongly
  reported as a cycle. That is why the code needs pathVisited as well. Only
  "still on the current path" means a cycle.
WATCH OUT
  The comment on visited says "fully explored, safe forever", but the code sets
  visited[node] = true when it enters the node, not when it finishes. The result
  is still correct only because the pathVisited check runs first inside the
  loop. If someone swaps those two if-checks, the comment's promise breaks.
  HasCycle is recursive, so a very long prerequisite chain means a very deep
  call stack, and this can cause a StackOverflowException, which cannot be
  caught. On an early return true, pathVisited is left dirty. That is fine here
  only because CanFinish stops right away.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you avoid recursion?
     Use Kahn's algorithm (BFS on in-degrees). Count in-degrees, put every
     course with in-degree 0 in a queue, and remove courses one by one. If the
     number removed is less than numCourses, there is a cycle. You need an extra
     in-degree array, but there is no stack depth risk.
  2. Can you return a valid order of courses (Course Schedule II)?
     Add a node to a list just before HasCycle returns false (post-order), then
     reverse the list. With edges going prereq -> course, the reversed
     post-order is a valid topological order (an order where every course comes
     after its prerequisites).
  3. Can you use one array instead of two bool arrays?
     Yes. Use one int state array: 0 = unvisited, 1 = on the path, 2 = done. The
     logic is the same, just in a single array.
  4. How do you report the actual cycle?
     Keep a parent array while you go down the DFS. When you find a back edge
     from node to nei, follow the parents from node back up to nei.
TRIGGER
  The problem gives "X must come before Y" dependencies and asks whether all of
  them can be met, or asks for an order that meets them.
C# NOTE
  List<int>[] indexed by course number fits here better than a Dictionary<int,
  List<int>>, because the courses are exactly 0..numCourses-1. Creating every
  list up front means courses with no dependents need no null check in HasCycle.
COMPLEXITY
  Time  : O(n + m)
  Space : O(n + m)
================================================================================
*/
