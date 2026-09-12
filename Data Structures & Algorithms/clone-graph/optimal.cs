// --------------------------------------------------------------------------
// -  optimal.cs            O(n + m) time / O(n) space
// -  DFS with hash map memoization of cloned nodes   [dfs-hashmap-clone]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself (from submission-0)
// -
// -  Recursive DFS visits each node once and each edge once, using a
// -  Dictionary to map originals to clones and avoid recloning
// --------------------------------------------------------------------------

public class Solution
{
    public Node CloneGraph(Node node)
    {
        Dictionary<Node, Node> visited = new Dictionary<Node, Node>();
        return Dfs(node, visited);
    }

    private Node Dfs(Node node, Dictionary<Node, Node> visited)
    {
        if (node == null)
            return null;

        if (visited.ContainsKey(node))
            return visited[node];

        Node copy = new Node(node.val);
        visited[node] = copy;

        foreach (Node neighbor in node.neighbors)
        {
            copy.neighbors.Add(Dfs(neighbor, visited));
        }
        return copy;
    }
}

/*
================================================================================
 PATTERN : DFS + hash map memo - register the clone before recursing
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
WHY THIS PATTERN
  A graph clone is not a tree copy: the same neighbor can be reached from
  several places, and edges can cycle back. Plain recursion over node.neighbors
  would either never terminate or would produce a fresh copy of a node each time
  it is reached, giving you a tree-shaped blowup instead of the same graph. The
  fix is one memo table, visited, keyed by the ORIGINAL node and holding the
  clone. That single table solves both problems at once, which is why no
  separate seen-set appears anywhere in the file.
THE ONE INVARIANT
  visited[node] = copy executes BEFORE the foreach over node.neighbors, not
  after it.

  That ordering is the whole algorithm. When the recursion walks an edge that
  leads back to a node already on the call stack, the ContainsKey guard at the
  top of Dfs fires and hands back the partially built copy - a Node whose val is
  set but whose neighbors list is still being filled. That is fine: the caller
  only needs the reference, and by the time the outermost call returns, every
  list has been completed by its owning frame.

  Move visited[node] = copy to just before the return and the two-node cycle 1
  <-> 2 recurses forever.
WHY A MAP AND NOT A SET
  An interviewer will ask why a HashSet<Node> is not enough. A set answers "have
  I been here", but on revisit you still need the specific clone object to
  append to copy.neighbors. Dictionary<Node, Node> answers "have I been here"
  and "which copy is yours" in one lookup, and it is what preserves object
  identity: two different originals that share a neighbor produce two clone
  lists pointing at the SAME clone instance, exactly mirroring the input.
CORRECTNESS ARGUMENT
  1. Every node is copied at most once: the only call to new Node(...) is
  immediately followed by the map write, and no later call to Dfs on that same
  node can get past ContainsKey.
  2. Every reachable node is copied at least once: Dfs is invoked on the entry
  node and then on every element of every neighbors list of a copied node, so
  the recursion covers the reachable set.
  3. Every edge is reproduced: the foreach body appends one element to
  copy.neighbors for each element of node.neighbors, in order, so the adjacency
  list of the copy is a positionally faithful image of the original.
  4. No original object leaks into the result: the only Node handed back is
  either null, a map value (itself created by new Node), or the freshly made
  copy.
WHAT THE CODE ASSUMES
  copy.neighbors.Add(...) is called without ever allocating a list, so it
  depends on the Node constructor initializing neighbors to an empty list. If
  you rewrite this from scratch with your own Node class, that is the line that
  will NullReference on you.

  The Dictionary uses Node's default equality, which for a class without an
  Equals/GetHashCode override is reference equality. That is the behavior you
  want here - it is identity, not value, that distinguishes graph nodes - and it
  means the solution does not rely on the problem's promise that val is unique.
WATCH OUT
  The null check lives inside Dfs, not in CloneGraph, so it covers both an
  empty-graph input and any null appearing inside a neighbors list. CloneGraph
  itself is a one-line trampoline that exists only to allocate visited.

  ContainsKey followed by visited[node] hashes the same key twice;
  TryGetValue(node, out var existing) does it in one probe. Behaviorally
  identical, worth saying out loud if asked to tighten the code.

  Recursion depth grows with the longest simple path, so a many-node chain can
  overflow the stack. The standard rewrite is BFS: create the clone of the entry
  node, seed visited and a Queue<Node>, then for each dequeued original, create
  clones of any unseen neighbors and append their clones to the dequeued node's
  copy. Same work, same table, explicit queue instead of the call stack.
TRIGGER
  Reach for this shape whenever you must rebuild a pointer-linked structure that
  may contain cycles or sharing: copy a linked list with random pointers,
  deep-copy an object graph, memoized traversal of a state graph. The signature
  is always map-from-old-to-new plus register-before-you-recurse.
COMPLEXITY
  Time  : O(n + m)
  Space : O(n)
================================================================================
*/
