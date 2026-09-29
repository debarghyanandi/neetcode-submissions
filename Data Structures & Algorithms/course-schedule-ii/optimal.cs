// ##########################################################################
// #  optimal.cs            O(n + m) time / O(n + m) space
// #  Kahn's algorithm, topological sort BFS   [topological-sort-kahn-bfs]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF
// #
// #  Each node and edge processed once in BFS; adjacency list storage
// #  dominates space.
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
 PATTERN : Topological Sort (Kahn's BFS) - peel off in-degree 0 nodes
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  adj      adj[p] = list of courses that need course p first
  inDeg    inDeg[c] = number of prerequisites of course c that are not taken yet
  q        courses whose prerequisites are all taken, so they can be taken now
  topo     the course order built so far, one course per dequeue
WHY THIS PATTERN
  The problem asks for an order in which to take courses, where some courses
  must come before others. That is a directed graph. A valid order is a
  topological order: every edge goes from earlier to later. Kahn's algorithm
  builds that order directly. It starts with the courses in q that have inDeg 0.
  Each time it takes one, it lowers inDeg for the courses in adj[node].
BRUTE FORCE
  The simple first idea is to scan all courses again and again. On each pass,
  pick any course not yet taken whose prerequisites are all in the order
  already, and add it. If a full pass adds nothing, stop. Each pass can cost O(n
  + m), and you may need up to n passes, so this is O(n * (n + m)). It is slower
  because it re-checks courses that did not change. Kahn's algorithm only
  touches a course when one of its prerequisites is finished.
INVARIANT
  At every step, inDeg[c] equals the number of prerequisites of c that are not
  yet in topo. A course is put into q only when that number drops to 0. So every
  course in topo comes after all of its prerequisites. A course that sits on a
  cycle, or depends on one, never reaches inDeg 0. That is why the check
  topo.Count == numCourses tells you if a full order exists.
EDGE DIRECTION
  For each pair, pair[1] is the prerequisite and pair[0] is the course. The edge
  goes prereq -> course: adj[prereq].Add(course) and inDeg[course]++. If you
  reverse this, you get the reverse order, and inDeg counts the wrong thing.
WATCH OUT
  The code assumes every pair has exactly two entries and both are in
  0..numCourses-1. Any other input throws an IndexOutOfRangeException. A
  duplicate pair is safe: it adds the edge twice and counts it twice in inDeg,
  so the counts still match. A self-loop like [0,0] gives inDeg[0] = 1, so
  course 0 never enters q and the code correctly returns an empty array. The
  comment "Kanhs" is a typo for Kahn's, not a different algorithm.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you solve it with DFS instead of BFS?
     Yes. Use DFS with three states per node (unvisited, visiting, done). Add
     each node to the order after all its neighbours are done, then reverse the
     order. A back edge to a "visiting" node means there is a cycle. The
     trade-off: recursion depth can reach n, so a long chain may overflow the
     stack unless you use an explicit stack.
  2. What if many valid orders exist and you must return the smallest one in
  dictionary order?
     Replace q with a min-heap, for example PriorityQueue<int,int>. You always
     take the smallest available course. Time becomes O(m + n log n).
  3. How do you find the minimum number of semesters if every course with no
  remaining prerequisites can be taken in the same semester?
     Process q one level at a time. Take the current q.Count nodes as one
     semester, and count the levels. If there is a cycle, return -1 as before.
TRIGGER
  Reach for this when a problem gives "A must happen before B" pairs and asks
  for a valid order, or asks whether one exists (cycle detection in a directed
  graph).
C# NOTE
  You know the answer has at most numCourses items. So you could fill a
  preallocated int[numCourses] with an index counter instead of List<int> topo.
  That skips the extra copy that topo.ToArray() makes.
COMPLEXITY
  Time  : O(n + m)
  Space : O(n + m)
================================================================================
*/
