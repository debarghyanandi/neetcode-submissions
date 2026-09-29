// ##########################################################################
// #  optimal.cs            O(n + m) time / O(n + m) space
// ##########################################################################

public class Solution
{
    public int[] FindOrder(int numCourses, int[][] prerequisites)
    {
        // My solution
        // Bfs Kanhs algo
        // Build the adjacency list
        List<int>[] adj = new List<int>[numCourses];
        for (int i = 0; i < numCourses; i++)
        {
            adj[i] = new List<int>();
        }

        int[] inDeg = new int[numCourses];

        // Build the graph + inDeg
        foreach (int[] pair in prerequisites)
        {
            int course = pair[0];
            int prereq = pair[1];
            adj[prereq].Add(course);
            inDeg[course]++;
        }


        Queue<int> q = new Queue<int>();
        // Insert all the nodes whose inDeg is 0;
        for (int i = 0; i < numCourses; i++)
        {
            if (inDeg[i] == 0)
            {
                q.Enqueue(i);
            }
        }

        List<int> topo = new List<int>();

        while (q.Count > 0)
        {
            int node = q.Dequeue();
            topo.Add(node);

            // Remove this node from its neightbours inDeg
            foreach (int nei in adj[node])
            {
                inDeg[nei]--;
                if (inDeg[nei] == 0)
                    q.Enqueue(nei);
            }
        }

        return (topo.Count == numCourses) ? topo.ToArray() : Array.Empty<int>();
    }
}

/*
================================================================================
 PROBLEM : There are numCourses courses labeled 0..numCourses-1. Each pair [a,
           b] in prerequisites means you must take b before a. Return any
           valid order to take all courses, or an empty array if a cycle makes
           it impossible. Example: 2, [[1,0]] -> [0,1]; 2, [[1,0],[0,1]] ->
           [].
 PATTERN : Topological Sort (BFS, Kahn's algorithm)
================================================================================
IDEA
  Build adj with an edge prereq -> course, and count inDeg for each course.
  Put every course with inDeg 0 in queue q, since it has no blockers.
  Pop a node, append it to topo, and decrease inDeg of each neighbor.
  A neighbor that reaches 0 is now unblocked, so it enters q.
  Nodes on a cycle never reach inDeg 0, so topo.Count < numCourses means [].
EXAMPLE
  4, [[1,0],[2,0],[3,1],[3,2]]: adj[0]=[1,2], adj[1]=[3], adj[2]=[3]
  inDeg=[0,1,1,2], q=[0]. Pop 0: inDeg[1]=0, inDeg[2]=0, q=[1,2].
  Pop 1: inDeg[3]=1. Pop 2: inDeg[3]=0, q=[3]. Pop 3. topo=[0,1,2,3].
  Cycle case 2, [[0,1],[1,0]]: inDeg=[1,1], q starts empty, topo=[] -> [].
COMPLEXITY
  Time  O(n + m)  each node is enqueued once and each edge is relaxed once
  Space O(n + m)  adj holds every edge, plus inDeg, q and topo per node
PATH TO OPTIMAL
  Brute force: try orderings and check each - O(n! * m) - hopeless.
  Repeatedly scan all courses for one with no unmet prereq - O(n * (n+m)).
  Kahn's BFS with an inDeg count (this file) - O(n + m) - no rescans needed.
KEYWORDS
  topological sort, Kahn's algorithm, indegree, DAG, cycle detection, BFS
WATCH OUT
  - Edge direction: pair[0] is the course and pair[1] is the prereq. Adding
    adj[course].Add(prereq) builds the reversed graph and gives reversed
    order.
  - Do not skip the topo.Count == numCourses check. Without it a cycle
    returns a partial order instead of an empty array.
  - A self-loop [0,0] sets inDeg[0]=1 forever. The code correctly returns [].
  - Seed q with ALL inDeg-0 nodes, not only node 0. The graph can be split
    into several parts (disconnected).
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you solve it with DFS instead?
     -> Use three colors (unvisited, visiting, done). Reaching a "visiting"
        node means a cycle. Append each node when it finishes, then reverse the
        list. It is still O(n + m), but recursion depth can reach n.
  2. How do you return the lexicographically smallest order?
     -> Replace q with a min-heap (PriorityQueue). The cost becomes O(m + n
        log n). The order is unique, but you pay the log factor.
  3. How do you find the minimum number of semesters if you can take any
     number of courses at once?
     -> Process q level by level. Each BFS level is one semester, so count the
        levels. It stays O(n + m).
  4. How do you only check if finishing is possible (Course Schedule I)?
     -> Run the same code but keep a counter instead of topo. Return counter
        == numCourses. This saves the O(n) output list.
TRIGGER
  Items have "must come before" dependencies and you need a valid order or
  must detect a cycle.
================================================================================
*/
