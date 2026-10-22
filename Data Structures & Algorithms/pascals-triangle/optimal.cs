// --------------------------------------------------------------------------
// -  optimal.cs            O(n^2) time / O(n) space
// --------------------------------------------------------------------------

public class Solution
{
    public List<List<int>> Generate(int numRows)
    {
        var res = new List<List<int>>();
        if (numRows <= 0)
            return res;

        res.Add(new List<int> { 1 });

        for (int i = 1; i < numRows; i++)
        {
            var prev = res[i - 1];
            var temp = new List<int>(prev);
            temp.Insert(0, 0); // add 0 at start.
            temp.Add(0); //add 0 at end

            var row = new List<int>();
            //because each new row has exactly prev.Count + 1 elements.
            for (int j = 0; j < prev.Count + 1; j++)
            {
                row.Add(temp[j] + temp[j + 1]);
            }
            res.Add(row);
        }
        return res;
    }
}

/*
================================================================================
 PROBLEM : Given an integer numRows, return the first numRows rows of Pascal's
           triangle as a list of lists. Each row starts and ends with 1. Every
           inner value is the sum of the two values directly above it.
           Example: numRows = 3 -> [[1],[1,1],[1,2,1]].
 PATTERN : 1-D Dynamic Programming (build row from previous row)
================================================================================
IDEA
  Start res with [1]. For each new row i, copy the last row prev into temp
  and pad it with a 0 at both ends. Then row[j] = temp[j] + temp[j+1] for j
  from 0 to prev.Count. The zero padding makes the edge 1s come out of the
  same sum, so no special case is needed. It is correct because it is the
  identity C(i,j) = C(i-1,j-1) + C(i-1,j), with C = 0 outside the row.
EXAMPLE
  numRows = 4. Start res = [[1]].
  i=1: temp=[0,1,0] -> row=[1,1]; i=2: temp=[0,1,1,0] -> row=[1,2,1]
  i=3: temp=[0,1,2,1,0] -> row=[1,3,3,1]
  Answer: [[1],[1,1],[1,2,1],[1,3,3,1]]
COMPLEXITY
  Time  O(n^2)  row i costs O(i) to copy, pad and sum; 1+2+...+n is O(n^2)
  Space O(n)    only temp and row of size O(n) besides the returned output
PATH TO OPTIMAL
  Each entry by factorial nCr - O(n^3) with big numbers - slow, overflows.
  Row from previous row (this file) - O(n^2) - every value is one addition.
  O(n^2) is the floor: the output itself has about n^2/2 numbers.
KEYWORDS
  Pascal's triangle, dynamic programming, binomial coefficients, nCr, 2D DP
WATCH OUT
  - Off-by-one: the new row has prev.Count + 1 values and temp has
    prev.Count + 2, so j stops at prev.Count and temp[j+1] stays in range.
  - numRows <= 0 must return an empty list, not [[1]]; this code checks it.
  - Values pass int.MaxValue at row index 34 (C(34,17) > 2^31); use long
    if the interviewer asks for big rows.
  - temp.Insert(0, 0) shifts the whole list; still O(i) per row, but
    setting row[0] = row[i] = 1 directly avoids the copy.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Return only row k (Pascal's Triangle II) with O(k) extra space?
     -> Keep one array of size k+1 and update it right to left with a[j] +=
        a[j-1], so old values are not overwritten. O(k^2) time, O(k).
  2. Can you get row k faster than O(k^2)?
     -> Use C(k,j+1) = C(k,j) * (k-j) / (j+1) with long; O(k) time. Multiply
        before dividing to stay exact, and watch overflow.
  3. Why does the zero padding work?
     -> C(n,k) is 0 outside 0..n, so padding is just those zeros; the edge
        sums become 0 + 1 = 1 with no branch.
  4. Values too large?
     -> Store results modulo a prime, or use BigInteger at a speed cost.
TRIGGER
  When each value depends only on a few neighbours in the previous row or
  level, build the answer row by row from the row before it.
================================================================================
*/
