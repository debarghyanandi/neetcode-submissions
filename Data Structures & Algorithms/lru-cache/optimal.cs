// --------------------------------------------------------------------------
// -  optimal.cs            O(1) time / O(k) space
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
 PROBLEM : Design a cache with a fixed capacity. Get(key) returns the value or
           -1. Put(key, value) inserts or updates. When the cache is over
           capacity, evict the least recently used key. Both Get and Put count
           as a "use". cap=2: put(1,1), put(2,2), get(1), put(3,3), get(2) ->
           1, -1
 PATTERN : Hash Map + Doubly Linked List
================================================================================
IDEA
  cache maps each key to its Node in a doubly linked list. The dummy node
  left sits beside the LRU end, and the dummy node right beside the MRU end.
  Every use calls Remove(node), then Insert(node) just before right. When
  cache.Count > cap, left.next is the least recently used node, so we drop
  it. This is correct because list order always equals recency order.
EXAMPLE
  cap=2: put(1,1) put(2,2) -> list [1,2]; get(1)=1 -> [2,1]
  put(3,3) -> [2,1,3], count 3 > 2, evict left.next=2 -> [1,3]
  get(2)=-1; put(1,10) -> [3,1]; get(1)=10
COMPLEXITY
  Time  O(1)  dictionary lookup plus a fixed number of pointer changes
  Space O(k)  one dictionary entry and one Node per stored key, at most cap+1
PATH TO OPTIMAL
  Array or list with timestamps, scan for oldest - O(n) per op - simple.
  Hash map + DLL (this file) - O(1) per op - no scan, O(1) unlink/move.
KEYWORDS
  LRU cache, design, hash map, doubly linked list, sentinel nodes, eviction
WATCH OUT
  - Node must store key: on eviction, cache.Remove(lru.key) needs it.
  - Update an existing key before you check eviction, or a full cache
    wrongly evicts another key when you only changed a value.
  - Without the left/right dummies, Remove and Insert need null checks.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you use built-in types?
     -> Java LinkedHashMap with access order, Python OrderedDict. Still O(1).
  2. Make it thread-safe?
     -> Lock around Get and Put, since Get changes the list too.
  3. LFU instead of LRU?
     -> Keep a map from count to a DLL plus minFreq. Still O(1), more code.
TRIGGER
  Need O(1) lookup plus O(1) "move to front / remove oldest": map + DLL.
================================================================================
*/
