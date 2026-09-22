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
 PATTERN : Topological Sort - Kahn's BFS on in-degrees
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  adj      adj[prereq] = list of courses that open up after prereq is done
  inDeg    inDeg[c] = number of prerequisites of c still unfinished
  q        courses whose prerequisites are all done and not yet output
  topo     the order built so far; final answer if it reaches numCourses
WHY THIS PATTERN
  The problem gives pairs "to take course a you must first take b", which is
  exactly a directed edge b -> a, and asks for any order that respects every
  edge. That is the definition of a topological order, and it exists only if the
  graph has no cycle. Kahn's algorithm answers both questions at once: it
  repeatedly takes a course with inDeg 0, appends it to topo, and lowers inDeg
  for every neighbour in adj[node]. If a cycle exists, those courses never reach
  inDeg 0, so topo stays short and the final size check catches it.
BRUTE FORCE
  The first thing most people write is: scan all numCourses each round, pick any
  course whose prerequisites are all already in topo, append it, repeat until
  nothing can be picked. That is O(n * (n + m)) because each of up to n rounds
  rescans the whole graph. The queue here replaces that rescan - a course is
  re-examined only when one of its prerequisites is actually removed.
INVARIANT
  At every point in the loop, inDeg[c] counts exactly the prerequisites of c
  that are not yet in topo, and q holds precisely the courses with inDeg 0 that
  have not been output. So when node is dequeued and appended, all of its
  prerequisites are already earlier in topo - that is what makes the output a
  valid order. Each edge is used to decrement exactly once, so a course is
  enqueued at most once and topo has no duplicates.
EDGE DIRECTION
  pair[0] is the course and pair[1] is the prereq, so the edge must run
  adj[prereq].Add(course) and the count must land on inDeg[course]. Swapping
  these two lines still compiles and still returns an array of the right length
  on many inputs - it just produces the reversed order. If you ever get "wrong
  order but valid-looking output", check this pair first.
WATCH OUT
  The failure return is Array.Empty<int>(), a zero-length array, not null - make
  sure the judge expects that and not null, and do not later mutate it, since
  Array.Empty caches one shared instance. numCourses = 0 is handled: no seeds,
  topo is empty, 0 == 0, so it returns an empty array as success, which is the
  usual expected answer. A self-loop pair like [2,2] gives inDeg[2] = 1 with no
  way to ever clear it, so it correctly falls into the cycle branch. The code
  assumes every value in prerequisites is in [0, numCourses), any out-of-range
  id throws IndexOutOfRangeException rather than returning empty.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do this without a queue, using DFS instead?
     Yes - DFS with three colours (unvisited, in-stack, done) and push each node
     to a list after its children, then reverse. Same complexity, but recursion
     depth equals the longest chain, so a deep graph risks stack overflow unless
     you write an explicit stack.
  2. The answer must be lexicographically smallest among all valid orders. What
  changes?
     Replace Queue<int> with a min-heap (PriorityQueue<int,int>) so the smallest
     available course is always taken next. Time becomes O(n log n + m); the
     rest of the code is untouched.
  3. How would you report which courses are in the cycle, not just fail?
     After the loop, any course with inDeg still greater than 0 is unfinishable;
     collect those indices instead of returning empty. It costs one extra O(n)
     pass and no extra memory.
  4. Prerequisites arrive as a stream and courses can be added over time?
     Keep adj and inDeg as growable structures and re-run only from the newly
     freed nodes; a full re-sort per update is O(n + m) each time, so
     incremental topological maintenance is the real answer when updates are
     frequent.
TRIGGER
  The input is a list of "B must come before A" pairs and the task is an order
  or a can-finish check.
C# NOTE
  List<int>[] adj is an array of lists, not a Dictionary<int, List<int>>, which
  is right here because course ids are already 0..numCourses-1 - direct
  indexing, no hashing - but it forces the explicit init loop since every slot
  starts as null.
COMPLEXITY
  Time  : O(n + m)
  Space : O(n + m)
================================================================================
*/
