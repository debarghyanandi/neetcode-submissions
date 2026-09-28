public class TrieNode{
    public TrieNode[] children = new TrieNode[26];
    public bool isWord = false;
}

public class PrefixTree {
    private TrieNode root;

    public PrefixTree() {
        root = new TrieNode(); 
    }
    
    public void Insert(string word) {
        TrieNode node = root;
        foreach(char c in word){
            int i = c - 'a'; //store the position of that char
            if(node.children[i] == null){
                node.children[i] = new TrieNode ();
            }
            node = node.children[i];
        }
        node.isWord = true;

    }
    
    public bool Search(string word) {
        TrieNode node = root;
        foreach(char c in word){
            int i = c - 'a'; //store the position of that char
            if(node.children[i] == null){
                return false;
            }
            node = node.children[i];
        }
        return node.isWord;
    }
    
    public bool StartsWith(string prefix) {
        TrieNode node = root;
        foreach(char c in prefix){
            int i = c - 'a'; //store the position of that char
            if(node.children[i] == null){
                return false;
            }
            node = node.children[i];
        }
        return true;
    }
}
