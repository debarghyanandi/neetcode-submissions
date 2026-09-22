// --------------------------------------------------------------------------
// -  optimal-variant.cs    O(n + m) time / O(n + m) space
// -  DFS cycle detection, path-visit tracking   [dfs-cycle-detection]
// -  ties with optimal.cs on O(n + m) time / O(n + m) space
// -
// -  Reference solution - not one you solved yourself
// -
// -  DFS visits each node and edge once; call stack depth bounded by
// -  longest dependency chain.
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
 PATTERN : DFS Cycle Detection on a Directed Graph (3-color)
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-3.cs when it was first processed
 STATUS  : Optimal variant - ties the best complexity by another route
================================================================================
VARIABLES
  adj          adj[prereq] = list of courses that unlock once prereq is done
  pair         one [course, prereq] entry from prerequisites
  visited      visited[v] = v and everything reachable from v is fully explored
  pathVisited  pathVisited[v] = v is on the DFS call stack right now
WHY THIS PATTERN
  The problem gives pairs "course needs prereq", which is exactly a directed
  edge, and asks whether every course can be finished. That is possible if and
  only if the graph has no directed cycle, because a cycle means each course in
  it waits on another one in the same cycle. The DFS walks forward along adj and
  asks one question: did I come back to a node I am still standing on?
  pathVisited answers that; visited keeps the walk from repeating work already
  proven safe.
BRUTE FORCE
  The first thing most people write is: for each course, run a fresh DFS or BFS
  and see if it can reach itself. That is one traversal per node, so O(n * (n +
  m)) time, and it re-walks the same subgraphs over and over. This file keeps
  the global visited array across all starts, so every edge is followed once.
INVARIANT
  When HasCycle(node) is running, pathVisited is true for exactly the nodes on
  the current chain of calls from the loop's start node down to node. So
  pathVisited[nei] being true means nei is an ancestor of node, and the edge
  node -> nei closes a real cycle. When HasCycle returns false, node is marked
  visited and cleared from pathVisited, meaning everything reachable from node
  was searched and contained no cycle, so any later DFS may skip it safely.
THE TWO ARRAYS DO DIFFERENT JOBS
  A single visited array is the classic wrong version here. Hitting an
  already-visited node is not a cycle by itself, since a node can be reached
  twice by two separate branches (a diamond shape) with no cycle at all. Only a
  node still on the active path is proof of a cycle, which is why pathVisited is
  set on the way in and cleared on the way out, while visited is set once and
  never cleared.
WATCH OUT
  The recursion depth is the length of the longest path in the graph. A chain
  like 0 -> 1 -> 2 -> ... built from a long prerequisites list will recurse that
  deep and can overflow the stack; an iterative version or Kahn's BFS avoids it.
  The code assumes every value in pair is in range 0..numCourses-1; a stray
  value throws IndexOutOfRangeException rather than returning false. Duplicate
  edges in prerequisites are stored twice in adj, which costs extra traversal
  work but does not change the answer. A self-loop [x, x] is handled correctly:
  pathVisited[x] is already true when the loop reads it.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How would you return the actual course order, not just true/false?
     Push node onto a stack right before pathVisited[node] = false; that gives
     reverse finish order, which is a valid topological order. Pop the stack
     into the result array. Same complexity, one extra array of size numCourses.
  2. How would you do this without recursion?
     Kahn's algorithm: compute an indegree count per course, put every
     zero-indegree course in a queue, and pop and decrement. If fewer than
     numCourses come out, a cycle exists. Same complexity, and no call stack to
     overflow.
  3. The graph is huge and mostly sparse, with numCourses very large but few
  prerequisites. Anything to change?
     The List<int>[] allocation is one List object per course whether or not it
     has edges, so the setup loop alone touches numCourses entries. A Dictionary
     keyed only by nodes that actually appear, or a flat CSR layout (one offsets
     array plus one edges array), keeps memory near the edge count.
  4. What if an edge could be dropped to make it finishable, and you had to name
  which one?
     Detect the cycle as here but carry the path, then report any edge on it.
     Deciding whether removing one edge makes the whole graph acyclic in general
     needs more care, since separate cycles may not share an edge.
TRIGGER
  Dependencies given as ordered pairs plus the question "can all of them be
  done" or "in what order" - build the directed graph and look for a cycle.
C# NOTE
  List<int>[] gives an array of separately allocated lists, so the first loop
  must fill every slot or adj[prereq].Add throws NullReferenceException - it is
  not optional setup. foreach over a List<int> uses a struct enumerator, so the
  inner loop over adj[node] does not allocate per call.
COMPLEXITY
  Time  : O(n + m)
  Space : O(n + m)
================================================================================
*/
