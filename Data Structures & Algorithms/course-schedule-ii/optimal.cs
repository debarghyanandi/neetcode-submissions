// ##########################################################################
// #  optimal.cs            O(n + m) time / O(n + m) space
// #  Kahn's algorithm topological sort BFS   [kahn-topological-sort]
// #  the only solution in this folder
// #
// #  YOU SOLVED THIS YOURSELF (from submission-0)
// #
// #  Each node is processed once and each edge is traversed once using a
// #  queue of zero-indegree nodes.
// ##########################################################################

public class Solution {
    public int[] FindOrder(int numCourses, int[][] prerequisites) {
        // My solution
        // Bfs Kanhs algo
        // Build the adjacency list
        List<int>[] courseGraph = new List<int>[numCourses];
        for (int i = 0; i < numCourses; i++)
        {
            courseGraph[i] = new List<int>();
        }
        
        int[] indegree = new int[numCourses];

        // Build the graph + indegree
        foreach (int[] pair in prerequisites)
        {
            int course = pair[0];
            int prereq = pair[1];
            courseGraph[prereq].Add(course);
            indegree[course]++;
        }
        

        Queue<int> queue = new Queue<int>();
        // Insert all the nodes whose indegree is 0;
        for (int i = 0; i < numCourses; i++)
        {
            if (indegree[i] == 0)
            {
                queue.Enqueue(i);
            }
        }

        List<int> order = new List<int>();

        while (queue.Count > 0)
        {
            int node = queue.Dequeue();
            order.Add(node);

            // Remove this node from its neightbours indegree
            foreach (int neighbor in courseGraph[node])
            {
                indegree[neighbor]--;
                if (indegree[neighbor] == 0)
                    queue.Enqueue(neighbor);
            }
        }

        return (order.Count == numCourses) ? order.ToArray() : Array.Empty<int>();
    }
}

/*
================================================================================
 PATTERN : Topological Sort - Kahn's BFS on indegree
 SOURCE  : YOUR OWN SOLUTION - marker check on submission-0.cs when it was
           first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  The problem asks for an order of courses where every prerequisite comes first,
  and says to return an empty array if no such order exists. That is exactly a
  topological order of a directed graph, plus a cycle test. Kahn's algorithm
  gives both at once: courseGraph[prereq] holds the courses unlocked by prereq,
  indegree[course] counts how many prerequisites are still unmet, and a course
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
  Every node placed in queue has indegree 0 at that moment, meaning all of its
  prerequisites are already in order. So order stays a valid partial topological
  order at every step. A node's counter reaches 0 exactly once, so it is
  enqueued exactly once and order has no duplicates. If a cycle exists, no node
  on that cycle ever drops to 0, so order.Count ends below numCourses and the
  final check returns Array.Empty.
EDGE DIRECTION IS THE EASY BUG
  The input pair is [course, prereq], so the edge must run prereq -> course, and
  it is course whose indegree goes up. The code does
  courseGraph[prereq].Add(course) and indegree[course]++. Flip these two and you
  silently get the reversed order, which still passes the count check and still
  looks like a valid answer at a glance.
COUNT CHECK IS THE CYCLE CHECK
  There is no visited set and no explicit cycle detection. The single comparison
  order.Count == numCourses does that job, because every acyclic graph drains
  completely and every cycle leaves at least one node stuck with a positive
  counter. Courses with no prerequisites at all are handled by the same rule:
  they start at indegree 0 and seed the queue.
WATCH OUT
  Duplicate pairs in prerequisites are not filtered. If [1,0] appears twice,
  indegree[1] becomes 2 and the edge is stored twice, so the counter is also
  decremented twice - it still works, but only because the two counts stay in
  sync. A self loop like [2,2] correctly makes the answer empty, since
  indegree[2] can never reach 0. Also note the code returns Array.Empty rather
  than null, so the caller never has to null check.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Return any valid order is fine here - what if the interviewer wants the
  lexicographically smallest order?
     Swap Queue for a PriorityQueue<int,int> (or SortedSet) so the smallest
     ready course is always taken next; that adds a log n factor to each push
     and pop.
  2. How would you also report which courses are stuck in the cycle?
     After the loop, any i with indegree[i] > 0 is on or downstream of a cycle;
     collect those in one extra pass over indegree at no extra asymptotic cost.
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
