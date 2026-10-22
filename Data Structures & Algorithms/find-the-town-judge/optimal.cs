// --------------------------------------------------------------------------
// -  optimal.cs            O(n + m) time / O(n) space
// --------------------------------------------------------------------------

public class Solution
{
    public int FindJudge(int n, int[][] trust)
    {
        int[] outd = new int[n + 1];
        int[] ind = new int[n + 1];

        foreach (var item in trust)
        {
            int ai = item[0];
            int bi = item[1];
            ind[bi]++;
            outd[ai]++;
        }
        for (int i = 0; i <= n; i++)
        {
            if (outd[i] == 0 && ind[i] == n - 1)
            {
                return i;
            }
        }
        return -1;
    }
}

/*
================================================================================
 PROBLEM : A town has n people labeled 1..n. trust[i] = [a, b] means a trusts
           b. The judge trusts nobody, and everyone else trusts the judge.
           Return the judge's label, or -1 if no such person exists. Example:
           n = 3, trust = [[1,3],[2,3]] -> 3.
 PATTERN : Graph in-degree / out-degree counting
================================================================================
IDEA
  Treat each trust pair as a directed edge ai -> bi. Count edges leaving
  each person in outd and edges arriving in ind. The judge is the person
  with outd[i] == 0 and ind[i] == n - 1. This is correct because the two
  rules of the problem are exactly these two degree conditions.
EXAMPLE
  n = 4, trust = [[1,3],[2,3],[4,3],[1,2]]
  outd: 1->2, 2->1, 3->0, 4->1 ind: 2->1, 3->3, 1->0, 4->0
  i=1,2: outd > 0, so skip. i=3: outd 0 and ind 3 == n-1 -> return 3.
COMPLEXITY
  Time  O(n + m)  one pass over the m pairs, then one scan over the n+1 slots
  Space O(n)      two count arrays of size n + 1
PATH TO OPTIMAL
  Brute force: for each person, scan all pairs - O(n*m) - simple but slow.
  Adjacency matrix: check row and column per person - O(n^2) - fast lookup,
  but uses n^2 memory.
  Two degree arrays (optimal.cs) - O(n + m) - one pass, linear memory.
KEYWORDS
  directed graph, in-degree, out-degree, counting array, celebrity problem
WATCH OUT
  - The scan starts at i = 0, an unused slot. With n = 1 and empty trust,
    slot 0 has outd 0 and ind 0 == n-1, so it returns 0. The answer is 1.
    Start the loop at i = 1.
  - Do not check only ind[i] == n - 1. Someone can be trusted by all others
    but also trust a person. outd[i] == 0 is required.
  - Arrays sized n, not n + 1, cause an index error for label n.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. Can you do it with one array?
     -> Use score[bi]++ and score[ai]--. The judge has score n - 1. Still O(n
        + m) time, but only one array of size n + 1.
  2. What if you can only ask knows(a, b) (Celebrity Problem)?
     -> Keep a candidate. If candidate knows i, set candidate = i. Then verify
        with one more pass. O(n) calls and O(1) space.
  3. Can duplicate pairs appear?
     -> Then counts break, since one person adds 2 to ind. Dedupe with a hash
        set of pairs first. That adds O(m) extra space.
  4. Why is there at most one judge?
     -> Two judges would need to trust each other, but a judge trusts nobody.
TRIGGER
  When an answer depends on how many edges point in or out of each node,
  count degrees in arrays and do not build the graph.
================================================================================
*/
