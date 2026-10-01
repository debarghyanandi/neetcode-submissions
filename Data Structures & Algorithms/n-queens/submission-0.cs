public class Solution {
    public List<List<string>> SolveNQueens(int n) 
    {
        List<List<string>> ans = new List<List<string>>();
        
        char [,] board = new char [n, n];
        
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                board[i, j] = '.';
            }
        }
        
        Solve(0, n, board, ans);

        return ans;
    }
    
    private void Solve(int col, int n, char [,] board, List<List<string>> ans)
    {
        if (col == n)
        {
            AddBoard(board, ans, n);
            return;
        }
        for (int row = 0; row < n; row++)
        {
           if (IsSafe(row, col, board, n))
           {
                board[row, col] = 'Q';
            
                Solve(col + 1, n, board, ans);
            
                board[row, col] = '.';
           }
        }
    }

    private bool IsSafe(int row, int col, char[,] board, int n){
       
       int storeRow = row;
       int storeCol = col;
       
       //we need to just check the prev 3 direction
       //Upper diagonal
       while (row >= 0 && col >= 0)
        {
            if (board[row, col] == 'Q')
                return false;

            row--;
            col--;
        }

       row = storeRow;
       col = storeCol;

       //left row
       while (col >= 0)
        {
            if (board[row, col] == 'Q')
            return false;

            col--;
        }

        row = storeRow;
        col = storeCol;
       
       //Lower diagonal
       while(row < n && col >= 0)
        {
            if (board[row, col] == 'Q')
            return false;

            row++;
            col--;
        }

        return true;
    }

    private void AddBoard (char [,] board, List<List<string>> ans, int n)
    {
        List<string> currentBoard = new List<string>();
        for(int i = 0; i < n; i++)
        {
            char[] temp = new char[n];
            for(int j = 0; j < n; j++)
            {
                temp[j] = board[i, j];
            }
            currentBoard.Add(new string(temp));
        }
        ans.Add(currentBoard);
    }
}
