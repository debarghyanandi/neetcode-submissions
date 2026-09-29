// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n + m) time / O(n + m) space
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
 PROBLEM : There are numCourses courses labeled 0..numCourses-1. Each pair [a,
           b] in prerequisites means you must take course b before course a.
           Return true if you can finish all courses, meaning there is no
           cycle. Example: 2, [[1,0],[0,1]] -> false (each course waits on the
           other).
 PATTERN : DFS cycle detection in a directed graph (3-state)
================================================================================
IDEA
  Build adj so that adj[prereq] lists every course that needs it.
  Run DFS from each unvisited node. pathVisited marks nodes on the current
  recursion path, and visited marks every node DFS has entered.
  Meeting a node with pathVisited set is a back edge, which means a cycle.
  A node that is visited but off the path was fully explored with no cycle.
  So skipping it is safe. optimal.cs does the same job with a different
  method.
EXAMPLE
  4, [[1,0],[2,1],[2,0],[3,3]] -> adj[0]=[1,2], adj[1]=[2], adj[3]=[3]
  DFS(0): path {0} -> 1: path {0,1} -> 2: no neighbors, unmark 2, then 1.
  Back at 0: nei 2 is visited but off the path, so skip it. Unmark 0.
  DFS(3): path {3}, nei 3 is on the path (self-loop) -> cycle -> false.
COMPLEXITY
  Time  O(n + m)  each node enters HasCycle once; each edge is scanned once
  Space O(n + m)  adj stores all edges; visited, pathVisited and recursion
                  depth are O(n)
WATCH OUT
  - If you use only visited, the diamond 0->1->2 plus 0->2 looks like a
    cycle. You need pathVisited to tell "on the path" from "already done".
  - Forgetting pathVisited[node] = false before return false makes every
    later path that reaches an explored node report a false cycle.
  - Recursion depth is the longest chain. A long line of courses can
    overflow the C# stack. Kahn's BFS or an explicit stack avoids this.
  - The comment in HasCycle talks about "neighbor", but the variable is nei.
================================================================================
*/
