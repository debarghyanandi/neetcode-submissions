// --------------------------------------------------------------------------
// -  optimal.cs            O(m * n * 4^L) time / O(m * n) space
// -  Trie with DFS backtracking   [trie-dfs-backtrack]
// -  the only solution in this folder
// -
// -  Reference solution - not one you solved yourself (from submission-0)
// -
// -  DFS explores up to 4^L branching paths from each of m×n starting
// -  cells, with visited array for backtracking.
// --------------------------------------------------------------------------

public class Solution
{

    private class TrieNode
    {
        public Dictionary<char, TrieNode> Children = new();
        public bool IsWord;
        public int Index = -1;
    }

    private void Insert(TrieNode root, string word, int index)
    {
        TrieNode node = root;

        foreach (char ch in word)
        {
            if (!node.Children.ContainsKey(ch))
            {
                node.Children[ch] = new TrieNode();
            }
            node = node.Children[ch];
        }

        node.IsWord = true;
        node.Index = index;
    }


    public List<string> FindWords(char[][] board, string[] words)
    {
        TrieNode root = new TrieNode();

        for (int i = 0; i < words.Length; i++)
        {
            Insert(root, words[i], i);
        }

        List<string> result = new();

        int rows = board.Length;
        int cols = board[0].Length;

        bool[,] visited = new bool[board.Length, board[0].Length];

        //Start Dfs from every cell
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Dfs(row, col, root, board, words, result, visited);
            }
        }
        return result;
    }

    private void Dfs(int row, int col, TrieNode node,
        char[][] board, string[] words, List<string> result, bool[,] visited)
    {

        if (row < 0 || col < 0 || row >= board.Length || col >= board[0].Length)
            return;

        if (visited[row, col])
            return;

        //current char
        char ch = board[row][col];

        if (!node.Children.ContainsKey(ch))
            return;

        node = node.Children[ch];

        if (node.IsWord)
        {
            result.Add(words[node.Index]);
            node.IsWord = false;
            node.Index = -1;
        }

        visited[row, col] = true;

        Dfs(row - 1, col, node, board, words, result, visited);
        Dfs(row + 1, col, node, board, words, result, visited);
        Dfs(row, col - 1, node, board, words, result, visited);
        Dfs(row, col + 1, node, board, words, result, visited);

        visited[row, col] = false;

    }
}

/*
================================================================================
 PATTERN : Trie + Backtracking DFS - walk the grid and the trie together
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  root       the empty trie node; every word in words hangs below it
  Index      in a TrieNode, the position in words of the word that ends here (-1 if none)
  IsWord     true while a word ends at this node and has not been reported yet
  visited    visited[row, col] = true while that cell is on the current DFS path
  node       in Dfs, the trie node for the letters on the path so far (before this cell)
WHY THIS PATTERN
  The problem asks which of many words can be traced on the board, using
  adjacent cells and no cell twice. Many words share prefixes. A trie (a tree
  where each edge is one letter) lets one DFS path check all words at once. Dfs
  moves on the board and down the trie together. It stops as soon as
  board[row][col] is not in node.Children, so no path is followed that no word
  can use.
BRUTE FORCE
  Run the classic single-word search for each word in words: start a
  backtracking DFS from every cell and match the word letter by letter. This
  costs O(W * m * n * 4^L) for W words. It loses because words with the same
  prefix search the same board paths again and again. The trie shares that work
  across all words.
INVARIANT
  When Dfs is called at (row, col) with node, node spells exactly the letters on
  the current path of visited cells. So when the code steps to
  node.Children[ch], that child spells the path plus this cell. If IsWord is
  true there, the path really spells words[node.Index] on the board. Setting
  visited to true before the four recursive calls and back to false after them
  keeps each path simple (no cell used twice). It also leaves the cell free for
  other paths.
CLEAR THE FLAG TO AVOID DUPLICATES
  The same word can be found by many different paths or from many start cells.
  After adding it, the code sets node.IsWord = false and node.Index = -1, so
  later finds are ignored. You need no HashSet for the result. This does change
  the trie, but that is safe because FindWords builds a new root on every call.
WATCH OUT
  int cols = board[0].Length and the bounds check in Dfs both read board[0]. An
  empty board throws an exception before any search runs. If words has
  duplicates, Insert overwrites Index with the last one. Only one copy is
  reported, which is right for a set of words but can surprise you. Found words
  stay in the trie as dead branches. Dfs keeps walking into them even when no
  word is left to find below.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you stop searching branches that have no words left?
     After the four recursive calls, if the child node has no Children and
     IsWord is false, remove it from its parent's Children. Later searches then
     stop early. The cost is more code, and you must keep a reference to the
     parent node.
  2. Can you drop the visited array?
     Yes. Save board[row][col] in a local, write a marker such as '#' into the
     cell, recurse, then put the letter back. That saves O(m * n) memory, but
     the input board is changed while the search runs.
  3. What if the words are very long and deep recursion is a risk?
     Use an explicit stack of frames (row, col, node, next direction to try).
     You must undo visited yourself when a frame is popped. It is harder to
     read, but there is no call-stack limit.
TRIGGER
  Many words to find at once in a grid or a text, where the words share
  prefixes: build a trie and let one search walk it.
C# NOTE
  ContainsKey followed by the indexer does two dictionary lookups in both Insert
  and Dfs. TryGetValue(ch, out var next) does it in one. For lowercase-only
  input, a TrieNode[26] array indexed by ch - 'a' avoids hashing altogether.
COMPLEXITY
  Time  : O(m * n * 4^L)
  Space : O(m * n)
================================================================================
*/
