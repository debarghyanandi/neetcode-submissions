public class Node{
    public int key {get; set;}
    public int val {get; set;}
    public Node prev {get; set;}
    public Node next {get; set;}

    public Node(int key, int val){
        this.key = key;
        this.val = val;
        prev = null;
        next = null;
    }
}

public class LRUCache {
    private int cap;
    private Dictionary<int, Node> cache;
    private Node left;
    private Node right;

    public LRUCache(int capacity) {
        cap = capacity;
        cache = new Dictionary<int, Node>();
        left = new Node(0,0);
        right = new Node(0,0);
        left.next = right;
        right.prev = left;
    }

    private void Remove(Node node){
        Node prev = node.prev;
        Node nxt = node.next;
        prev.next = nxt;
        nxt.prev = prev;
    }

    private void Insert (Node node){
        Node prev = right.prev;
        prev.next = node;
        node.prev = prev;
        node.next = right;
        right.prev = node;
    }
    
    public int Get(int key) {
        if(cache.ContainsKey(key)){
            Node node = cache[key];
            Remove(node);
            Insert(node);
            return node.val;
        }
        return -1;
    }
    
    public void Put(int key, int value) {
        if(cache.ContainsKey(key)){
            Remove(cache[key]);
        }
        Node newNode = new Node(key, value);
        cache[key] = newNode;
        Insert(newNode);

        if(cache.Count > cap){
            Node lru = left.next;
            Remove(lru);
            cache.Remove(lru.key);
        }
    }
}
