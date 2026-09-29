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
 PATTERN : Topological Sort (Kahn's BFS) - detect a cycle
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  adj       adj[p] = list of courses that need course p first
  inDeg     inDeg[c] = number of prerequisites of course c not yet taken
  q         courses whose prerequisites are all taken, ready to take now
  topoCnt   number of courses taken so far (removed from the graph)
WHY THIS PATTERN
  The problem says "course A needs course B first" and asks whether all courses
  can be finished. That is a directed graph, and the question is really: "does
  this graph have a cycle?" Kahn's algorithm takes courses from q only when
  inDeg reaches 0. A course inside a cycle never reaches inDeg 0, so it is never
  counted in topoCnt. This means topoCnt == numCourses is true exactly when
  there is no cycle.
BRUTE FORCE
  Repeat this: scan all courses, find one that is not taken and has no untaken
  prerequisites, then mark it taken. If a full scan finds no such course before
  every course is taken, return false. Each scan costs O(n + m), and you may do
  n scans, so the total is O(n * (n + m)). It loses because it searches for the
  next ready course again and again. Kahn's algorithm keeps the ready courses in
  q, so it never has to search.
INVARIANT
  Every course in q, or already counted in topoCnt, has all of its prerequisites
  already counted. inDeg[c] always equals the number of prerequisites of c that
  are not yet counted. So a course is added to q exactly once, at the moment its
  last prerequisite is counted. If a cycle exists, every course in the cycle
  keeps inDeg of at least 1, because one of its prerequisites is never counted.
  So topoCnt stays below numCourses.
EDGE DIRECTION
  The code adds the edge prereq -> course (adj[prereq].Add(course)) and
  increments inDeg[course]. Nodes with no prerequisites start in q. For this
  yes/no question, reversing every edge would give the same answer, because a
  reversed cycle is still a cycle. It would not give the same answer if you
  needed the actual order.
WATCH OUT
  Nothing checks the values in pair. A course number outside 0..numCourses-1
  throws IndexOutOfRangeException. A self-loop like [3,3] is handled correctly:
  inDeg[3] never reaches 0. Duplicate pairs are also safe, because each copy
  adds one to inDeg and also gets its own entry in adj, so the counts still
  match. The comment "Kanhs" is a typo for Kahn's, but the code does what the
  comment says.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Return a valid order of the courses (Course Schedule II).
     Add each dequeued node to a result list instead of only incrementing
     topoCnt. If the list has fewer than numCourses items, return an empty
     array. The extra cost is O(n) space for the list.
  2. Can you solve it with DFS instead?
     Yes. Use three colors per node: unvisited, on the current path, done. If
     you reach a node that is on the current path, you found a cycle. This needs
     no inDeg array, but deep chains can overflow the call stack unless you
     write the DFS with your own stack.
  3. Does it matter that q is a queue?
     No. Any container works here, for example a Stack<int>, because we only
     count the nodes. A min-heap (PriorityQueue) gives the smallest valid order
     in dictionary order, at O(log n) per operation.
  4. Find the minimum number of semesters if you can take any number of courses
  at once.
     Process q one level at a time: take all nodes in q as one semester and
     count the levels. The answer is the length of the longest chain. If there
     is a cycle, the answer is -1.
TRIGGER
  Items with "X must come before Y" dependencies, and the question is whether an
  order exists or what the order is.
C# NOTE
  Course numbers are dense (0..numCourses-1), so an array of List<int> indexed
  by course is a good fit. It avoids the hashing and key checks a
  Dictionary<int, List<int>> would need.
COMPLEXITY
  Time  : O(n + m)
  Space : O(n + m)
================================================================================
*/
