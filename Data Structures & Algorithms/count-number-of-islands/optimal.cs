// ##########################################################################
// #  optimal.cs            O(m * n) time / O(m * n) space
// ##########################################################################

public class Solution
{
    public int NumIslands(char[][] grid)
    {
        // My solution
        int n = grid.Length;
        int m = grid[0].Length;

        // Track visited cells
        int[,] vis = new int[n, m];

        int cnt = 0;

        for (int row = 0; row < n; row++)
        {
            for (int col = 0; col < m; col++)
            {
                // Start DFS only from unvisited land
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
        int n = grid.Length;
        int m = grid[0].Length;

        // Invalid position
        if (row < 0 || row >= n ||
            col < 0 || col >= m)
        {
            return;
        }

        // Already visited
        if (vis[row, col] == 1)
            return;

        // Water cell
        if (grid[row][col] == '0')
            return;

        // Mark current land cell as visited
        vis[row, col] = 1;

        // Visit all 4 neighbours

        // Up
        Dfs(row - 1, col, vis, grid);

        // Down
        Dfs(row + 1, col, vis, grid);

        // Left
        Dfs(row, col - 1, vis, grid);

        // Right
        Dfs(row, col + 1, vis, grid);
    }
}

/*
================================================================================
 PROBLEM : Given a 2D grid of chars, '1' is land and '0' is water. Return how
           many islands there are. An island is land cells joined up, down,
           left or right (diagonals do not count). Example:
           [["1","1"],["0","1"]] -> 1.
 PATTERN : Grid DFS flood fill (connected components)
================================================================================
IDEA
  Scan every cell with row and col. When a cell is land and vis is still 0,
  it starts a new island: call Dfs, which marks every land cell reachable
  in 4 directions, then cnt++. Dfs stops at the border, at visited cells
  and at water. It is correct because each island is fully marked by its
  first cell, so no island is counted twice and none is missed.
EXAMPLE
  grid: 1 1 0 / 0 1 0 / 1 0 1
  (0,0): Dfs marks (0,0),(0,1),(1,1) -> cnt=1; (2,0): alone -> cnt=2
  (2,2): alone -> cnt=3. (1,1)-(2,0) and (1,1)-(2,2) touch only diagonally.
  Answer: 3
COMPLEXITY
  Time  O(m * n)  each cell is marked once and checked by at most 4 neighbour
                  calls
  Space O(m * n)  vis array plus recursion stack up to m*n deep
PATH TO OPTIMAL
  Label every land cell, merge labels in repeated passes - O((m*n)^2) - slow.
  Flood fill with DFS from each new land cell - O(m*n) - one visit per cell.
  optimal-variant.cs is the other O(m*n) way; same bound, different style.
KEYWORDS
  flood fill, DFS, BFS, connected components, grid graph, union-find
WATCH OUT
  - grid[0].Length throws if grid is empty; check grid.Length == 0 first.
  - A huge all-land grid makes recursion m*n deep -> StackOverflow in C#.
  - Mark vis before recursing, or neighbours call back forever.
  - Only 4 directions; adding diagonals gives a different problem.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you avoid deep recursion?
     -> Use BFS with a queue or DFS with an explicit stack. Same O(m*n) time;
        the queue holds at most O(min(m,n)) cells for BFS, no stack overflow.
  2. Can you save the vis memory?
     -> Set grid[r][c] = '0' when you visit it. O(1) extra besides the stack,
        but it changes the input, so ask if that is allowed.
  3. Land is added one cell at a time; give the count after each add?
     -> Union-find over cells: each add unions with land neighbours and
        adjusts the count. Near O(1) amortized per add, O(m*n) space.
  4. Return the size of the largest island instead?
     -> Make Dfs return 1 plus its neighbours' results; keep a max. Same cost.
TRIGGER
  A grid or graph where you must count or measure connected groups of cells.
================================================================================
*/
