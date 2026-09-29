// --------------------------------------------------------------------------
// -  optimal.cs            O(m * n * 4^L) time / O(m * n) space
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
 PROBLEM : Given a grid of letters (board) and a list of words, return every
           word that can be spelled on the board. A path moves up, down, left
           or right, and one cell cannot be used twice in the same word. Order
           does not matter. Ex: board [[a,b],[c,d]], words [ab,abd,ac,aba] ->
           [ac,ab,abd]
 PATTERN : Trie + Backtracking DFS on a grid
================================================================================
IDEA
  Put all words into one trie. Each end node stores IsWord and Index.
  Start Dfs from every cell. Walk down the trie as the path grows, and stop
  at once when board[row][col] is not in node.Children.
  visited marks cells on the current path and is reset on the way back.
  It is correct because every board path that matches a trie prefix is
  tried. Setting IsWord = false after a hit stops the same word being added
  twice.
EXAMPLE
  board [[a,b],[c,d]], words [ab,abd,ac,aba]; move order: up, down, left,
  right
  (0,0)a -> down (1,0)c: "ac" added; c->d has no trie child, go back
  (0,0)a -> right (0,1)b: "ab" added -> down (1,1)d: "abd" added
  b -> left (0,0)a is visited, so "aba" is not found. Answer [ac, ab, abd]
COMPLEXITY
  Time  O(m * n * 4^L)  every cell starts a DFS of depth at most L with up to
                        4 branches
  Space O(m * n)        visited grid (plus trie size and recursion depth L)
PATH TO OPTIMAL
  Word Search once per word - O(W * m*n * 4^L) - repeats the same grid work.
  Trie with one shared DFS (this file) - O(m*n * 4^L) - words with a common
  prefix share one walk, and dead prefixes are cut early. No sibling file.
KEYWORDS
  trie, prefix tree, backtracking, DFS on grid, word search, pruning
WATCH OUT
  - Forgetting visited[row, col] = false after the 4 calls: later paths
    cannot reuse the cell, and valid words are missed.
  - Without IsWord = false, a word reachable by two paths is added twice.
  - The code never deletes dead trie leaves, so fully found branches are
    still walked again. Pruning them is a big speed-up on hard tests.
  - board[0].Length throws on an empty board. Guard rows == 0 first.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How can you make it faster in practice?
     -> After a hit, remove the child from node.Children if it has no children
        left. Or store the word string in the node and use a char array of 26.
        Big-O is the same, but there are far fewer calls.
  2. Why a trie and not a HashSet of words?
     -> A set only checks full words, so you cannot stop early. The trie tells
        you "no word starts like this" and cuts the path at once.
  3. Can you save the visited memory?
     -> Write '#' into board[row][col] and restore it on the way back. Extra
        space drops to the recursion depth O(L), but the input is changed.
  4. What if diagonal moves are allowed?
     -> Add the 4 diagonal calls. The branch factor goes to 8: O(m*n * 8^L).
TRIGGER
  Many words searched on one grid or one text at the same time means: build
  a trie and run one pruned DFS.
================================================================================
*/
