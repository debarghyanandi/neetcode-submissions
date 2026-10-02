public class Solution {
    public bool IsValidSudoku(char[][] board) {
        
        //My solution
        Dictionary<int, HashSet<char>> rows = new Dictionary<int, HashSet<char>>();
        Dictionary<int, HashSet<char>> cols = new Dictionary<int, HashSet<char>>();
        Dictionary<(int row, int col), HashSet<char>> box 
        = new Dictionary<(int row, int col), HashSet<char>>();

        for (int r = 0; r < 9; r++){
            
            for (int c = 0; c < 9; c++){
                
                if(board[r][c] == '.')
                    continue;

                if(rows.ContainsKey(r) && rows[r].Contains(board[r][c])
                    || cols.ContainsKey(c) && cols[c].Contains(board[r][c])
                    || box.ContainsKey((r/3, c/3)) && box[(r/3, c/3)].Contains(board[r][c]))
                    return false;
                
                //if unique element insert it.
                //rows.ContainsKey(r) && rows[r].Contains(borad[r][c])
                if(!rows.ContainsKey(r))
                {
                    rows[r] = new HashSet<char> ();
                }
                
                if(!cols.ContainsKey(c))
                {
                    cols[c] = new HashSet<char> ();
                }

                if(!box.ContainsKey((r/3, c/3)))
                {
                    box[(r/3, c/3)] = new HashSet<char> ();
                }

                rows[r].Add(board[r] [c]);
                cols[c].Add(board[r] [c]);
                box[(r/3, c/3)].Add(board[r][c]);
                
            }
        }
        return true;
    }
}
