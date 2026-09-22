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
 PATTERN : DFS with visited-map - clone graph node by node
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  map     map[original node] = its clone; also serves as the visited set
  copy    the new Node holding node.val, filled with cloned neighbors
WHY THIS PATTERN
  The problem gives a connected undirected graph with cycles and asks for a deep
  copy. A plain traversal would loop forever on a cycle, and a plain copy would
  duplicate the same node twice when two neighbors point at it. Putting copy
  into map before the neighbor loop makes map do both jobs: it marks node as
  seen, and it hands back the one clone that every other edge must reuse.
BRUTE FORCE
  The first idea is usually two passes: walk the graph once to collect all nodes
  and make a bare clone for each, then walk again to wire the neighbor lists.
  That is also O(n + m) but needs two traversals and its own visited set in
  each. This single DFS does the same work in one pass, so the two-pass version
  only loses on code size and clarity, not on order of growth.
INVARIANT
  At every entry to Dfs, map holds a clone for each node already visited, and
  any node in map has had its own neighbors either fully wired or is an ancestor
  currently being wired. Because copy is inserted into map on the line before
  the foreach, a cycle that comes back to node finds it in map and returns the
  same object instead of recursing again. So each original node is created
  exactly once and each edge is appended exactly once, which makes the clone
  structurally identical.
WATCH OUT
  The recursion depth follows the longest DFS path, so a long chain graph can
  overflow the stack. The code assumes node.neighbors is never null and that the
  Node constructor gives copy an empty, non-null neighbors list; if a Node(int)
  constructor left neighbors null, copy.neighbors.Add throws. map uses the
  default reference equality of Node, which is what we want here - if Node ever
  got a custom Equals or GetHashCode based on val, two different nodes with the
  same val would collide and the clone would be wrong. The node == null check
  returns null for an empty graph, which is correct, but it also silently
  returns null for a null neighbor entry instead of failing loudly.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How would you remove recursion?
     Use an explicit Stack or Queue of original nodes. Create and map the clone
     when you first push a node, then on pop iterate its neighbors: map the
     unseen ones, push them, and append map[n] to the clone's neighbor list.
     Same time, same map, but stack depth is now heap memory you control.
  2. The graph is disconnected - does this still work?
     No. This starts from one node and only reaches its component. You would
     need the full node list and a loop calling Dfs on each unmapped node,
     returning a list of clone roots instead of one.
  3. The nodes carry extra mutable payload, like a list of tags?
     map still handles identity, but each field must be copied by value, not by
     reference - new Node(node.val) copies the int fine, while a shared List
     would leave the clone aliased to the original.
  4. How do you verify the clone is right?
     Traverse both graphs in lockstep with a pair map; check vals match,
     neighbor counts match, and that no clone node is reference-equal to any
     original node.
TRIGGER
  A traversal where the same node can be reached by several paths and you must
  return one object per original - map the node to its result before you
  recurse.
C# NOTE
  ContainsKey followed by map[node] hashes node twice; TryGetValue(node, out var
  existing) does it in one lookup and is the usual C# idiom here.
COMPLEXITY
  Time  : O(n + m)
  Space : O(n)
================================================================================
*/
