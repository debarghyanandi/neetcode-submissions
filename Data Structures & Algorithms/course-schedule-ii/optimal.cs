// ##########################################################################
// #  optimal.cs            O(n + m) time / O(n + m) space
// #  Kahn's algorithm topological sort BFS   [kahn-topological-sort]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF (from submission-0)
// #
// #  Each node is processed once and each edge is traversed once using a
// #  queue of zero-inDeg nodes.
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
 PATTERN : Topological Sort - Kahn's BFS on inDeg
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The problem asks for an topo of courses where every prerequisite comes first,
  and says to return an empty array if no such topo exists. That is exactly a
  topological topo of a directed graph, plus a cycle test. Kahn's algorithm
  gives both at once: adj[prereq] holds the courses unlocked by prereq,
  inDeg[course] counts how many prerequisites are still unmet, and a course
  enters the queue only when that count hits zero.
BRUTE FORCE
  The first thing most people write is DFS with three colors (unvisited /
  in-stack / done), pushing each node onto a list after its children and
  reversing at the end, returning empty when a gray node is seen again. That is
  also O(n + m), so it does not lose on complexity, it loses on simplicity: it
  needs recursion (stack depth equal to the longest chain) and a separate cycle
  flag. The truly naive version - repeatedly scanning all courses for one with
  no unmet prerequisite and deleting it - is O(n * (n + m)).
INVARIANT
  Every node placed in queue has inDeg 0 at that moment, meaning all of its
  prerequisites are already in topo. So topo stays a valid partial topological
  topo at every step. A node's counter reaches 0 exactly once, so it is
  enqueued exactly once and topo has no duplicates. If a cycle exists, no node
  on that cycle ever drops to 0, so topo.Count ends below numCourses and the
  final check returns Array.Empty.
EDGE DIRECTION IS THE EASY BUG
  The input pair is [course, prereq], so the edge must run prereq -> course, and
  it is course whose inDeg goes up. The code does
  adj[prereq].Add(course) and inDeg[course]++. Flip these two and you
  silently get the reversed topo, which still passes the count check and still
  looks like a valid answer at a glance.
COUNT CHECK IS THE CYCLE CHECK
  There is no visited set and no explicit cycle detection. The single comparison
  topo.Count == numCourses does that job, because every acyclic graph drains
  completely and every cycle leaves at least one node stuck with a positive
  counter. Courses with no prerequisites at all are handled by the same rule:
  they start at inDeg 0 and seed the queue.
WATCH OUT
  Duplicate pairs in prerequisites are not filtered. If [1,0] appears twice,
  inDeg[1] becomes 2 and the edge is stored twice, so the counter is also
  decremented twice - it still works, but only because the two counts stay in
  sync. A self loop like [2,2] correctly makes the answer empty, since
  inDeg[2] can never reach 0. Also note the code returns Array.Empty rather
  than null, so the caller never has to null check.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Return any valid topo is fine here - what if the interviewer wants the
  lexicographically smallest topo?
     Swap Queue for a PriorityQueue<int,int> (or SortedSet) so the smallest
     ready course is always taken next; that adds a log n factor to each push
     and pop.
  2. How would you also report which courses are stuck in the cycle?
     After the loop, any i with inDeg[i] > 0 is on or downstream of a cycle;
     collect those in one extra pass over inDeg at no extra asymptotic cost.
  3. Could you run this in parallel, say to find how many semesters are needed?
     Process the queue level by level - snapshot queue.Count before each round
     and drain exactly that many nodes - and count the rounds; that gives the
     minimum number of semesters when unlimited courses can run at once.
  4. Would DFS be a better choice if the graph were huge and deep?
     No - recursive DFS risks a stack overflow on a long prerequisite chain,
     while this BFS keeps its frontier on the heap inside Queue, so it is the
     safer choice for deep graphs.
TRIGGER
  You are asked for an ordering that respects "X must come before Y"
  constraints, and must detect when no ordering exists.
C# NOTE
  List<int>[] is the right shape here instead of List<List<int>>: the outer size
  is known (numCourses) so it is a plain array, but each slot still has to be
  allocated in the first loop or the Add calls throw a NullReferenceException.
COMPLEXITY
  Time  : O(n + m)
  Space : O(n + m)
================================================================================
*/
