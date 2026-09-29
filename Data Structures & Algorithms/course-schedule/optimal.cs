// --------------------------------------------------------------------------
// -  optimal.cs            O(n + m) time / O(n + m) space
// --------------------------------------------------------------------------

public class Solution
{
    //Bfs Kanhs algo
    public bool CanFinish(int numCourses, int[][] prerequisites)
    {
        //build the adjacency list
        List<int>[] adj = new List<int>[numCourses];
        for (int i = 0; i < numCourses; i++)
        {
            adj[i] = new List<int>();
        }

        int[] inDeg = new int[numCourses];

        //build the graph + indegree
        foreach (int[] pair in prerequisites)
        {
            int course = pair[0];
            int prereq = pair[1];
            adj[prereq].Add(course);
            inDeg[course]++;
        }

        Queue<int> q = new Queue<int>();
        //insert all the nodes whose indegree is 0;
        for (int i = 0; i < numCourses; i++)
        {
            if (inDeg[i] == 0)
            {
                q.Enqueue(i);
            }
        }

        int topoCnt = 0;

        while (q.Count > 0)
        {
            int node = q.Dequeue();
            topoCnt++;

            //remove this node from its neightbours indegree
            foreach (int nei in adj[node])
            {
                inDeg[nei]--;
                if (inDeg[nei] == 0)
                    q.Enqueue(nei);
            }
        }

        return topoCnt == numCourses;
    }
}

/*
================================================================================
 PROBLEM : You get numCourses (courses 0..n-1) and pairs [course, prereq].
           Each pair means prereq must be taken before course. Return true if
           you can finish all courses, meaning the graph has no cycle.
           Example: 2, [[1,0],[0,1]] -> false.
 PATTERN : Topological Sort (Kahn's algorithm, BFS on in-degree)
================================================================================
IDEA
  Build adj with an edge prereq -> course, and count inDeg for each course.
  Put every course with inDeg 0 in q, because it is ready to take now.
  Pop a node, add 1 to topoCnt, and lower inDeg of each nei in adj[node].
  A nei that reaches 0 joins q. Nodes on a cycle never reach 0.
  So topoCnt == numCourses holds exactly when no cycle exists.
EXAMPLE
  n=4, pairs [[1,0],[2,1],[3,2],[2,3]]: edges 0->1, 1->2, 2->3, 3->2
  inDeg=[0,1,2,1], q=[0]. Pop 0: cnt=1, inDeg[1]=0, push 1.
  Pop 1: cnt=2, inDeg[2]=1, not 0. q is empty. Cycle 2<->3 is stuck.
  topoCnt=2 != 4 -> false
COMPLEXITY
  Time  O(n + m)  each node is enqueued once and each edge is relaxed once
  Space O(n + m)  adj holds all m edges, plus inDeg and q of size n
PATH TO OPTIMAL
  Check a path from each node to itself - O(n*(n+m)) - baseline.
  DFS with 3 colors (unvisited / on stack / done) finds a back edge in
  O(n+m) - one pass - see optimal-variant.cs if it is that DFS.
  Kahn's BFS (this file) - O(n+m) - no recursion depth risk.
KEYWORDS
  topological sort, Kahn's algorithm, in-degree, cycle detection, DAG, BFS
WATCH OUT
  - Compare topoCnt to numCourses, not to prerequisites.Length. Courses
    with no edges still count, and they start in q.
  - A course id outside 0..numCourses-1 throws IndexOutOfRange in adj.
  - The comment says "Kanhs". It is Kahn's algorithm.
  - In a DFS version, a 2-state visited set is wrong. You need "on stack".
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Return a valid order (Course Schedule II)?
     -> Append each dequeued node to a list. Return it if its size is
        numCourses, else return empty. Still O(n+m).
  2. Minimum number of semesters if you take any ready courses together?
     -> Run the BFS level by level. The number of levels is the answer. Return
        -1 on a cycle. Same O(n+m).
  3. Why does a cycle leave topoCnt short?
     -> Every node on a cycle has an in-edge from another cycle node. That
        node is never popped, so its in-degree never drops to 0.
TRIGGER
  The problem has "X before Y" dependencies and asks if an order exists.
================================================================================
*/
