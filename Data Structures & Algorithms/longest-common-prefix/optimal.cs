// --------------------------------------------------------------------------
// -  optimal.cs            O(n * m) time / O(1) space
// --------------------------------------------------------------------------

public class Solution
{
    public string LongestCommonPrefix(string[] strs)
    {
        string ans = "";
        bool isMatch = true;

        for (int i = 0; i < strs[0].Length; i++)
        {
            char prefix = strs[0][i];

            for (int j = 1; j < strs.Length; j++)
            {

                //Not Match
                if (strs[j].Length <= i || prefix != strs[j][i])
                {
                    isMatch = false;
                    break;
                }
            }

            if (isMatch == false)
                break;
            else
                ans += prefix;
        }
        return ans;
    }
}

/*
================================================================================
 PROBLEM : Given an array of strings strs, return the longest string that is a
           prefix of every string in it. If there is no common prefix, return
           "". Example: ["flower","flow","flight"] -> "fl".
 PATTERN : Vertical Scanning (column by column)
================================================================================
IDEA
  Use strs[0] as the reference. For each index i, take prefix = strs[0][i].
  Check that every other string strs[j] is long enough and has the same
  char at i. On the first failure, set isMatch = false and stop. Otherwise
  append prefix to ans. This is correct because a common prefix must match
  at every index from the start, so the first bad column ends the answer.
EXAMPLE
  strs = ["flower","flow","flight"]
  i=0 'f': all match -> ans="f"; i=1 'l': all match -> ans="fl"
  i=2 'o': strs[2][2]='i' differs -> isMatch=false, break
  Answer: "fl"
COMPLEXITY
  Time  O(n * m)  at most m columns, each compared across n strings
  Space O(1)      only i, j, prefix and the flag (ans is output)
PATH TO OPTIMAL
  Brute force: try each prefix of strs[0], longest first, and test all
  strings - O(n*m^2) - repeats work for every candidate prefix.
  suboptimal.cs - O(n*m) time, O(n*m) extra space - checks once per char.
  optimal.cs - same time, O(1) extra - compares chars in place, no storage.
KEYWORDS
  longest common prefix, vertical scanning, horizontal scanning, string, trie
WATCH OUT
  - ans += prefix builds a new string each time: real cost is O(m^2)
    copying. Use a StringBuilder, or return strs[0].Substring(0, i).
  - Empty strs (length 0) throws at strs[0]. Return "" first if allowed.
  - Check strs[j].Length <= i BEFORE strs[j][i], or "flo" vs "flower"
    reads past the end. The || short-circuit is what makes this safe.
FOLLOW-UP AN INTERVIEWER WILL ASK
  1. How do you do it with sorting?
     -> Sort strs, then compare only the first and last strings. Time is
        O(n*m*log n) for the sort, but the scan is simple.
  2. Many queries against the same set of strings?
     -> Build a trie once in O(total chars). Walk down while a node has one
        child and is not a word end. Costs more memory, but queries are fast.
  3. Can you use binary search?
     -> Binary search on the prefix length L in [0, minLen]. Test "do all
        start with strs[0][0..L)?" Time O(n*m*log m). Not faster, but a common
        thing to discuss.
  4. Strings arrive as a stream?
     -> Keep the current prefix and shrink it against each new string. O(m)
        memory, and each string is read once.
TRIGGER
  When you must find what all strings share from the start, compare them
  position by position and stop at the first mismatch.
================================================================================
*/
