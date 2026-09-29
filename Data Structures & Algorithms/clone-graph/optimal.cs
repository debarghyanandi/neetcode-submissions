// --------------------------------------------------------------------------
// -  optimal.cs            O(n + m) time / O(n) space
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
 PROBLEM : You get a reference to one node of a connected, undirected graph.
           Each Node has an int val and a list of neighbors. Return a deep
           copy: new Node objects with the same vals and the same edges,
           sharing nothing with the input. Example: adjList
           [[2,4],[1,3],[2,4],[1,3]] -> same adjList, new nodes.
 PATTERN : DFS + hash map (original -> clone)
================================================================================
IDEA
  Dfs(node, map) returns the clone of node. If node is already a key in map,
  it returns that clone and stops. Otherwise it creates copy, stores
  map[node] = copy first, and only then recurses into each neighbor. Storing
  before recursing means a cycle comes back to an existing clone, so each
  node is cloned exactly once and every edge points to the right clone.
EXAMPLE
  4-cycle 1-2-3-4-1, adj 1:[2,4] 2:[1,3] 3:[2,4] 4:[1,3]. Dfs(1): new c1.
  -> Dfs(2): new c2; its neighbor 1 is in map -> c1; Dfs(3): new c3;
     neighbor 2 in map -> c2; Dfs(4): new c4, neighbors 1,3 hit map.
  Result: 4 clones, 8 neighbor links, same adjacency list as the input.
COMPLEXITY
  Time  O(n + m)  each node is cloned once and each edge is followed from both
                  ends
  Space O(n)      map holds n entries; the recursion stack can reach depth n
PATH TO OPTIMAL
  Recursion with no visited map - infinite on any cycle - does not work.
  DFS + map (this file) - O(n + m) - each node is created once. No sibling.
KEYWORDS
  clone graph, deep copy, DFS, BFS, hash map, visited set, cycle, graph
WATCH OUT
  - Put map[node] = copy BEFORE the foreach. If you add it after, a cycle
    recurses forever (stack overflow).
  - A long path graph makes recursion n deep and can overflow the stack.
  - Key the map by Node reference, not by val, unless vals are unique.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it without recursion?
     -> Use BFS with a queue. Clone the start node, then for each dequeued
        node clone unseen neighbors, enqueue them, and add links. Same O(n + m).
  2. What if the graph is not connected?
     -> You only get one node, so you only reach its component. With a list of
        all nodes, run Dfs from each one that is not yet in map.
  3. Copy a linked list with random pointers?
     -> Same idea: map old node -> new node, then set next and random from
        map. O(n) space. Interleaving copies in the list gives O(1) extra space.
TRIGGER
  When you must deep-copy a structure with shared nodes or cycles, map each
  original to its clone and check the map before creating a new one.
================================================================================
*/
