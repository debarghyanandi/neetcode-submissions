public class Solution {
    
    private class TrieNode{
    public Dictionary<char, TrieNode> Children = new ();
    public bool IsWord;
    public int Index = -1;
    }
    
    private void Insert(TrieNode root, string word, int index)
    {
        TrieNode node = root;
        
        foreach(char ch in word)
        {
            if(!node.Children.ContainsKey(ch))
            {
                node.Children[ch] = new TrieNode();
            }
            node = node.Children[ch];
        }
        
        node.IsWord = true;
        node.Index = index;
    }


    public List<string> FindWords(char[][] board, string[] words) {
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
            for(int col = 0; col < cols; col++)
            {
                Dfs(row, col, root, board, words, result, visited);
            }
        }
        return result;
    }

    private void Dfs(int row, int col, TrieNode node,
        char[][] board, string[] words, List<string> result, bool[,] visited)
        {

            if(row < 0 || col < 0 || row >= board.Length || col >= board[0].Length)
                return;

            if(visited [row,col])
                return;

            //current char
            char ch = board[row][col];

            if(!node.Children.ContainsKey(ch))
                return;

            node = node.Children[ch];

            if(node.IsWord)
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
