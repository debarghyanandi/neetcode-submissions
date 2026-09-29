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
 PATTERN : Hash Map + Doubly Linked List - O(1) move-to-recent
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  cap      the most entries the cache may hold
  cache    cache[key] = the list node that holds that key's value
  left     dummy head node; left.next is the least recently used entry
  right    dummy tail node; right.prev is the most recently used entry
  lru      the node taken from left.next and evicted when the cache is over capacity
WHY THIS PATTERN
  The problem asks for Get and Put in constant time, and it asks us to drop the
  least recently used key. The dictionary cache finds a key's node in O(1). The
  doubly linked list keeps the nodes in order of use, so it can unlink a node
  and move it to the end in O(1). Neither structure can do both jobs alone. The
  dictionary has no order, and the list cannot find a key without walking
  through it.
BRUTE FORCE
  Keep a list of (key, value) pairs in order of use. On each Get or Put, search
  the list for the key, remove it, and append it at the end. On overflow, remove
  the first element. This is correct but costs O(n) per operation, because both
  the search and the removal from the middle of the list scan or shift elements.
  The interviewer wants O(1).
INVARIANT
  The nodes between left and right are exactly the values in cache, ordered from
  least recent (next to left) to most recent (next to right). Every Get and Put
  that touches a key calls Remove and then Insert, which moves that node next to
  right. So left.next is always the entry that has gone longest without use.
  When cache.Count goes over cap, left.next is the right node to evict.
NODE STORES ITS OWN KEY
  When we evict lru, we must also delete it from the dictionary. The list only
  gives us the node, so the node has to carry its key. That is why Node has a
  key field and why the code calls cache.Remove(lru.key). If the node held only
  the value, eviction could not find its dictionary entry without a scan.
SENTINEL NODES
  left and right are dummy nodes that are never stored in cache. Because of
  them, Remove and Insert never meet a null prev or next, so there are no
  special cases for an empty list or for the first or last node. Their key 0
  never clashes with a real key 0, because they are never added to the
  dictionary.
WATCH OUT
  When Put gets a key that already exists, it unlinks the old node and makes a
  new one. It does not update node.val in place. This is correct, but it creates
  a new object on every update. The dictionary entry must be overwritten
  (cache[key] = newNode) at the same time as the unlink. If you split those two
  steps, the dictionary will point at a node that is no longer in the list.
  Eviction runs after the insert, so the check must be cache.Count > cap and not
  >=. The code has no locking, so two threads calling Get at the same time can
  break the prev and next links.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How would you make this thread-safe?
     The simplest way is one lock around the body of Get and Put. Even Get
     changes the list, so a reader-writer lock does not help much. Striped locks
     (one lock per shard of the keys) give more throughput, but the recency
     order is then only kept inside each shard.
  2. How would you change it to LFU (evict the least frequently used key)?
     Keep a count in each node and one doubly linked list for each frequency,
     plus a minFreq variable. A Get moves the node to the next frequency's list.
     Eviction takes the oldest node from the minFreq list. All steps are still
     O(1), but there is more to keep in sync.
  3. Can you use library types instead of your own Node?
     Yes. In C#, use LinkedList<(int key, int val)> with a Dictionary<int,
     LinkedListNode<...>>. Its Remove(node) and AddLast(node) are O(1). You
     write less code, but you lose the sentinel trick and have to handle the
     empty-list case yourself.
  4. What if entries must also expire after a time limit (TTL)?
     Store an expiry time in each node. On Get, treat an expired node as missing
     and remove it. To remove expired entries early, add a min-heap ordered by
     expiry time. That makes those operations O(log n).
TRIGGER
  Reach for this when a problem needs O(1) lookup by key and also an O(1) way to
  reorder items or remove the oldest one.
C# NOTE
  Get and Put each call cache.ContainsKey(key) and then cache[key], which does
  two hash lookups. cache.TryGetValue(key, out Node node) does one lookup and
  gives you the node directly.
COMPLEXITY
  Time  : O(1)
  Space : O(k)
================================================================================
*/
