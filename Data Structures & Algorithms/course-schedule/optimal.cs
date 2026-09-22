// --------------------------------------------------------------------------
// -  optimal.cs            O(n + m) time / O(n + m) space
// -  Kahn's algorithm, BFS topological sort   [kahn-bfs-topological]
// -  ties with optimal-variant.cs on O(n + m) time / O(n + m) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  Each node and edge processed exactly once: O(numCourses) nodes +
// -  O(prerequisites) edges via queue.
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
 PATTERN : Topological Sort (Kahn's BFS) - cycle detection by count
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  adj       adj[prereq] = list of courses that unlock after prereq
  inDeg     inDeg[c] = number of prerequisites of c still unfinished
  q         courses whose prerequisites are all done, ready to take
  topoCnt   how many courses have been removed from the graph so far
WHY THIS PATTERN
  The problem says pair [course, prereq] means prereq must come before course,
  so the input is a directed graph and the question "can I finish all courses"
  is really "is this graph free of cycles". Kahn's algorithm peels off nodes
  that have no remaining prerequisite, which is exactly what "a course you can
  take right now" means. Any course stuck in a cycle never reaches inDeg 0, so
  it is never enqueued. Comparing topoCnt to numCourses turns cycle detection
  into one integer compare.
BRUTE FORCE
  The first thing most people write is DFS from every course, carrying the
  current path in a visited set, and returning false if the walk re-enters a
  node already on the path. Without a second "fully explored" marker that is O(V
  * (n + m)) because the same subtree is re-walked from every start. It is not
  wrong, just repeated work; Kahn's touches each edge exactly once instead.
INVARIANT
  At every point in the while loop, inDeg[x] is the number of prerequisites of x
  that have not yet been dequeued, and q holds exactly the not-yet-processed
  courses whose count has hit zero. So a course is enqueued only after all its
  prerequisites left the queue, which means the dequeue order is a valid course
  order. When the queue empties, topoCnt counts every course that could ever
  reach zero; if some course remains, it sits in or behind a cycle, and topoCnt
  < numCourses.
EDGE DIRECTION
  adj[prereq].Add(course) points from the requirement to the thing it unlocks,
  and inDeg counts on the course side. Flipping this - building
  adj[course].Add(prereq) while still incrementing inDeg[course] - compiles,
  runs, and silently answers the reversed problem. The pair is unpacked into
  named locals course and prereq first, which is the cheap defense against
  getting pair[0] and pair[1] backwards.
WATCH OUT
  The comment says "Bfs Kanhs algo" but the traversal order does not matter
  here; a Stack would give the same true/false answer, so do not claim the BFS
  layering is doing work it is not. Duplicate pairs in prerequisites are counted
  twice in inDeg and appear twice in adj, which still balances out and stays
  correct, but a self-loop [a, a] makes inDeg[a] = 1 with no other path to
  decrement it, correctly returning false. The code never validates that course
  and prereq are inside [0, numCourses), so a bad pair throws
  IndexOutOfRangeException rather than returning false. Also note the method
  reports only yes or no; the valid order is computed and then thrown away.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Return the actual course order instead of a bool.
     Append node to a List<int> (or an int[] of size numCourses indexed by
     topoCnt) inside the while loop, then return it when topoCnt == numCourses
     and an empty array otherwise. Same time cost, one more array of size n.
  2. Print which courses are stuck in the cycle.
     After the loop, scan inDeg and report every index still greater than 0 -
     those are exactly the courses never released. It costs one extra O(n) pass
     and no extra memory.
  3. numCourses is huge and most courses have no prerequisites.
     List<int>[] allocates n empty List objects up front even for isolated
     nodes. Switch to a CSR layout: one int[] of edge targets plus an int[] of
     start offsets built from a counting pass, so the memory is two flat arrays
     sized n and m.
  4. Prerequisites arrive one at a time and you must answer after each.
     Kahn's from scratch per query is O(q * (n + m)). For incremental edge
     additions, keep the current topological order and only repair the affected
     window, or detect a cycle by checking reachability from the new edge's head
     back to its tail.
TRIGGER
  "A must come before B" pairs plus a question about whether everything can be
  scheduled, or in what order.
C# NOTE
  Queue<int> of a value type avoids boxing and Dequeue is O(1), which is the
  right pick over List<int> with RemoveAt(0); the int[] inDeg also starts
  zero-filled by the CLR, so no explicit init loop is needed.
COMPLEXITY
  Time  : O(n + m)
  Space : O(n + m)
================================================================================
*/
