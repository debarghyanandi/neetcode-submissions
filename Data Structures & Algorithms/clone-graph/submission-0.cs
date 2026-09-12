/*
// Definition for a Node.
public class Node {
    public int val;
    public IList<Node> neighbors;

    public Node() {
        val = 0;
        neighbors = new List<Node>();
    }

    public Node(int _val) {
        val = _val;
        neighbors = new List<Node>();
    }

    public Node(int _val, List<Node> _neighbors) {
        val = _val;
        neighbors = _neighbors;
    }
}
*/

public class Solution {
    public Node CloneGraph(Node node) {
        Dictionary <Node, Node> map = new Dictionary <Node, Node>();
        return Dfs(node, map);
    }
    private Node Dfs(Node node, Dictionary <Node, Node> map){
        if(node == null)
        return null;

        if(map.ContainsKey(node))
        return map[node];

        Node copy = new Node(node.val);
        map[node] = copy;

        foreach (Node n in node.neighbors){
            copy.neighbors.Add(Dfs(n, map));
        }
        return copy;
    }
}
