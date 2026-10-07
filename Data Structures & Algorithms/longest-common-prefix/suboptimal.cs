// --------------------------------------------------------------------------
// -  suboptimal.cs         O(n * m) time / O(n * m) space
// --------------------------------------------------------------------------

public class Solution
{
    private class TrieNode
    {
        public Dictionary<char, TrieNode> children = new();
        public bool isWord;
    }

    private void Insert(TrieNode root, string word)
    {

        TrieNode node = root;

        foreach (char c in word)
        {
            if (!node.children.ContainsKey(c))
            {
                node.children[c] = new TrieNode();
            }

            node = node.children[c];
        }

        node.isWord = true;
    }

    public string LongestCommonPrefix(string[] strs)
    {

        TrieNode root = new TrieNode();

        foreach (string str in strs)
        {
            Insert(root, str);
        }

        StringBuilder s = new StringBuilder();

        while (root.children.Count == 1 && !root.isWord)
        {
            char c = root.children.Keys.First();

            s.Append(c);
            root = root.children[c];
        }

        return s.ToString();
    }

}

/*
================================================================================
 PROBLEM : Given an array of strings strs, return the longest prefix that
           every string starts with. If no such prefix exists, return "".
           Example: ["flower","flow","flight"] -> "fl".
 PATTERN : Trie (prefix tree) + walk the single-child chain
================================================================================
IDEA
  Insert every string into a trie, one node per character. Then start at
  root and walk down while the node has exactly one child and isWord is
  false, appending each char to s. One child means every string continues
  with that same char. A second child or an ended word means they split.
  Unlike optimal.cs, it builds a whole trie instead of comparing in place.
EXAMPLE
  strs = ["flower","flow","flight"]: root has only 'f' -> s="f";
  'f' has only 'l' -> s="fl"; 'l' has 'o' and 'i' (Count 2) -> stop.
  Answer "fl". Tricky: ["ab","a"] -> append 'a'; that node has isWord
  true, so stop -> "a" (child 'b' is ignored).
COMPLEXITY
  Time  O(n * m)  every char of every string is inserted once; the walk is
                  shorter
  Space O(n * m)  up to one TrieNode with its own Dictionary per inserted char
WATCH OUT
  - Dropping the !root.isWord check breaks ["ab","a"]: it returns "ab".
  - An empty string in strs sets the root's isWord to true, so the loop
    never runs and the answer is "". That is correct; do not special-case.
  - Keys.First() needs System.Linq and StringBuilder needs System.Text.
    The code compiles only if those usings exist.
  - The loop reassigns root itself. Fine here, but any later use of root
    would see the deepest node, not the real root.
================================================================================
*/
