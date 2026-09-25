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
 PATTERN : Trie + backtracking DFS over grid (Word Search II)
 SOURCE  : Reference solution - not one you solved yourself - marker check on
           submission-0.cs when it was first processed
 STATUS  : Optimal
================================================================================
VARIABLES
  root      trie root holding all of words, built once per call
  Children  Children[ch] = next trie node for letter ch
  IsWord    true if the path from root to this node spells a whole word
  Index     Index = position in words of the word ending at this node, -1 if none
  visited   visited[row,col] = cell is on the current DFS path
  node      in Dfs, the trie node matching the letters already walked
WHY THIS PATTERN
  The problem asks for every word in words that can be spelled on the board, so
  a plain per-word search repeats the same grid walk once per word. A trie
  merges all words into one shared prefix tree, so a single DFS from each cell
  advances the board position and the trie position together. The line `if
  (!node.Children.ContainsKey(ch)) return;` is the whole point: the moment the
  letters on the path stop being a prefix of any word, the branch dies. visited
  gives the "same cell may not be used twice in one word" rule.
BRUTE FORCE
  The first version most people write is: for each word in words, run the
  classic Word Search DFS from every cell. That costs O(words.Length * m * n *
  4^L) and it re-walks identical prefixes over and over - "oath" and "oaths"
  each pay for the "oat" walk. The trie pays for shared prefixes once, which
  drops the words.Length factor out of the exponential part.
INVARIANT
  When Dfs(row, col, node, ...) is entered, node is the trie node for the exact
  letter sequence of the cells already marked in visited, and those cells form a
  simple path ending at the previous cell. Marking visited[row,col] = true
  before the four recursive calls and clearing it right after keeps that path
  property exact on entry and on exit. So whenever node.IsWord is true after the
  descent, the path really spells words[node.Index] with no repeated cell, and
  the answer is sound.
SETTING ISWORD = FALSE IS THE DE-DUPLICATION
  The same word can be spelled on the board from many start cells, but result
  must list it once. Instead of a HashSet, the code clears node.IsWord (and
  Index) the first time it collects a word, so later hits on that node add
  nothing. The side effect is that the trie is consumed: after FindWords
  returns, root no longer reports any found word, so the trie cannot be cached
  and reused across calls.
WATCH OUT
  `int cols = board[0].Length;` and `board[0].Length` inside Dfs both throw if
  board is empty or its first row is empty - there is no guard. The bounds test
  also uses board[0].Length per call rather than the cols local, so a ragged
  board (rows of different lengths) would index out of range on a shorter row.
  Order matters in Dfs: visited is written only after the trie check passes, and
  every path that writes it also clears it, so the single shared visited array
  is safe across the top-level loop - move the marking earlier and you would
  leak state on the early returns. Finally, Index = -1 is written together with
  IsWord = false; since IsWord alone stops the node from being read, that write
  is dead but harmless.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. The search still walks deep into trie branches whose words were all already
  found. How would you fix that?
     Return a child count or prune on the way out - after the four recursive
     calls, if node has no children and is not a word, remove that character
     from the parent's Children. Dead branches then fail the ContainsKey test
     immediately. Cost is passing the parent node down or returning a flag.
  2. Can you drop the visited array?
     Yes - write a sentinel such as '#' into board[row][col] before recursing
     and restore ch after. That saves the m*n bool array but mutates the
     caller's input during the run, and it breaks if '#' can legally appear in
     words.
  3. Why store Index instead of the string itself in the node?
     Storing the word directly (public string Word) removes the dependency on
     the words array being passed down through every Dfs frame and lets you
     shrink the signature. The trade-off is one extra string reference per
     terminal node instead of one int.
  4. The word list is huge and the board is small. What changes?
     Build time O(total characters of words) starts to dominate, and memory
     grows with the Dictionary per node; switch to a fixed 26-slot array or drop
     words whose letter multiset is not covered by the board before inserting
     them.
TRIGGER
  Many patterns to be matched at once against the same search space, where the
  patterns share prefixes - merge them into a trie and walk the trie alongside
  the search.
C# NOTE
  `node.Children.ContainsKey(ch)` followed by `node.Children[ch]` hashes ch
  twice on every step; `if (!node.Children.TryGetValue(ch, out var next))
  return;` does it once, and the same applies to the ContainsKey/indexer pair in
  Insert. Returning IList<string> instead of the concrete List<string> would
  also match the usual LeetCode signature without changing the body.
COMPLEXITY
  Time  : O(m * n * 4^L)
  Space : O(m * n)
================================================================================
*/
