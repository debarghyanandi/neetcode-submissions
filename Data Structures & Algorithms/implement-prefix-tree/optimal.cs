// --------------------------------------------------------------------------
// -  optimal.cs            O(L) time / O(1) space
// -  Trie prefix tree   [trie]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself (from submission-0)
// -
// -  Each operation traverses characters in sequence; trie nodes created
// -  during insertion are part of the persistent data structure, not
// -  auxiliary space
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
 PATTERN : Trie (Prefix Tree) - one child slot per letter
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  node   the trie node for the characters read so far
  i      child slot for char c: i = c - 'a', so 'a' -> 0 and 'z' -> 25
WHY THIS PATTERN
  The problem asks for exact-word lookup and also "does any word start with this
  prefix". Words that share a prefix should share storage and share work. A trie
  stores each prefix once as a path from root. Each char picks one of 26
  children, so a query only walks down the path for its own characters. It never
  looks at other words.
BRUTE FORCE
  Keep every inserted word in a List<string>. Search compares against each word,
  and StartsWith calls word.StartsWith(prefix) on each one. Both cost O(N * L)
  for N stored words. A HashSet<string> makes Search fast, but StartsWith must
  still scan every word. The trie makes both queries depend only on the query
  length.
INVARIANT
  After the loop reads k characters, node is the unique node for the string of
  those first k characters. That node exists only if some inserted word starts
  with that string. So hitting a null child means no stored word has this
  prefix, and false is correct. isWord is set only at the node where an Insert
  loop ended. So it is true exactly when that full string was inserted.
SEARCH VS STARTSWITH
  The two walks are the same. They differ only in the last line. Search returns
  node.isWord, and StartsWith returns true. After Insert("apple"), Search("app")
  is false because the 'p' node has isWord false. StartsWith("app") is true
  because the path exists.
WATCH OUT
  The index c - 'a' assumes lowercase a-z only. An uppercase letter, digit or
  space gives an index below 0 or above 25, and children[i] throws
  IndexOutOfRangeException. An empty string is not rejected. Insert("") sets
  root.isWord to true, so Search("") then returns true, and StartsWith("") is
  always true. The same walk loop is copied three times, so a fix in one copy
  can easily be missed in the others. Insert also allocates up to one new
  TrieNode per character. Only Search and StartsWith use no extra memory.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How would you support Delete(word)?
     Walk the path and clear isWord. To free memory, also remove nodes that have
     no children and are not words. Do this bottom-up, with recursion or a stack
     of visited nodes. Another option is a per-node count of words passing
     through, which is simpler but costs one int per node.
  2. How would you count how many words start with a prefix?
     Add a prefixCount field and increment it on every node Insert passes
     through. The query then walks the prefix and returns that count. This is
     one extra int per node.
  3. What if the alphabet is large, for example Unicode?
     Replace the fixed array with a Dictionary<char, TrieNode>. Memory now grows
     only with the children that really exist. The cost is a hash lookup per
     step instead of an array index.
  4. What if Search allows '.' to match any letter?
     At a '.', try all non-null children with a DFS (depth-first search: go deep
     on one branch, then back up). The worst case becomes branching over 26
     children at every dot.
TRIGGER
  Many queries that ask "is this a word" or "does anything start with this" over
  a shared set of strings.
C# NOTE
  The initializer isWord = false is redundant. C# fields default to false, and
  new TrieNode[26] already fills every slot with null. A private helper that
  walks a string and returns the final node (or null) would remove the three
  copied loops.
COMPLEXITY
  Time  : O(L)
  Space : O(1)
================================================================================
*/
