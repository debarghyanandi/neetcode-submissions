// --------------------------------------------------------------------------
// -  optimal.cs            O(n + m) time / O(n + m) space
// -  Kahn's algorithm, topological sort BFS   [kahn-bfs-topo-sort]
// -  ties with optimal-variant.cs on O(n + m) time / O(n + m) space
// -
// -  Reference solution - not one you solved yourself (from submission-0)
// -
// -  Each course and prerequisite edge processed exactly once; queue stores
// -  at most all courses with in-degree zero.
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
 PATTERN : Topological Sort - Kahn's BFS over indegrees
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The question "can every course be finished" is the same as "is this directed
  graph free of cycles". Kahn's algorithm answers that without extra
  bookkeeping: inDeg[course] counts how many prerequisites are still unfinished,
  and a course can be taken the moment that count hits 0. If topoCnt reaches
  numCourses, every node was reachable that way, so no cycle exists.
BRUTE FORCE
  The first thing most people write is the same removal idea done naively: scan
  all numCourses nodes for one with indegree 0, remove it, repeat. That rescan
  costs O(V) per removal, giving O(V*E), while the queue here hands you the next
  ready node in O(1). The other common first try is running a fresh DFS from
  every node to look for a path back to itself, which repeats the same work
  numCourses times.
INVARIANT
  At every point in the loop, inDeg[v] equals the number of prerequisites of v
  that have not yet been dequeued, and the queue holds exactly the nodes whose
  count just reached 0 but which have not been processed. So a node is enqueued
  at most once, and only after all of its prerequisites were counted out. When
  the queue drains, any node still holding inDeg > 0 sits on a cycle or depends
  on one, so topoCnt < numCourses and the answer is false.
COUNT, NOT A LIST
  The code never stores the order, only topoCnt. That is enough because the
  question is yes/no. The follow-up problem, Course Schedule II, is the same
  loop with an int[] order and a write index in place of topoCnt, returning an
  empty array when the count falls short.
DUPLICATE AND SELF EDGES
  A repeated pair like [1,0] twice adds the edge twice and raises inDeg[1]
  twice, and the loop decrements it twice, so the result stays correct without
  deduplicating. A self loop [0,0] sets inDeg[0] to 1 with no other way to lower
  it, so node 0 never enters the queue and the method correctly returns false.
WATCH OUT
  The edge direction here is prereq -> course: adj[prereq].Add(course) with
  inDeg[course]++. Swapping the two consistently still gives the right
  true/false, because a reversed graph has a cycle exactly when the original
  does, so that bug hides in this problem and only shows up when you are asked
  to return the actual order. There is no null or length guard on prerequisites
  or on pair, so a malformed row would throw. If numCourses is 0, topoCnt is 0
  and the method returns true, which is the sensible answer but worth saying out
  loud rather than discovering by accident.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Return a valid order of courses instead of a boolean.
     Push each dequeued node into an int[] of length numCourses and return it
     when topoCnt == numCourses, otherwise return an empty array. Same cost; you
     only pay numCourses extra ints for the output.
  2. Write it with DFS instead of BFS.
     Use a three-state array (unvisited, on the current path, done) and report a
     cycle when you re-enter a node that is on the current path. Same
     complexity, but recursion can overflow the stack on a long prerequisite
     chain unless you push an explicit Stack<int>.
  3. What if the graph is dense, close to numCourses squared edges?
     Adjacency lists still cost O(n + m), but memory grows with m; a
     bool[numCourses, numCourses] matrix has fixed size yet forces an
     O(numCourses) scan per dequeue, so the lists stay better unless you need
     O(1) edge lookups.
  4. How would you report which courses are stuck in the cycle?
     After the loop, every i with inDeg[i] > 0 is either on a cycle or
     downstream of one, so collect those. To name the cycle itself you need a
     DFS that keeps the current path stack and slices it when it revisits a node
     on that path.
TRIGGER
  When a problem asks whether a set of "must come before" rules can all be
  satisfied, or asks for a valid order, count indegrees and drain a queue.
C# NOTE
  Because course ids are exactly 0..numCourses-1, List<int>[] indexed directly
  beats Dictionary<int, List<int>> here - no hashing and no missing-key checks -
  and the separate loop that fills every adj[i] with a new List<int> is
  required, since a fresh reference array holds nulls, not empty lists.
COMPLEXITY
  Time  : O(n + m)
  Space : O(n + m)
================================================================================
*/
