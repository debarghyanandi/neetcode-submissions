public class Solution {
    private class TrieNode
    {
        public Dictionary<char, TrieNode> children = new();
        public bool isWord;
    }

    private void Insert (TrieNode root, string word) {
    
        TrieNode node = root;

        foreach (char c in word)
        {
            if(!node.children.ContainsKey(c))
            {
                node.children[c] = new TrieNode();
            }
        
            node = node.children[c];
        }

        node.isWord = true;
    }

    public string LongestCommonPrefix(string[] strs) {
        
        TrieNode root = new TrieNode();
        
        foreach(string str in strs)
        {
            Insert(root, str);
        }

        StringBuilder s = new StringBuilder();
        
        while(root.children.Count == 1 && !root.isWord)
        {
            char c = root.children.Keys.First();
            
            s.Append(c);
            root = root.children[c];
        }

        return s.ToString();
    }

}