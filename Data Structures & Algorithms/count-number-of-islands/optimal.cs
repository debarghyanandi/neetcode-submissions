// ##########################################################################
// #  optimal.cs            O(m * n) time / O(m * n) space
// ##########################################################################

public class Solution
{
    public int NumIslands(char[][] grid)
    {
        //My solution
        int rows = grid.Length;
        int cols = grid[0].Length;

        // Visited array
        int[,] vis = new int[rows, cols];
        int cnt = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (vis[row, col] == 0 && grid[row][col] == '1')
                {
                    Dfs(row, col, vis, grid);
                    cnt++;
                }
            }
        }
        return cnt;
    }

    private void Dfs(int row, int col, int[,] vis, char[][] grid)
    {
        int rows = grid.Length;
        int cols = grid[0].Length;

        //mark this node visisted
        vis[row, col] = 1;

        //visit all 6 neighbours
        for (int delRow = -1; delRow <= 1; delRow++)
        {
            for (int delCol = -1; delCol <= 1; delCol++)
            {
                //exclude the node itself(0,0) and diagonal nodes
                if (Math.Abs(delRow) == Math.Abs(delCol))
                    continue;

                int nRow = row + delRow;
                int nCol = col + delCol;

                //invalid neighbour validation
                //land validation
                //visited validation
                if (nRow >= 0 && nRow < rows &&
                    nCol >= 0 && nCol < cols &&
                    vis[nRow, nCol] == 0 &&
                    grid[nRow][nCol] == '1')
                {
                    Dfs(nRow, nCol, vis, grid);
                }
            }
        }
    }
}

/*
================================================================================
 PROBLEM : You get a 2D grid of chars: '1' is land and '0' is water. Return
           how many islands there are. An island is land cells joined up,
           down, left or right. Diagonal cells do NOT join. Example:
           ["11","01","10"] -> 2
 PATTERN : Grid DFS flood fill + connected components count
================================================================================
IDEA
  Scan every cell with row and col. When a cell is '1' and vis is 0, it is a
  new island: call Dfs and add 1 to cnt. Dfs marks the whole island in vis,
  so no later cell of that island starts a new count. The delRow/delCol loop
  skips cells where |delRow| == |delCol| (the center and 4 diagonals).
  Correct because each Dfs covers exactly one connected component.
EXAMPLE
  grid: 110 / 010 / 101
  (0,0) new -> Dfs marks (0,0),(0,1),(1,1); cnt=1
  (2,0) new -> cnt=2; (2,2) new -> cnt=3 (diagonal to (1,1) does not join)
  Answer: 3
COMPLEXITY
  Time  O(m * n)  each cell is marked once, and each check looks at 9 offsets
  Space O(m * n)  vis matrix, plus a recursion stack as deep as one island
PATH TO OPTIMAL
  Flood from every '1' with no shared visited set - O((m*n)^2) - repeats work.
  Shared vis matrix, count each Dfs start - O(m*n) - each cell done once.
  optimal-variant.cs - same O(m*n) with a different traversal choice.
KEYWORDS
  flood fill, DFS, BFS, connected components, grid graph, union-find, visited
WATCH OUT
  - The comment "visit all 6 neighbours" is wrong. The loop checks 9 offsets
    and skips 5 of them, so it visits 4 neighbours.
  - A large all-'1' grid makes recursion depth m*n. This can cause a stack
    overflow in C#. Use an explicit stack or BFS queue instead.
  - An empty grid (rows == 0) crashes on grid[0].Length. Return 0 first.
  - Mark vis before you recurse. If you mark after, cells get pushed twice.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it with less extra memory?
     -> Set grid cells to '0' when you visit them, and drop vis. Extra space
        is then only the stack. The trade-off is that the input gets changed.
  2. What if land cells are added one at a time (Number of Islands II)?
     -> Use union-find, and union each new cell with its land neighbours. Each
        add is near O(1) amortized, so you never re-scan the grid.
  3. Return the size of the largest island instead?
     -> Make Dfs return 1 plus the sum of its neighbours' results, and keep a
        max. Time and space stay O(m*n).
  4. Should diagonals count as connected?
     -> Loop all 8 offsets and skip only (0,0). Time and space stay the same.
TRIGGER
  When a grid asks you to count or measure groups of touching cells, use a
  flood fill with DFS or BFS.
================================================================================
*/
