// --------------------------------------------------------------------------
// -  optimal.cs            O(1) time / O(n) space
// -  doubly linked list + hash map   [dll-hashmap]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself (from submission-0)
// -
// -  hash map gives O(1) node lookup while the doubly linked list maintains
// -  recency order for O(1) eviction/move-to-front, using O(n) auxiliary
// -  space for capacity n entries
// --------------------------------------------------------------------------

public class Node
{
    public int key { get; set; }
    public int val { get; set; }
    public Node prev { get; set; }
    public Node next { get; set; }

    public Node(int key, int val)
    {
        this.key = key;
        this.val = val;
        prev = null;
        next = null;
    }
}

public class LRUCache
{
    private int cap;
    private Dictionary<int, Node> cache;
    private Node left;
    private Node right;

    public LRUCache(int capacity)
    {
        cap = capacity;
        cache = new Dictionary<int, Node>();
        left = new Node(0, 0);
        right = new Node(0, 0);
        left.next = right;
        right.prev = left;
    }

    private void Remove(Node node)
    {
        Node prev = node.prev;
        Node afterNode = node.next;
        prev.next = afterNode;
        afterNode.prev = prev;
    }

    private void Insert(Node node)
    {
        Node prev = right.prev;
        prev.next = node;
        node.prev = prev;
        node.next = right;
        right.prev = node;
    }

    public int Get(int key)
    {
        if (cache.ContainsKey(key))
        {
            Node node = cache[key];
            Remove(node);
            Insert(node);
            return node.val;
        }
        return -1;
    }

    public void Put(int key, int value)
    {
        if (cache.ContainsKey(key))
        {
            Remove(cache[key]);
        }
        Node newNode = new Node(key, value);
        cache[key] = newNode;
        Insert(newNode);

        if (cache.Count > cap)
        {
            Node lruNode = left.next;
            Remove(lruNode);
            cache.Remove(lruNode.key);
        }
    }
}

/*
================================================================================
 PATTERN : Hash map + doubly linked list, sentinels at both ends
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
INVARIANT
  The chain between the two sentinels holds exactly the nodes stored as values
  in cache, ordered stale-to-fresh: left.next is the least recently used entry,
  right.prev is the most recently used. Nothing records a timestamp or a counter
  - recency is the list position itself, kept true because every operation that
  touches a key ends by splicing that key's node in just before right. On exit
  from Put, cache.Count <= cap always holds.
WALKTHROUGH
  1. Get(key): miss returns -1 without disturbing anything. On a hit,
  Remove(node) unlinks it from wherever it sat and Insert(node) re-links it at
  the right end, so a read counts as a use.
  2. Put(key, value): if the key is present, Remove(cache[key]) unlinks the
  stale node from the list. Then a fresh Node is built, cache[key] overwrites
  (or adds) the dictionary entry, and Insert puts it at the right end.
  3. Eviction: if cache.Count > cap, take lruNode = left.next, unlink it, and
  cache.Remove(lruNode.key). The order matters - read lruNode.key before you
  lose the reference.
WHY SENTINELS EARN THEIR KEEP
  left and right are Node(0,0) that never enter cache, so their key 0 can never
  collide with a real key 0 and they can never be evicted. That is what lets
  Remove read node.prev and node.next unconditionally: any real node has
  something on both sides. Drop the sentinels and both Remove and Insert need
  null branches for the head and tail cases, which is where hand-written
  linked-list code usually breaks.
WHY THE NODE CARRIES ITS KEY
  Get and Put travel dictionary -> node, but eviction travels the other way: it
  picks left.next off the list and then has to delete the matching dictionary
  entry. lruNode.key is the only bridge back. If Node stored only val, the
  evicted key would stay in cache forever pointing at an unlinked node,
  cache.Count would never fall back to cap, and the cache would evict on every
  subsequent Put.
WATCH OUT
  Put on an existing key allocates a whole new Node rather than assigning
  node.val = value. That is correct - the old node is unlinked first, then its
  last reference is overwritten by cache[key] = newNode - but the discarded
  node's prev and next still point into the live list. Harmless only because
  nothing reads it again; do not keep a stray reference to it.

  The eviction check is a single if, not a while, and it runs after the insert.
  That is sound because each Put adds at most one entry: the existing-key branch
  replaces rather than grows. Change Put to insert more than one entry and that
  single if stops being enough.

  ContainsKey followed by cache[key] probes the dictionary twice on the Get hit
  path, and Put probes three times (ContainsKey, the indexer read inside
  Remove(cache[key]), the indexer write). TryGetValue collapses each of those to
  one probe.
INTERVIEW FOLLOW-UPS
  Why doubly linked? Unlinking a node you already hold a reference to requires
  its predecessor. A singly linked list forces an O(n) scan to find it, which
  throws away the exact guarantee the dictionary was there to buy.

  Why not just a list or an array? Either the lookup or the reorder becomes
  linear; you need both directions cheap, which is why two structures point at
  the same Node objects.

  Thread safety: none here. Two concurrent Get calls both mutate prev/next
  pointers and can tear the chain - a real cache needs a lock or per-shard
  striping.

  LFU instead of LRU: position no longer encodes the answer, since you need
  counts. The O(1) version keeps a list of frequency buckets, each bucket itself
  an LRU list.

  C# has LinkedList<T> and LinkedListNode<T>, so Dictionary<int,
  LinkedListNode<(int,int)>> with AddLast/Remove/First gives the same structure
  with no pointer code. The manual version is what gets asked for, because the
  splice logic is the thing being tested.
TRIGGER
  Reach for this shape when a problem needs keyed lookup AND an ordering that
  changes on every access. The giveaways: a stated fixed capacity, "O(1) get and
  put", or any eviction rule phrased in terms of how recently something was
  touched.
COMPLEXITY
  Time  : O(1)
  Space : O(n)
================================================================================
*/
