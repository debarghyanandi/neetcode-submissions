// --------------------------------------------------------------------------
// -  optimal.cs            O(n^2) time / O(n^2) space
// --------------------------------------------------------------------------

public class Solution
{
    public int[] FindMissingAndRepeatedValues(int[][] grid)
    {
        int N = grid.Length;
        HashSet<int> seen = new HashSet<int>();
        int doubleVal = 0, missing = 0;

        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                if (seen.Contains(grid[i][j]))
                {
                    doubleVal = grid[i][j];
                }
                seen.Add(grid[i][j]);
            }
        }

        for (int num = 1; num <= N * N; num++)
        {
            if (!seen.Contains(num))
            {
                missing = num;
                break;
            }
        }

        return new int[] { doubleVal, missing };
    }
}

/*
================================================================================
 PROBLEM : You get an n x n grid holding numbers from 1 to n*n. Exactly one
           number appears twice and exactly one number is missing. Return
           [repeated, missing], in that order. Example: [[1,3],[2,2]] ->
           [2,4].
 PATTERN : Hash Set (seen) + range scan
================================================================================
IDEA
  Walk every cell. If seen already holds grid[i][j], that value is doubleVal.
  Then add the value to seen. Next, check num from 1 to N*N. The first num
  not in seen is missing. This is correct because every value in 1..N*N
  appears once except two. The only value seen twice is the repeat. The only
  value never seen is the missing one.
EXAMPLE
  grid = [[1,3],[2,2]], N = 2, range 1..4
  cells: 1 new, 3 new, 2 new, 2 already in seen -> doubleVal = 2
  scan: 1 yes, 2 yes, 3 yes, 4 not in seen -> missing = 4, break
  answer [2,4]
COMPLEXITY
  Time  O(n^2)  each of the N*N cells and each of the N*N candidates is
                checked once
  Space O(n^2)  seen can hold up to N*N distinct values
PATH TO OPTIMAL
  Brute force: for each num, count it over the whole grid - O(n^4) - simple.
  Hash set (this file) - O(n^2) time and space - each cell is touched once.
  Math (sum and sum of squares) - O(n^2) time, O(1) space - no set needed.
KEYWORDS
  hash set, matrix, counting, missing number, duplicate number, math sums
WATCH OUT
  - The range is 1..N*N, not 1..N. N = grid.Length is the side length.
  - Return order is [repeated, missing]. Swapping them is a common slip.
  - With the math version, sums of squares grow like n^6. Use long, not int.
  - seen.Add returns false on a duplicate. You can use that, but do not
    skip the Add, or the scan will wrongly report the repeat as missing.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it in O(1) extra space?
     -> Let S = sum of grid - n2(n2+1)/2 = a - b and Q = squares diff =
        a^2-b^2. Then a + b = Q / S, so a = (S + Q/S)/2. O(n^2) time, O(1) space,
        use long.
  2. Any other O(1) space idea?
     -> XOR all cells with 1..N*N to get a ^ b. Split by one set bit, then
        check which group value occurs in the grid. O(n^2) time, O(1) space.
  3. What if you may change the input?
     -> Flatten and mark visits by negating the cell at index value-1. A cell
        already negative shows the repeat. A positive cell at the end shows the
        missing value. O(1) extra space, but it destroys the input.
  4. Why does the first loop find the right repeat?
     -> Only one value appears twice. So Contains is true exactly once.
TRIGGER
  Numbers in a known range 1..k with one duplicate and one gap: use a seen
  set, or sum and XOR tricks for O(1) space.
================================================================================
*/
