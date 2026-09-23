// --------------------------------------------------------------------------
// -  optimal.cs            O(1) time / O(k) space
// -  Doubly-linked list with hash map   [lru-doubly-linked-list]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself
// -
// -  Hash map provides O(1) access to nodes; doubly-linked list maintains
// -  recency order and enables O(1) eviction of least-recently-used item
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
        Node nxt = node.next;
        prev.next = nxt;
        nxt.prev = prev;
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
            Node lru = left.next;
            Remove(lru);
            cache.Remove(lru.key);
        }
    }
}

/*
================================================================================
 PATTERN : Hash map + doubly linked list - O(1) LRU cache
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  cap      the capacity limit the cache must not exceed
  cache    cache[key] = the Node object holding that key's value
  left     dummy head sentinel; left.next is the least recently used node
  right    dummy tail sentinel; right.prev is the most recently used node
  lru      the victim node evicted when Count goes over cap
WHY THIS PATTERN
  The problem asks for get and put in constant time, plus eviction of the least
  recently used entry. A dictionary alone gives O(1) lookup but no ordering; a
  list alone gives ordering but O(n) search. Combining them, cache finds the
  Node in O(1) and the node's own prev/next pointers let Remove and Insert
  unlink and relink it in O(1), so the usage order is maintained without
  scanning.
BRUTE FORCE
  The simple first version is a Dictionary from key to value plus a separate
  timestamp or a List<int> of keys in usage order. Every get moves a key to the
  back of the list, which is an O(n) search and shift, and eviction scans for
  the oldest timestamp, also O(n). It is correct but each operation is linear in
  the number of cached keys, which fails the constant-time requirement.
INVARIANT
  After every public call, the nodes between left and right are exactly the
  values of cache, ordered from least recently used at left.next to most
  recently used at right.prev, and their count is at most cap. Get and Put both
  re-link the touched node right before right, so "most recently touched" is
  always the tail. Because that ordering holds, left.next is always the correct
  eviction victim.
SENTINEL NODES
  left and right are dummy nodes that hold no real key or value. They exist so
  Remove never has to test for a null prev or next, and Insert never has to
  special-case an empty list. The cost is two wasted Node objects; the benefit
  is that both helpers are four straight pointer assignments with no branches.
WATCH OUT
  Put always allocates a new Node even when the key already exists - the old
  node is unlinked and thrown away instead of having its val updated. That
  works, but it creates garbage on every overwrite and would break if anything
  outside held a reference to the old node. Also note the eviction check is
  Count > cap, so if capacity is constructed as 0 the cache briefly holds one
  node and then immediately evicts it, which happens to be correct but only by
  accident. Remove assumes node.prev and node.next are non-null; calling it on a
  node not currently in the list would throw a NullReferenceException.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How would you make this thread safe?
     Wrap both Get and Put in a single lock over one object, since each one
     mutates both cache and the list and the two must stay in sync. A
     ReaderWriterLock does not help because Get also writes - it moves the node.
  2. How would you support LFU (least frequently used) instead?
     Add a count field to Node and keep one doubly linked list per frequency,
     plus a minFreq variable. On access you move the node from its frequency
     list to the next one up; eviction takes the head of the minFreq list. Still
     O(1), but noticeably more bookkeeping.
  3. What if entries also expire after a time-to-live?
     Store an expiry timestamp on Node and treat an expired node as a miss in
     Get, unlinking it there. For eager cleanup you would need a second
     structure ordered by expiry time, such as a priority queue, which adds
     O(log n) per insert.
  4. Can you drop the custom Node class?
     Yes - use LinkedList<int> of keys with a Dictionary<int,
     LinkedListNode<int>> plus a Dictionary<int, int> for values, since
     LinkedList exposes O(1) Remove(node) and AddLast. Less code to own, but two
     dictionaries instead of one.
TRIGGER
  A problem that demands O(1) lookup AND an ordering by recency or position that
  changes on every access.
C# NOTE
  Get and Put both call ContainsKey and then index cache, hashing the key twice;
  TryGetValue(key, out Node node) does it in one lookup. Also cache[key] =
  newNode is the right idiom here because it overwrites silently, where
  cache.Add would throw on a duplicate key.
COMPLEXITY
  Time  : O(1)
  Space : O(k)
================================================================================
*/
