// --------------------------------------------------------------------------
// -  optimal.cs            O(n * n!) time / O(n^2) space
// --------------------------------------------------------------------------

public class Solution
{
    public List<List<string>> SolveNQueens(int n)
    {
        List<List<string>> ans = new List<List<string>>();

        char[,] board = new char[n, n];

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

    private void Solve(int col, int n, char[,] board, List<List<string>> ans)
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

    private bool IsSafe(int row, int col, char[,] board, int n)
    {

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
        while (row < n && col >= 0)
        {
            if (board[row, col] == 'Q')
                return false;

            row++;
            col--;
        }

        return true;
    }

    private void AddBoard(char[,] board, List<List<string>> ans, int n)
    {
        List<string> currentBoard = new List<string>();
        for (int i = 0; i < n; i++)
        {
            char[] temp = new char[n];
            for (int j = 0; j < n; j++)
            {
                temp[j] = board[i, j];
            }
            currentBoard.Add(new string(temp));
        }
        ans.Add(currentBoard);
    }
}

/*
================================================================================
 PROBLEM : Given n, place n queens on an n x n board so that no two queens
           attack. Two queens attack if they share a row, a column, or a
           diagonal. Return every valid board, each as n strings of 'Q' and
           '.'. Example: n = 4 ->
           [["..Q.","Q...","...Q",".Q.."],[".Q..","...Q","Q...","..Q."]]
 PATTERN : Backtracking (DFS, one queen per column)
================================================================================
IDEA
  Solve fills one column at a time, from col 0 to col n-1. For each row it
  asks IsSafe, places 'Q', recurses on col + 1, then resets the cell to '.'.
  Only columns to the left hold queens, so IsSafe scans just three ways:
  upper-left diagonal, same row to the left, and lower-left diagonal.
  It is correct because every column gets exactly one queen, and each new
  queen is checked against all earlier ones. So no attack is ever missed.
EXAMPLE
  n = 4. col0 row0 -> col1 row2 (col2 dead) or row3 (col3 dead): backtrack.
  col0 row1 -> col1 row3 -> col2 row0 -> col3 row2 -> col == n, AddBoard.
  col0 row2 gives the mirror board. col0 row3 fails.
  Answer: [["..Q.","Q...","...Q",".Q.."],[".Q..","...Q","Q...","..Q."]]
COMPLEXITY
  Time  O(n * n!)  about n! partial boards survive pruning, each IsSafe scan
                   is O(n)
  Space O(n^2)     the n x n board plus recursion depth n (output not counted)
PATH TO OPTIMAL
  Try every set of n cells, then check - C(n^2, n) boards - very slow.
  One queen per column, try all n^n row choices - smaller, still no pruning.
  Backtrack and stop a branch at the first conflict - O(n * n!) - this file.
  (There is no sibling file; every step past brute force is described here.)
KEYWORDS
  backtracking, DFS, N-Queens, constraint satisfaction, pruning, diagonals
WATCH OUT
  - Forgetting board[row, col] = '.' after the recursive call. Old queens
    stay on the board and later branches are wrongly rejected.
  - You place by column, but the output strings are rows. AddBoard reads
    board[i, j] row by row. Do not swap the indexes.
  - n = 2 and n = 3 have no solution. The code returns an empty list.
  - With hash sets, the key row - col can be negative. Add n - 1 to use it
    as an array index.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you make the safety check O(1)?
     -> Keep three bool arrays: used rows, row + col, row - col + n - 1. Set
        them when placing and clear them when removing. O(n!) time, O(n) space.
  2. Only count the solutions (N-Queens II)?
     -> Same backtracking, but increment a counter instead of AddBoard. No
        board copies. Bitmasks for rows and diagonals make it very fast.
  3. Can symmetry help?
     -> Try only the first half of the rows in col 0 and double the count. For
        odd n, handle the middle row on its own. About 2x faster.
  4. Very large n and you need just one board?
     -> Backtracking is too slow there. Use a known constructive pattern in
        O(n), or min-conflicts local search (move the queen with most attacks).
TRIGGER
  When you must list all arrangements that obey "no two may conflict" rules,
  build one choice at a time and backtrack at the first conflict.
================================================================================
*/
