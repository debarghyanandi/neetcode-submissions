// --------------------------------------------------------------------------
// -  optimal.cs            O(L) time / O(1) space
// --------------------------------------------------------------------------

public class TrieNode
{
    public TrieNode[] children = new TrieNode[26];
    public bool isWord = false;
}

public class PrefixTree
{
    private TrieNode root;

    public PrefixTree()
    {
        root = new TrieNode();
    }

    public void Insert(string word)
    {
        TrieNode node = root;
        foreach (char c in word)
        {
            int i = c - 'a'; //store the position of that char
            if (node.children[i] == null)
            {
                node.children[i] = new TrieNode();
            }
            node = node.children[i];
        }
        node.isWord = true;

    }

    public bool Search(string word)
    {
        TrieNode node = root;
        foreach (char c in word)
        {
            int i = c - 'a'; //store the position of that char
            if (node.children[i] == null)
            {
                return false;
            }
            node = node.children[i];
        }
        return node.isWord;
    }

    public bool StartsWith(string prefix)
    {
        TrieNode node = root;
        foreach (char c in prefix)
        {
            int i = c - 'a'; //store the position of that char
            if (node.children[i] == null)
            {
                return false;
            }
            node = node.children[i];
        }
        return true;
    }
}

/*
================================================================================
 PROBLEM : Build a trie class with Insert(word), Search(word) and
           StartsWith(prefix). Search is true only if the exact word was
           inserted before. StartsWith is true if any inserted word begins
           with prefix. Words are lowercase a-z. Insert("apple");
           Search("app") -> false; StartsWith("app") -> true.
 PATTERN : Trie (prefix tree) with fixed 26-slot child array
================================================================================
IDEA
  Each TrieNode has children[26], one slot per letter, and an isWord flag.
  Insert walks from root and creates a missing child for each char (index
  i = c - 'a'), then sets isWord on the last node. Search and StartsWith walk
  the same path and fail on the first null child. At the end, Search returns
  node.isWord and StartsWith returns true. It is correct because each path
  from root spells exactly one prefix, so shared prefixes share nodes.
EXAMPLE
  Insert("apple") -> path a-p-p-l-e is created; only the 'e' node has isWord.
  Search("app") -> walk a,p,p fine, but node.isWord = false -> false.
  StartsWith("app") -> same walk -> true. Search("apx") -> 'x' null -> false.
  Insert("app") -> no new nodes, sets isWord on 2nd 'p'; Search("app") ->
  true.
COMPLEXITY
  Time  O(L)  one step per char of word or prefix; each step is an array
              lookup
  Space O(1)  walks use one node pointer; Insert adds at most L nodes of 26
              slots
PATH TO OPTIMAL
  List of words, scan all for each call - O(N*L) per call - simple baseline.
  HashSet of words + set of all prefixes - O(L) hash, O(sum L^2) memory -
  fast.
  Trie (optimal.cs) - O(L) per call, shared prefixes stored once - less
  memory.
KEYWORDS
  trie, prefix tree, children array, isWord flag, prefix search, autocomplete
WATCH OUT
  - Search must return node.isWord, not true. Otherwise "app" matches after
    only "apple" was inserted. That is the StartsWith logic, not Search.
  - c - 'a' assumes lowercase a-z. An uppercase letter or digit gives an
    index outside 0..25 and throws IndexOutOfRangeException.
  - Empty string: StartsWith("") returns true even on an empty trie, and
    Search("") is true only after Insert(""). Say so if asked.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you add Delete(word)?
     -> Walk down, clear isWord, then on the way back prune child nodes that
        have no children and no isWord. O(L) time, recursion or a stack.
  2. What if the alphabet is large, like Unicode?
     -> Use Dictionary<char, TrieNode> for children. Memory is only the edges
        that exist, but each step is a hash lookup instead of an array index.
  3. Search with '.' as a wildcard (Add and Search Words)?
     -> DFS: on '.', try all non-null children. Worst case O(26^L) per search,
        but normal letters stay O(1) per step.
  4. Return all words with a given prefix (autocomplete)?
     -> Walk to the prefix node, then DFS below it and collect words where
        isWord is true. Cost is O(L + size of that subtree).
TRIGGER
  Many lookups by prefix over a set of strings, or letter-by-letter matching.
================================================================================
*/
