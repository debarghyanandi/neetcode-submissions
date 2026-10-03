// ##########################################################################
// #  optimal.cs            O(1) time / O(1) space
// ##########################################################################

public class Solution
{
    public bool IsValidSudoku(char[][] board)
    {

        //My solution
        Dictionary<int, HashSet<char>> rows = new Dictionary<int, HashSet<char>>();
        Dictionary<int, HashSet<char>> cols = new Dictionary<int, HashSet<char>>();
        Dictionary<(int row, int col), HashSet<char>> box
        = new Dictionary<(int row, int col), HashSet<char>>();

        for (int r = 0; r < 9; r++)
        {

            for (int c = 0; c < 9; c++)
            {

                if (board[r][c] == '.')
                    continue;

                if (rows.ContainsKey(r) && rows[r].Contains(board[r][c])
                    || cols.ContainsKey(c) && cols[c].Contains(board[r][c])
                    || box.ContainsKey((r / 3, c / 3)) && box[(r / 3, c / 3)].Contains(board[r][c]))
                    return false;

                //if unique element insert it.
                //rows.ContainsKey(r) && rows[r].Contains(borad[r][c])
                if (!rows.ContainsKey(r))
                {
                    rows[r] = new HashSet<char>();
                }

                if (!cols.ContainsKey(c))
                {
                    cols[c] = new HashSet<char>();
                }

                if (!box.ContainsKey((r / 3, c / 3)))
                {
                    box[(r / 3, c / 3)] = new HashSet<char>();
                }

                rows[r].Add(board[r][c]);
                cols[c].Add(board[r][c]);
                box[(r / 3, c / 3)].Add(board[r][c]);

            }
        }
        return true;
    }
}

/*
================================================================================
 PROBLEM : Given a 9x9 board of chars ('1'-'9' or '.' for empty), return true
           if the filled cells are valid. No digit may repeat in any row, any
           column, or any 3x3 box. The board does not need to be solvable.
           Example: a board with '5' at (0,0) and '5' at (1,1) -> false,
           because both are in the top-left box.
 PATTERN : Hash Set per group (rows, columns, boxes) in one pass
================================================================================
IDEA
  Scan every cell once. Skip '.' cells. For a digit at (r, c), check three
  sets: rows[r], cols[c], and box[(r / 3, c / 3)]. If any set already has
  the digit, return false. Otherwise add the digit to all three sets. This is
  correct because every cell belongs to exactly one row, one column and one
  box, so any repeat is seen the moment its second copy is scanned.
EXAMPLE
  Board: '5' at (0,0), '3' at (0,1), '5' at (1,1), all else '.'.
  (0,0) '5': no sets yet -> add to rows[0], cols[0], box[(0,0)].
  (0,1) '3': add. (1,1) '5': rows[1], cols[1] missing, box[(0,0)] has '5'.
  Return false (a box-only repeat, which row and column checks miss).
COMPLEXITY
  Time  O(1)  always 81 cells, each with O(1) hash lookups and inserts.
  Space O(1)  at most 27 sets with at most 9 digits each.
PATH TO OPTIMAL
  Three passes: check each row, then each column, then each box - O(1) -
  easy but three separate loops. This file (optimal.cs): one pass with three
  groups of sets - O(1) - same bound, but less code and an early exit.
KEYWORDS
  hash set, matrix, sudoku, box index, r / 3, duplicate detection, bitmask
WATCH OUT
  - Box key must be (r / 3, c / 3). Using (r % 3, c % 3) mixes up different
    boxes. With a flat array, use (r / 3) * 3 + c / 3.
  - Check all three sets before adding. Adding first makes every digit
    look like a duplicate of itself.
  - The 9 is hard-coded in both loops. The code does not check that cells
    hold only '1'-'9'.
  - The comment "rows.ContainsKey(r) && rows[r].Contains(borad...)" is dead
    code with a typo. Remove it.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you use less memory than hash sets?
     -> Use int rows[9], cols[9], boxes[9] as bitmasks. Bit d marks digit d.
        Test with mask & (1 << d). It is still O(1), but faster with no hashing.
  2. What if the board is n^2 x n^2 instead of 9x9?
     -> Same idea, box index (r / n, c / n). Time O(n^4), space O(n^4).
  3. Now actually solve the sudoku.
     -> Backtracking. Try each digit in an empty cell using these same sets,
        then undo it on failure. Worst case is exponential.
  4. Does valid mean solvable?
     -> No. This only checks the filled cells. Solvability needs a search.
TRIGGER
  When each item belongs to several groups and no group may repeat a value,
  keep one hash set (or bitmask) per group.
================================================================================
*/
