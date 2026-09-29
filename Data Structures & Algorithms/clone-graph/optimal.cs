// --------------------------------------------------------------------------
// -  optimal.cs            O(n + m) time / O(n) space
// -  DFS with memoization   [dfs-memoization-clone]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  Each node and edge is visited exactly once; memoization prevents
// -  reprocessing via the dictionary.
// --------------------------------------------------------------------------

public class Solution
{
    public Node CloneGraph(Node node)
    {
        Dictionary<Node, Node> map = new Dictionary<Node, Node>();
        return Dfs(node, map);
    }

    private Node Dfs(Node node, Dictionary<Node, Node> map)
    {
        if (node == null)
            return null;

        if (map.ContainsKey(node))
            return map[node];

        Node copy = new Node(node.val);
        map[node] = copy;

        foreach (Node n in node.neighbors)
        {
            copy.neighbors.Add(Dfs(n, map));
        }
        return copy;
    }
}

/*
================================================================================
 PATTERN : Graph DFS + Hash Map - map each original node to its copy
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  map    map[original] = the cloned Node made for that original
  copy   the new Node for the current node; its neighbors list is filled in by the recursion
  n      one neighbor of the original node (not a count)
WHY THIS PATTERN
  The problem asks for a deep copy of a graph. A deep copy means new nodes with
  the same links, not shared ones. A graph can have cycles and shared neighbors,
  so a plain recursive copy would copy the same node many times or never stop. A
  traversal (DFS) visits every reachable node. The Dictionary map remembers
  which originals already have a copy, so each node is cloned exactly once and
  every edge points to that one copy.
BRUTE FORCE
  First collect every reachable node into a list and make a copy for each one.
  Then, for every edge, search the list of copies one by one to find the
  matching copy. This is correct, but each neighbor lookup is a linear scan, so
  the total is about O(n * (n + m)). The Dictionary replaces that scan with an
  O(1) average lookup.
INVARIANT
  Every original node that Dfs has entered has exactly one entry in map, and
  that entry is its only copy. So when Dfs(n, map) is called, it either returns
  the existing copy or makes the first and only one. Every copy.neighbors.Add
  therefore links to the single copy of that neighbor. When the top call
  returns, every reachable node has been copied once and every edge has been
  copied once, from each side.
STORE THE COPY BEFORE RECURSING
  map[node] = copy runs before the foreach over neighbors. If a cycle leads back
  to this node while its neighbors are still being built, the ContainsKey check
  finds the half-built copy and returns it. That is fine, because the copy's
  neighbors list gets filled in as the recursion unwinds. If you moved the map
  insert after the loop, a cycle (even a single edge A-B, which is stored in
  both directions) would recurse forever.
WATCH OUT
  The recursion goes as deep as the longest DFS path. A long chain-shaped graph
  can cause a StackOverflowException, and .NET cannot catch that exception, so
  the process just ends. The code assumes the Node constructor creates an empty
  neighbors list; if it left neighbors null, copy.neighbors.Add would throw. The
  code also assumes Node does not override Equals/GetHashCode. If it compared
  nodes by val, two different nodes with the same val would share one copy.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you remove the recursion?
     Use BFS with a Queue<Node>. Create the copy and add it to map when you
     enqueue a node, then connect the copied neighbors when you dequeue it. It
     takes the same time and has no stack depth risk, but the code is a little
     longer.
  2. Node values are unique and run from 1 to n. Can you drop the Dictionary?
     Yes. Use a Node[] indexed by val as the visited-and-copy table. The space
     is still O(n), but you avoid hashing. It only works because val is a
     unique, small integer.
  3. What if the graph is disconnected and you are given a list of all nodes?
     Loop over the list and call Dfs on each node, sharing the same map. Nodes
     already copied return right away, so the total work stays linear.
  4. How is this like "Copy List with Random Pointer"?
     It is the same idea: an original-to-copy map that breaks cycles and shared
     references. You can also solve that problem in O(1) extra space by putting
     each copy right after its original in the list, but that trick does not
     work for general graphs.
TRIGGER
  When you must deep-copy a structure whose pointers can form cycles or shared
  references, keep an original-to-copy map and fill it while you traverse.
C# NOTE
  map.ContainsKey(node) followed by map[node] does two hash lookups. One call to
  map.TryGetValue(node, out var existing) does the same job with a single
  lookup.
COMPLEXITY
  Time  : O(n + m)
  Space : O(n)
================================================================================
*/
